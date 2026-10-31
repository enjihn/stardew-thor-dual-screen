# Thor Dual Screen on Android

This branch targets the official Play 1.6.15.3 ARM64 game through unchanged
publisher JunimoGate 0.3.1 and SMAPI 4.5.2-junimogate.41 on AYN Thor Android 13.
The mod reuses the upstream UI and the running game's artwork and fonts.
No game assemblies or assets are distributed.

The Release candidate is installed in the one-mod ThorRelease group.
Integration, fixture-free save/reload and matched performance passed. Physical
controls and final soak review remain open. See [current Release acceptance](release-acceptance.md).
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
