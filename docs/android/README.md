# Thor Dual Screen on Android

Temporary item-switch row 0.5.2-android.3: see its [acceptance report](item-switch-acceptance.md)
and [execution checkpoint](item-switch-execution-checkpoint.md) for current status.
It retains the existing personal branded 0.3.1-thor.2 host and direct launch icon.
The previous [toolbar-hiding release](toolbar-acceptance.md), 0.5.2-android.2,
is preserved for rollback. The baseline records below describe 0.5.2-android.1.

This branch targets the official Play 1.6.15.3 ARM64 game through unchanged
publisher JunimoGate 0.3.1 and SMAPI 4.5.2-junimogate.41 on AYN Thor Android 13.
The mod reuses the upstream UI and the running game's artwork and fonts.
No game assemblies or assets are distributed.

The accepted Release is installed in the one-mod ThorRelease group.
Integration, fixture-free save/reload, matched performance, the 30-minute mixed
soak and user-confirmed physical controls passed. See [Release acceptance](release-acceptance.md)
for the complete result and measured limitations.
See [acceptance](acceptance.md), [compatibility](compatibility.json) and the
[resumable checkpoint](execution-state.md) for the current verdict.

Use the packaged [installation and rollback notes](../../android/INSTALL.md).
The game launches through JunimoGate's selected mod group. The ordinary Play
icon and its real saves remain separate. No original save has been imported.

See [build instructions](build.md) for the isolated Windows toolchain,
private reference extraction, Release build and archive allowlist.
The guarded test fixture is separate and must be absent from a release group.

JunimoGate's Save Export produced a zero-byte file in the tested version.
Disposable Save Import and cold reload passed using an independently verified
ZIP. Preserve independently verified backups; an export button succeeding is
not evidence that a backup is usable.
