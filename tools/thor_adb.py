#!/usr/bin/env python3
"""Serial-scoped device discovery, binary capture, and mod ZIP staging.

Inventory contains private device data. Store it under artifacts/private/.
Staging does not import, enable, select, or launch a mod.
"""
import argparse
from datetime import datetime, timezone
import hashlib
import json
from pathlib import Path
import subprocess
import sys


def adb(serial, *args, timeout=30):
    if not serial or not serial.strip():
        raise ValueError("An explicit authorized device serial is required")
    return subprocess.run(
        ["adb", "-s", serial, *args],
        stdout=subprocess.PIPE, stderr=subprocess.PIPE,
        timeout=timeout, check=True,
    ).stdout


def capture(serial, physical_id, out):
    if not str(physical_id).isdigit():
        raise ValueError("Use a numeric physical SurfaceFlinger display ID")
    data = adb(serial, "exec-out", "screencap", "-p", "-d", str(physical_id))
    if not data.startswith(b"\x89PNG\r\n\x1a\n"):
        raise ValueError("Device returned no PNG; check its screencap help and physical ID")
    out = Path(out)
    out.parent.mkdir(parents=True, exist_ok=True)
    with out.open("xb") as stream:
        stream.write(data)
    return {"path": str(out), "bytes": len(data), "sha256": hashlib.sha256(data).hexdigest()}


def stage(serial, archive):
    archive = Path(archive).resolve(strict=True)
    if archive.suffix.lower() != ".zip":
        raise ValueError("Stage accepts a mod ZIP only")
    digest = hashlib.sha256(archive.read_bytes()).hexdigest()
    destination = f"/sdcard/Download/ThorDualScreen-{digest}.zip"
    adb(serial, "push", str(archive), destination, timeout=120)
    device_digest = adb(serial, "shell", "sha256sum", destination).decode("utf-8").split()[0].lower()
    if device_digest != digest:
        raise ValueError("Staged file checksum differs from local ZIP")
    return {"destination": destination, "sha256": digest, "checksumVerified": True,
            "imported": False, "selected": False}


def inventory(serial, out):
    if adb(serial, "get-state").strip() != b"device":
        raise ValueError("Selected device is not ready")
    report = {"recordedUtc": datetime.now(timezone.utc).isoformat(), "serial": serial, "queries": {}}
    commands = {
        "manufacturer": ["getprop", "ro.product.manufacturer"],
        "model": ["getprop", "ro.product.model"],
        "api": ["getprop", "ro.build.version.sdk"],
        "abi": ["getprop", "ro.product.cpu.abilist"],
        "firmware": ["getprop", "ro.build.fingerprint"],
        "androidUser": ["am", "get-current-user"],
        "display": ["dumpsys", "display"],
        "captureDisplays": ["dumpsys", "SurfaceFlinger", "--display-id"],
        "input": ["dumpsys", "input"],
        "inputHelp": ["input", "--help"],
        "captureHelp": ["screencap", "-h"],
    }
    for package in ("com.chucklefish.stardewvalley", "org.junimogate.app"):
        commands[package] = ["dumpsys", "package", package]
        commands[package + ".paths"] = ["pm", "path", package]
        commands[package + ".installer"] = ["pm", "list", "packages", "-i", package]
    for name, args in commands.items():
        try:
            output = adb(serial, "shell", *args).decode("utf-8", errors="replace")
            report["queries"][name] = {"exitCode": 0, "output": output}
        except subprocess.CalledProcessError as error:
            # Some Android help commands and absent package queries return nonzero.
            # Preserve the actual result for inspection; never call it successful.
            report["queries"][name] = {
                "exitCode": error.returncode,
                "output": (error.stdout or b"").decode("utf-8", errors="replace"),
                "error": (error.stderr or b"").decode("utf-8", errors="replace"),
            }
    out = Path(out)
    out.parent.mkdir(parents=True, exist_ok=True)
    with out.open("x", encoding="utf-8") as stream:
        json.dump(report, stream, indent=2)
    return {"path": str(out), "failedQueries": [name for name, value in report["queries"].items() if value["exitCode"] != 0]}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    commands = parser.add_subparsers(dest="command", required=True)
    for name in ("inventory", "capture", "stage"):
        command = commands.add_parser(name)
        command.add_argument("--serial", required=True)
        if name == "stage":
            command.add_argument("--zip", required=True, type=Path)
        else:
            command.add_argument("--out", required=True, type=Path)
        if name == "capture":
            command.add_argument("--physical-id", required=True)
    args = parser.parse_args()
    try:
        if args.command == "capture":
            result = capture(args.serial, args.physical_id, args.out)
        elif args.command == "stage":
            result = stage(args.serial, args.zip)
        else:
            result = inventory(args.serial, args.out)
        print(json.dumps(result, indent=2))
        return 1 if result.get("failedQueries") else 0
    except (OSError, ValueError, subprocess.SubprocessError) as error:
        detail = (error.stderr or b"").decode("utf-8", errors="replace") if isinstance(error, subprocess.CalledProcessError) else str(error)
        print(f"ADB operation failed: {detail.strip()}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
