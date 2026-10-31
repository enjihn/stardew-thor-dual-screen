# Reproduce the Android Release library

Pinned source and runtime identities are in compatibility.json. This build
uses the actual Android game assemblies, never desktop or Cinderbox references.

## Inputs

- .NET SDK 9.0.118 for Windows x64, Android workload 35.0.7/9.0.100.
- Android API 35 revision 2, build-tools 35.0.0, command-line tools 19.0.
- Temurin JDK 17.0.16+8, Python 3.11, ADB 36.0.2.
- Official Play Stardew Valley 1.6.15.3/code245 ARM64 installed by its owner.
- Publisher JunimoGate 0.3.1/code11, source commit
  029eafed917ae853d6c0a59330ee273524168296.

Keep tools under ignored .local/dotnet, .local/android-sdk and
.local/jdk/jdk-17.0.16+8. Set DOTNET_ROOT to that dotnet directory,
DOTNET_CLI_HOME to .local/dotnet-home, DOTNET_CLI_TELEMETRY_OPTOUT=1 and
DOTNET_GENERATE_ASPNET_CERTIFICATE=false. Install the pinned Android workload
with that SDK. Use one Windows ADB server for the USB device.

Query the selected device's pm path com.chucklefish.stardewvalley and pull
every returned APK into a private directory. Build the pinned host's
 tools/JunimoGate.GameInspector project with the same SDK. Run its
extract-assemblies OUTPUT APK... command against a fresh private directory.
Extract the verified host APK's managed assemblies separately. Place exactly
one of each of the following in the flat .local/refs/mobile directory:

- StardewValley.dll and StardewValley.GameData.dll from the Play APKs.
- StardewModdingAPI.dll, MonoGame.Framework.dll and xTile.dll from the matching host.

Do not mix identities. The build record includes each reference SHA256.
Never commit or distribute these private references, APKs or game content.

## Build and check

Run from the repository root using the recorded source commit:

```powershell
python tools/build_android.py --refs .local/refs/mobile --configuration Release --build-id YOUR_RECORDED_BUILD_ID
python -m unittest discover -s tests -p 'test_*.py'
.local/dotnet/dotnet.exe run --project tests/DualScreen.Android.Tests -c Release
```

For a clean build, first run the same local dotnet against
android/DualScreen.Android.csproj with `clean -c Release` and the same
MobileReferenceDir, AndroidSdkDirectory and JavaSdkDirectory properties used
by tools/build_android.py. The helper accepts explicit paths for each tool.

Output is artifacts/private/builds/BUILD_ID/ThorDualScreen-Android.zip, plus
build.json and SHA256.txt. The archive has one ThorDualScreen root containing
only DualScreen.dll, manifest.json, LICENSE, ATTRIBUTION.md and INSTALL.md.
Its timestamps are normalized. AndroidGenerateResourceDesigner is disabled because
the mod declares no Android resources; the SDK designer-assembly feature stays
enabled for the host references. DebugType=none excludes machine-specific PDB
metadata from the shipping DLL. Rebuild with identical inputs and build ID,
compare SHA256, and inspect the archive before staging. Fixture binaries,
runtime DLLs, APKs, saves, private paths and desktop runtime metadata are excluded.

`python tools/thor_adb.py stage --serial SERIAL --zip ZIP` copies and hashes
the file on the device. It does not import or enable it. Use the launcher UI
as described in INSTALL.md, restart the host and check the actual build ID
in the startup log. Final acceptance requires the physical-device matrix.
