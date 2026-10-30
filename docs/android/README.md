# Android execution and resumption

This branch is an incomplete port checkpoint, not a mobile release. USB authorization blocked the installed-game inventory and every device gate. See `execution-state.md`, `compatibility.json`, and `acceptance.md` for the actual state.

The original upstream desktop project and artwork-rendering source remain unchanged. No application or save on the device has been modified. There is no mod ZIP or installed mobile build to roll back yet.

## Build tools prepared

Tools are isolated under ignored `.local/`. The verified Windows toolchain is .NET SDK 9.0.118, Android workload 35.0.7/9.0.100, Temurin JDK 17.0.16+8, Android API 35 revision 2, build-tools 35.0.0, and command-line tools 19.0. Existing Windows ADB remains the USB owner.

The SDK ZIP came from [Microsoft's 9.0.118 distribution](https://builds.dotnet.microsoft.com/dotnet/Sdk/9.0.118/dotnet-sdk-9.0.118-win-x64.zip), checked against the SHA512 in [Microsoft's release metadata](https://builds.dotnet.microsoft.com/dotnet/release-metadata/9.0/releases.json):

```text
63e3b368409c08d7e8ae984142e911f70373d8a7a23cd37393fc443144674e56855888247c47e97eaf60af3d028b31d569ed7e0e11b10729a1dc5e1527c2e4ae
```

The [Temurin 17.0.16+8 Windows JDK ZIP](https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.16%2B8/OpenJDK17U-jdk_x64_windows_hotspot_17.0.16_8.zip) matched its publisher SHA256:

```text
8c7cfff78a55c56ebaf470ed6a89c6466b47d8274bdabdda997d7507c20325c5
```

For this prepared checkout, select the local tools in PowerShell before builds:

```powershell
$env:DOTNET_ROOT = (Resolve-Path .local/dotnet).Path
$env:DOTNET_CLI_HOME = (Resolve-Path .local/dotnet-home).Path
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_GENERATE_ASPNET_CERTIFICATE = 'false'
$sdkPath = (Resolve-Path .local/android-sdk).Path
$jdkPath = (Resolve-Path '.local/jdk/jdk-17.0.16+8').Path
```

Use `.local/dotnet/dotnet.exe` with explicit `AndroidSdkDirectory` and `JavaSdkDirectory` MSBuild properties. The mobile project is intentionally not created until references can be derived from the actual installed Play game.

Host source is pinned to JunimoGate v0.3.1, commit `029eafed917ae853d6c0a59330ee273524168296`. Its `tools/JunimoGate.GameInspector` builds using the local SDK without Android game references. Use that tool for the installed game's assembly stores once USB is authorized.

## Device helper

```text
python tools/thor_adb.py inventory --serial SERIAL --out artifacts/private/device-inventory.json
python tools/thor_adb.py capture --serial SERIAL --physical-id PHYSICAL_ID --out artifacts/private/bottom.png
python tools/thor_adb.py stage --serial SERIAL --zip PATH_TO_MOD_ZIP
python -m unittest discover -s tests -p "test_*.py"
```

Inventory is private and may include device identifiers. Its command errors remain explicit, and a failed query produces a nonzero exit. Screenshot output is binary and refuses to overwrite existing captures. Staging checks the destination SHA256 and explicitly reports that import and selection have not occurred. Discover physical screenshot IDs separately from logical input IDs.

## Host routes found in source, still requiring device verification

- `JunimoGate.GameHost.GameHostBridge.GetActivity()` exposes the active game Activity.
- Create framework `Presentation` and `ImageView` objects on the UI thread, using the Presentation context. The actual Release-host touch/event-peer probe remains mandatory.
- Mod import: Mods, Import Mod ZIP, choose the ZIP, confirm import. Default import is library-only, then use Add to group and select/enable the revision. Restart the game process after changing selection.
- Diagnostic export: Logs, More, Export all, Export, then select a local document destination. The ZIP includes current/previous game and SMAPI logs, with bounded source sizes.

These routes were read in the pinned source, not exercised on this device. Installation, mod selection, save export/import, and rollback instructions must be completed from actual device observations before release.

## Preservation and eventual rollback

Keep the Play installation and real saves intact. Prefer a new disposable hosted farm when original saves cannot be exported through legitimate interfaces. Never clear data, uninstall to resolve a signing mismatch, root, or bypass app-private storage.

Once a mobile release is verified, rollback should disable the mod or select the previous verified revision and restart the hosted game. This route is currently untested. The standard Play icon is separate from the modded host launch route.
