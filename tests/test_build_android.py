import importlib.util
from pathlib import Path
import tempfile
import unittest
import zipfile


ROOT = Path(__file__).resolve().parents[1]
spec = importlib.util.spec_from_file_location('build_android', ROOT / 'tools/build_android.py')
build = importlib.util.module_from_spec(spec)
spec.loader.exec_module(build)


class PackagingTests(unittest.TestCase):
    def test_missing_reference_rejected_before_build(self):
        with tempfile.TemporaryDirectory() as tmp:
            with self.assertRaisesRegex(ValueError, 'Missing reference'):
                build.validate_refs(Path(tmp))

    def test_duplicate_reference_rejected(self):
        with tempfile.TemporaryDirectory() as tmp:
            refs = Path(tmp)
            for name in build.REFERENCE_NAMES:
                (refs / name).write_bytes(b'MZ')
            (refs / 'nested').mkdir()
            (refs / 'nested/StardewValley.dll').write_bytes(b'MZ')
            with self.assertRaisesRegex(ValueError, 'Ambiguous reference'):
                build.validate_refs(refs)

    def test_probe_archive_has_only_entry_and_manifest(self):
        with tempfile.TemporaryDirectory() as tmp:
            base = Path(tmp)
            dll = base / 'DualScreen.dll'
            dll.write_bytes(b'MZ-owned-mod')
            out = base / 'mod.zip'
            build.package_probe(dll, {'EntryDll': 'DualScreen.dll'}, out)
            with zipfile.ZipFile(out) as z:
                self.assertEqual(set(z.namelist()), {'ThorDualScreen/manifest.json', 'ThorDualScreen/DualScreen.dll'})
                self.assertIsNone(z.testzip())

    def test_external_entry_dll_rejected(self):
        with tempfile.TemporaryDirectory() as tmp:
            base = Path(tmp)
            dll = base / 'DualScreen.dll'
            dll.write_bytes(b'MZ-owned-mod')
            with self.assertRaisesRegex(ValueError, 'EntryDll'):
                build.package_probe(dll, {'EntryDll': '../StardewValley.dll'}, base / 'mod.zip')


if __name__ == '__main__':
    unittest.main()
