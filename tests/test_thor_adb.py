"""Boundary tests: serial scope, binary capture, and honest staging results."""
import hashlib
import importlib.util
from pathlib import Path
import subprocess
import tempfile
import unittest
from unittest.mock import patch

HELPER = Path(__file__).resolve().parents[1] / "tools" / "thor_adb.py"


class ThorAdbTests(unittest.TestCase):
    def setUp(self):
        self.assertTrue(HELPER.is_file(), "serial-scoped ADB helper is not implemented")
        spec = importlib.util.spec_from_file_location("thor_adb", HELPER)
        self.mod = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(self.mod)

    def test_capture_preserves_binary_and_uses_physical_id(self):
        payload = b"\x89PNG\r\n\x1a\n\x00\xff\r\n"
        with tempfile.TemporaryDirectory() as temp, patch.object(self.mod.subprocess, "run") as run:
            run.return_value = subprocess.CompletedProcess([], 0, payload, b"")
            out = Path(temp) / "bottom.png"
            self.mod.capture("test-serial", "987654321", out)
            self.assertEqual(out.read_bytes(), payload)
            self.assertEqual(run.call_args.args[0], ["adb", "-s", "test-serial", "exec-out", "screencap", "-p", "-d", "987654321"])
            self.assertGreater(run.call_args.kwargs["timeout"], 0)
            self.assertTrue(run.call_args.kwargs["check"])
            self.assertNotEqual(run.call_args.kwargs.get("text"), True)

    def test_failed_capture_does_not_create_file(self):
        with tempfile.TemporaryDirectory() as temp, patch.object(self.mod.subprocess, "run") as run:
            run.side_effect = subprocess.CalledProcessError(1, ["adb"], stderr=b"unauthorized")
            out = Path(temp) / "bottom.png"
            with self.assertRaises(subprocess.CalledProcessError):
                self.mod.capture("test-serial", "987654321", out)
            self.assertFalse(out.exists())

    def test_capture_rejects_non_png_response(self):
        with tempfile.TemporaryDirectory() as temp, patch.object(self.mod.subprocess, "run") as run:
            run.return_value = subprocess.CompletedProcess([], 0, b"unknown display", b"")
            out = Path(temp) / "bottom.png"
            with self.assertRaises(ValueError):
                self.mod.capture("test-serial", "987654321", out)
            self.assertFalse(out.exists())

    def test_stage_reports_only_staging_and_checks_device_bytes(self):
        with tempfile.TemporaryDirectory() as temp, patch.object(self.mod.subprocess, "run") as run:
            archive = Path(temp) / "mod.zip"
            archive.write_bytes(b"test archive")
            digest = hashlib.sha256(b"test archive").hexdigest()
            run.side_effect = [subprocess.CompletedProcess([], 0, b"pushed", b""), subprocess.CompletedProcess([], 0, (digest + "  /sdcard/Download/file.zip\n").encode(), b"")]
            result = self.mod.stage("test-serial", archive)
            self.assertTrue(result["checksumVerified"])
            self.assertFalse(result["imported"])
            self.assertFalse(result["selected"])
            self.assertEqual(result["sha256"], digest)
            for call in run.call_args_list:
                self.assertEqual(call.args[0][:3], ["adb", "-s", "test-serial"])
                self.assertGreater(call.kwargs["timeout"], 0)
                self.assertTrue(call.kwargs["check"])

    def test_stage_rejects_checksum_mismatch(self):
        with tempfile.TemporaryDirectory() as temp, patch.object(self.mod.subprocess, "run") as run:
            archive = Path(temp) / "mod.zip"
            archive.write_bytes(b"test archive")
            run.side_effect = [subprocess.CompletedProcess([], 0, b"pushed", b""), subprocess.CompletedProcess([], 0, ("0" * 64 + "  file.zip\n").encode(), b"")]
            with self.assertRaises(ValueError):
                self.mod.stage("test-serial", archive)

    def test_empty_serial_rejected_before_adb(self):
        with patch.object(self.mod.subprocess, "run") as run:
            with self.assertRaises(ValueError):
                self.mod.adb("", "get-state")
            run.assert_not_called()


if __name__ == "__main__":
    unittest.main()
