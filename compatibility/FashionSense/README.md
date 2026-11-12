# Fashion Sense 7.5.0 Android compatibility

Fashion Sense is by **PeacefulEnd**. This directory contains a minimal compatibility patch and the upstream GNU GPL version 3 license. It contains no game assemblies, mod binaries or artwork.

Upstream source: [Floogen/FashionSense](https://github.com/Floogen/FashionSense), commit [39e9e9e13c9f9a78416172e73a229cf8703ddb59](https://github.com/Floogen/FashionSense/tree/39e9e9e13c9f9a78416172e73a229cf8703ddb59), the source for version 7.5.0. The original [CharacterCustomizationPatch.cs](https://github.com/Floogen/FashionSense/blob/39e9e9e13c9f9a78416172e73a229cf8703ddb59/FashionSense/Framework/Patches/Menus/CharacterCustomizationPatch.cs) retains its original authorship. The compatibility modification is dated 2026-11-12. Preserve the upstream license and attribution with derivative source or packages.

## Exact change

On Android 1.6.15.3, `CharacterCustomization` wraps `MobileCustomizer`. It has no desktop `ResetComponents` method or desktop selection-button fields. The original first Harmony call receives a null target, and Fashion Sense returns from startup before finishing initialization.

The guard checks `AccessTools.TypeByName("StardewValley.Menus.MobileCustomizer")`. When that type exists, only `CharacterCustomizationPatch.Apply` returns before installing its desktop hooks. Otherwise all original desktop hooks execute. No platform name or assembly version is substituted.

The optional new-farmer **start with Hand Mirror** checkbox is omitted on this mobile UI, along with that group's initial null custom-hair metadata assignment. The mirror shop, tool, menu, appearance rendering and later startup code receive no changes from this patch. Mirror use, bundled presets, touch recoloring, world/tool/inventory rendering, native save and cold reload have passed on ThorTest. Physical controller acceptance and the combined setup remain pending.

The source-equivalent patch also sets the manifest version to `7.5.0-android.1`. The upstream source manifest uses a release-time version placeholder. The binary patcher makes the same version change in the original release manifest.

## Reproduce the patched original package

Use the existing [FashionSenseAndroidPatch tool](../../tools/FashionSenseAndroidPatch/Program.cs), which modifies the original release in memory using Mono.Cecil 0.11.6. This preserves the original assembly for JunimoGate's existing compatibility rewrites. It does not rebuild all upstream source against the Android API.

Input archive:

- Filename: `Fashion Sense 7.5.0-9969-7-5-0-1773351944.zip`
- Size: 318146 bytes
- SHA256: `a12fe600e338a5ebabcccfd5a3f585dbd0346fa4357f4e4e5aaa6a23a5e8e6d0`
- Original manifest ID/version: `PeacefulEnd.FashionSense` / `7.5.0`

From this repository root, with the local .NET 9 SDK available:

```powershell
$env:DOTNET_ROOT = (Resolve-Path '.local/dotnet').Path
$env:DOTNET_CLI_HOME = (Resolve-Path '.local/dotnet-home').Path
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_GENERATE_ASPNET_CERTIFICATE = 'false'
$originalZip = 'C:\path\Fashion Sense 7.5.0-9969-7-5-0-1773351944.zip'
$outputZip = '.local/visual-mod-sources/FS-research/FashionSense-7.5.0-android.1.zip'
& .local/dotnet/dotnet.exe run --project tools/FashionSenseAndroidPatch -c Release -- $originalZip $outputZip
Get-FileHash -Algorithm SHA256 -LiteralPath $outputZip
```

Replace the input path with the downloaded original archive and choose an unused output path. The tool rejects other input hashes and refuses to overwrite an existing output or the original. It changes only the target method body and manifest version; all other archive entry contents are copied.

Current candidate SHA256: `09f5ab859d8dff44d99eede1c3f6a9c76f6cde40ac461cf6808103676348642e`. This identifies the current candidate, not a claim that runtime acceptance has passed.

## Source equivalent

`CharacterCustomization-mobile-guard.patch` applies to the pinned upstream source tree. Check or apply it from that tree with:

```powershell
git apply --check C:/path/to/compatibility/FashionSense/CharacterCustomization-mobile-guard.patch
git apply C:/path/to/compatibility/FashionSense/CharacterCustomization-mobile-guard.patch
```

The source patch documents the same guard and package version. Applying it and performing a full source rebuild is a separate path from patching the pinned original release. A successful source patch application does not establish Android build or runtime compatibility.

Target environment: official Stardew Valley Android 1.6.15.3, branded JunimoGate 0.3.1-thor.2 / SMAPI 4.5.2-junimogate.41, and ThorDualScreen 0.5.2-android.3. This change addresses the observed startup binding only. Full initialization and mirror/preset gameplay checks passed on ThorTest. See docs/android/visual-mod-acceptance.md for the current acceptance status.
