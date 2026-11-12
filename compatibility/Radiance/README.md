# Radiance 2.4.8 Android compatibility

SDV-Radiance is by **phuicmt**. This compatibility change prevents discovery of
an optional integration from aborting startup because an unrelated loaded
assembly has an unavailable dependency. Runtime acceptance is pending. See
[visual-mod-acceptance.md](../../docs/android/visual-mod-acceptance.md) for the
current device results.

The upstream baseline is [v2.4.8](https://github.com/PHUICMT/SDV-Radiance/tree/00a861891038edaf2b74331acd8d69281c59bb76),
commit `00a861891038edaf2b74331acd8d69281c59bb76`. Its MIT license and original
attribution are preserved in [LICENSE.upstream](LICENSE.upstream). The new
helper, patcher and compatibility changes use this repository's existing
GPL-3.0 license, copied in [LICENSE](LICENSE). The upstream portions retain
their original license. This directory contains no game assemblies or artwork.

The original package failed during `ScaleUpArt.Find` with a missing
`Pathoschild.Http.Client, Version=4.4.2.0` dependency. The optional
`AccessTools.TypeByName` lookup enumerates types across loaded assemblies;
that unrelated load failure escapes before GMCM registration. The following
DynamicShader lookup has the same discovery pattern.

Two optional named-type call operands are replaced:

- `SDVRadiance.ScaleUpArt.Find`: lookup of
  `ScaleUpUnofficial.HarmonyPatches`.
- `SDVRadiance.ShadowSuppression.LeanDynamicShaderCanopies`: lookup of
  `DynamicShader.Core.Rendering.CanopyLight.TreeCanopyLightDrawer`.

[ExactTypeLookup.cs](ExactTypeLookup.cs) asks each loaded assembly for the exact
type name with `Assembly.GetType`, without enumerating every type. It continues
past file, assembly or type loading failures. A present integration can still
be found; an absent integration returns null. This does not disable optional
integrations globally or add the unrelated HTTP library.

After this change, the first candidate reached GMCM and water labels, then
failed while inspecting an unrelated type's unresolved base class. Version
`2.4.8-android.2` also routes these three discovery-only `Type.IsAssignableFrom`
calls through the helper:

- `LocationDrawHook.LocationTypesThatDrawThemselves` (the iterator's `MoveNext`
  method in the binary).
- `ShadowSuppression.PatchSelfDrawnCharacters`.
- `HarmonyPatcher.HoldCharactersWhileFrozen`.

The helper preserves successful assignability results and returns false for
an unresolved candidate, catching the same four loader exception kinds. This
lets discovery skip that type while preserving valid locations and characters.
The first candidate stopped before farm gameplay; the revised candidate still
requires its own live acceptance.

The packaged binary calls the separate `Radiance.ThorCompat.dll`, built for
.NET 6. The patcher uses Mono.Cecil 0.11.6 and runs on .NET 9. It ships no
additional Harmony library. It copies all other original archive entries,
including compiled shaders, artwork, labels and upstream notices, changing
only `SDV-Radiance.dll`, manifest version `2.4.8-android.2`, and adding the
helper DLL plus its GPL notice `ThorCompatibility-LICENSE`. It does not
rebuild the upstream renderer.

Build from this repository root using the .NET 9 SDK. The third CLI argument
is a private directory containing the original assembly's game, SMAPI and
Harmony references for Cecil resolution; use local references, do not publish
those assemblies. Replace the paths below and choose an unused output ZIP.

```powershell
$radianceOriginalZip = 'C:/path/to/original-Radiance-2.4.8.zip'
$radianceOutputZip = 'C:/path/to/new-Radiance-2.4.8-android.2.zip'
$radiancePrivateRefs = 'C:/path/to/private/reference-assemblies'
dotnet build tools/RadianceAndroidPatch/RadianceAndroidPatch.csproj -c Release
dotnet tools/RadianceAndroidPatch/bin/Release/net9.0/RadianceAndroidPatch.dll self-test
dotnet tools/RadianceAndroidPatch/bin/Release/net9.0/RadianceAndroidPatch.dll $radianceOriginalZip $radianceOutputZip $radiancePrivateRefs
Get-FileHash -LiteralPath $radianceOutputZip -Algorithm SHA256
```

The patcher accepts exactly three packaging arguments in that order:
`ORIGINAL_ZIP NEW_ZIP PRIVATE_REFERENCE_DIRECTORY`. It rejects an existing
output, an output equal to the original, or an input with a different hash.

- Pinned original ZIP SHA256:
  `7e1e29c2ddc64ca0e13e6243dcac1b22fb62b6be2c2e342f29bc846e3420e3d0`.
- Original manifest: `phuicmt.SDVRadiance`, version `2.4.8`.
- Current candidate: `SDV-Radiance-2.4.8-android.2-licensed.zip`.
- Current candidate ZIP SHA256:
  `15680f5f7e90c69a090099e965e02b9b892293e50ec6c66d996bcf33eb163b31`.

The candidate hash identifies the produced artifact. It is not a guarantee
that another SDK or packaging environment produces identical ZIP bytes.
The lookup self-test covers present and absent types, an unrelated assembly
with a missing dependency, and refusal to enumerate types. It also checks
valid, unrelated and throwing assignability cases. Its pass does not establish
successful game initialization or rendering.

[Optional-integration-exact-lookup.patch](Optional-integration-exact-lookup.patch)
is the source equivalent for the pinned upstream tree. It adds the same helper
implementation under `src/Integrations/ExactTypeLookup.cs`, retaining namespace
`Radiance.ThorCompat`, and adds explicit System, Collections and IO imports
because upstream does not enable implicit usings. The five replacement call
sites use its fully qualified name. It also changes the manifest version.

Check or apply from a separate checkout of that exact upstream revision:

```powershell
git apply --check C:/path/to/Optional-integration-exact-lookup.patch
git apply C:/path/to/Optional-integration-exact-lookup.patch
dotnet build SDV-Radiance.csproj -c Release -p:GamePath=C:/path/to/private/game-reference-directory -p:EnableModDeploy=false -p:EnableModZip=false
```

The upstream SDK compiles the added source through its existing default glob.
A full source rebuild embeds the helper class in the upstream assembly instead
of using the packaged helper DLL. It is a source-equivalent alternative, not
the binary production path above; its build/runtime compatibility is unverified.
Keep the added helper's license with that derivative source. Do not redistribute
game or SMAPI reference assemblies with the source or patch.

Target environment: official Android Stardew Valley 1.6.15.3, branded
JunimoGate 0.3.1-thor.2 / SMAPI 4.5.2-junimogate.41, and ThorDualScreen
0.5.2-android.3. This patch addresses the observed optional-discovery startup
failure only. Startup, GMCM access, top-screen effects, unchanged lower Aim,
and the 60 FPS requirement still need their device acceptance results.
