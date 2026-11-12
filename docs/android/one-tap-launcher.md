# Native direct launcher checkpoint

Verified on the connected AYN Thor on 2026-11-12. Tap the green dual-screen/star **Stardew Dual Screen** home tile. It enters the game through its normal loading screens without visiting the mod manager. An existing session resumes. It does not automatically load a farm.

The rejected accessibility helper is disabled at the Android package level and its service is off. Its source is archived privately. The native launcher uses no accessibility automation.

## Installed compatibility

| Component | Version |
|---|---|
| Official Play game | com.chucklefish.stardewvalley, 1.6.15.3, code 245, ARM64 |
| Personal host | org.junimogate.thor, 0.3.1-thor.1, code 11, Release |
| Host source base | JunimoGate v0.3.1, 029eafed917ae853d6c0a59330ee273524168296 |
| SMAPI | 4.5.2-junimogate.41, submodule 9471959851a75dfa1f2648016b78fc171a5a1999 |
| Mod | 0.5.2-android.1, build 22cf243112b710381db6db8b89dc01882958a67f |
| Device / launcher | AYN Thor, Android 13 / API 33, Cocoon 3.11 |
| Selected group | ThorRelease, one enabled production mod |

APK SHA256: `cd0efef7131c7fab12e0862196815db1adfd64ced6e87cad63fa22bbf74f249a`

Unchanged mod ZIP SHA256: `250ccf42543906951ef68cc33305e25991c4815844c8a4f7d060595b34cd322a`

## Acceptance

| Check | Observed result |
|---|---|
| Release build | Succeeded, zero errors, one nullable binding warning at ActivityOptions.MakeBasic. |
| Host tests | 256/256 plus RuntimePack self-test passed. These are host checks, separate from Android acceptance below. |
| Package gate | Every artifact check passed: personal v2/v3 signing, non-debuggable Release, ARM64, package identity, isolated game process, commercial-payload exclusions and manager shortcut task isolation. |
| Source portability | Patch applied cleanly to a clean recursive checkout of the pinned public host and SMAPI commits. |
| Cold home-icon launch | Cocoon launched DirectLaunchActivity at 14:05:44.817 device time. Game dispatch at 14:05:45.551 with no MainActivity visit. Original intro and dual-screen title rendered. This interval is dispatch latency, not time to playable title. |
| Copied save | Native import added one disposable farm. All four imported file hashes matched the source. Farm loaded with gameplay on top and Today/Bag companion panels below. |
| Affected input | Lower Load selected the copied farm; Bag switched panels; top inventory opened. Original mod and artwork retained. |
| Warm resume | After fixing Cocoon's per-app display setting, one home-icon press resumed the same game process on display 0, with unchanged inventory and lower Bag panel. |
| Manager shortcut safety | Invoked with NEW_TASK and CLEAR_TASK flags. Game process and inventory survived; manager routed back to the game on display 0. |
| Co-op availability | Re-enabled the documented mobile feature in this installation. Lower Co-op opens native Host/Join on top. No peer connection was tested in this launcher follow-up. |
| Preservation | Original packages remain installed. The publisher host's disposable source-save hashes were unchanged after testing. No real save was imported, overwritten or removed. |

In Cocoon, edit this app and enable **Launch on main**. This is saved on the tested Thor. With it off, Cocoon moved the running game to the lower display on warm launch. The setting corrected that failed check without runtime changes. Do not run both hosts simultaneously, since each can create companion windows.

The prior physical control confirmation and full mod acceptance remain in [release-acceptance.md](release-acceptance.md). This follow-up used physical-device ADB input and captures; it did not repeat or relabel physical button tests.

## Delivery and resume

Source branch: `android/one-tap-launcher` in `enjihn/stardew-thor-dual-screen`. Host local branch: `thor/direct-launch`. The source patch and build, installation, update and rollback notes are in [host/direct-launch](../../host/direct-launch/README.md).

Private evidence remains in ignored `artifacts/private/direct-launch/`: cold-launch logs, import hashes, farm/inventory captures, warm resume, manager safety and Co-op menu captures. Build/package reports are in the sibling host's ignored `artifacts/android/`. Final binaries are local delivery artifacts, not committed binaries.

All launcher gates above passed. Source contains no game binaries, saves, signing secrets or raw device logs. Only a disposable verified copy exists in the new host. Real saves remain in their original installation and are not synchronized automatically.
