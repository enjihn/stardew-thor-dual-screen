"""Send one bounded request to the separately installed disposable-farm fixture."""
import argparse
import json
from pathlib import Path
import re
import subprocess
import time
from thor_adb import adb

REMOTE = '/sdcard/Android/data/org.junimogate.app/files/ThorFixture'


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--serial', required=True)
    parser.add_argument('--request', required=True, type=Path)
    parser.add_argument('--out', required=True, type=Path)
    args = parser.parse_args()
    request = json.loads(args.request.read_text(encoding='utf-8-sig'))
    ident = request['id']
    if not re.fullmatch(r'[A-Za-z0-9_-]{1,60}', ident):
        raise ValueError('Invalid request ID')
    if args.out.exists():
        raise ValueError('Choose a new evidence output path')
    adb(args.serial, 'push', str(args.request.resolve()), REMOTE + '/request.incoming')
    adb(args.serial, 'shell', 'mv', REMOTE + '/request.incoming', REMOTE + '/request.json')
    deadline = time.monotonic() + 20
    while time.monotonic() < deadline:
        try:
            data = adb(args.serial, 'shell', 'cat', f'{REMOTE}/{ident}.json')
            result = json.loads(data)
            args.out.parent.mkdir(parents=True, exist_ok=True)
            args.out.write_bytes(data)
            print(json.dumps(result, indent=2))
            return
        except (subprocess.CalledProcessError, json.JSONDecodeError):
            time.sleep(0.25)
    raise TimeoutError('Fixture gave no complete response in 20 seconds')


if __name__ == '__main__':
    main()
