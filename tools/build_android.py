#!/usr/bin/env python3
"""Build the mobile library and package an explicit, mod-owned allowlist."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import subprocess
import zipfile

ROOT = Path(__file__).resolve().parents[1]
REFERENCE_NAMES = ('StardewValley.dll', 'StardewValley.GameData.dll',
                   'StardewModdingAPI.dll', 'MonoGame.Framework.dll', 'xTile.dll')


def validate_refs(refs):
    result = {}
    for name in REFERENCE_NAMES:
        matches = list(refs.rglob(name))
        if not matches:
            raise ValueError(f'Missing reference: {name}')
        if len(matches) != 1 or matches[0].parent != refs:
            raise ValueError(f'Ambiguous reference: {name}; supply one flat reference directory')
        if matches[0].read_bytes()[:2] != b'MZ':
            raise ValueError(f'Not a managed PE reference: {name}')
        result[name] = hashlib.sha256(matches[0].read_bytes()).hexdigest()
    return result


def package_mod(dll, manifest, out):
    if manifest.get('EntryDll') != 'DualScreen.dll' or dll.name != 'DualScreen.dll':
        raise ValueError('EntryDll must be the unique mod-owned DualScreen.dll')
    payloads = {'ThorDualScreen/manifest.json': (json.dumps(manifest, indent=2) + '\n').encode(),
                'ThorDualScreen/DualScreen.dll': dll.read_bytes(),
                'ThorDualScreen/LICENSE': (ROOT / 'LICENSE').read_bytes(),
                'ThorDualScreen/ATTRIBUTION.md': (ROOT / 'android/ATTRIBUTION.md').read_bytes(),
                'ThorDualScreen/INSTALL.md': (ROOT / 'android/INSTALL.md').read_bytes()}
    with zipfile.ZipFile(out, 'w', zipfile.ZIP_DEFLATED) as z:
        for name, data in payloads.items():
            entry = zipfile.ZipInfo(name, (2026, 1, 1, 0, 0, 0))
            entry.compress_type = zipfile.ZIP_DEFLATED
            z.writestr(entry, data)
    with zipfile.ZipFile(out) as z:
        if set(z.namelist()) != set(payloads) or len(z.namelist()) != len(payloads) or z.testzip():
            raise ValueError('Archive allowlist validation failed')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--refs', required=True, type=Path)
    parser.add_argument('--configuration', choices=['Release'], default='Release')
    parser.add_argument('--build-id', required=True)
    parser.add_argument('--dotnet', type=Path, default=ROOT / '.local/dotnet/dotnet.exe')
    parser.add_argument('--android-sdk', type=Path, default=ROOT / '.local/android-sdk')
    parser.add_argument('--jdk', type=Path, default=ROOT / '.local/jdk/jdk-17.0.16+8')
    args = parser.parse_args()
    if not re.fullmatch(r'[A-Za-z0-9._-]{1,80}', args.build_id):
        parser.error('Build ID must contain 1 to 80 ASCII letters, digits, dots, underscores or hyphens')
    refs = args.refs.resolve()
    hashes = validate_refs(refs)
    env = os.environ.copy()
    env.update(DOTNET_ROOT=str(args.dotnet.resolve().parent),
               DOTNET_CLI_HOME=str(ROOT / '.local/dotnet-home'),
               DOTNET_CLI_TELEMETRY_OPTOUT='1', DOTNET_GENERATE_ASPNET_CERTIFICATE='false')
    project = ROOT / 'android/DualScreen.Android.csproj'
    properties = [f'-p:Configuration={args.configuration}', f'-p:MobileReferenceDir={refs}',
                  f'-p:ThorBuildId={args.build_id}', f'-p:AndroidSdkDirectory={args.android_sdk.resolve()}',
                  f'-p:JavaSdkDirectory={args.jdk.resolve()}']
    subprocess.run([str(args.dotnet.resolve()), 'build', str(project), *properties, '--nologo'],
                   cwd=ROOT, env=env, check=True, timeout=300)
    target = subprocess.run([str(args.dotnet.resolve()), 'msbuild', str(project), *properties,
                             '-getProperty:TargetPath', '-nologo'], cwd=ROOT, env=env, check=True,
                            capture_output=True, text=True, timeout=60).stdout.strip()
    dll = Path(target)
    if not dll.is_file() or not dll.resolve().is_relative_to(ROOT / 'android/bin/Release'):
        raise ValueError(f'Unexpected build TargetPath: {target}')
    manifest = json.loads((ROOT / 'android/manifest.json').read_text())
    manifest['Description'] += f' Build {args.build_id}.'
    out_dir = ROOT / 'artifacts/private/builds' / args.build_id
    out_dir.mkdir(parents=True, exist_ok=True)
    out = out_dir / 'ThorDualScreen-Android.zip'
    package_mod(dll, manifest, out)
    digest = hashlib.sha256(out.read_bytes()).hexdigest()
    record = {'buildId': args.build_id, 'configuration': args.configuration,
              'zip': str(out), 'sha256': digest, 'referenceSha256': hashes}
    (out_dir / 'build.json').write_text(json.dumps(record, indent=2) + '\n')
    (out_dir / 'SHA256.txt').write_text(f'{digest}  {out.name}\n')
    print(json.dumps(record, indent=2))


if __name__ == '__main__':
    main()
