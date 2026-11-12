# Toolbar hiding execution checkpoint

Updated 2026-11-12. Status: PASS. All mandatory implementation and device gates
passed. The owner confirmed physical controls on the final fixture-free build.
The exact test scope, results, compatibility and rollback are in
[toolbar acceptance](toolbar-acceptance.md).

## Delivered state

Branch android/hide-top-toolbar, base ec3ac6a. Source delivery commit is HEAD
on this branch. Target remote is the personal fork origin:
https://github.com/enjihn/stardew-thor-dual-screen.git. No upstream merge or PR.
If interrupted before source delivery, push this branch to origin; do not
restart acceptance or alter the currently running game.

Release 0.5.2-android.2, build toolbar-android2-06. ZIP:
artifacts/private/builds/toolbar-android2-06/ThorDualScreen-Android.zip.
SHA256: 4f3c4062ff90b52a28643250507446b736fe8b78d1ed459736e65ec08334a8d3.
The same bytes are staged in device Downloads/Toolbar06.zip and copied into
the workspace outputs as ThorDualScreen-Android-0.5.2-android.2.zip.

Thor serial57ae2a4f; top logical0, companion logical4. Selected group:
ThorToolbarRelease, exactly one enabled mod. Custom direct-launch entry opens
this group. No fixture loaded. Game left running on the disposable ThorTest
farm453080632. Do not stop a later live game without checking user activity.

Runtime: Play1.6.15.3/code245 ARM64, existing branded host0.3.1-thor.2/code12,
SMAPI4.5.2-junimogate.41, Harmony2.4.2-junimogate.64, Android13/API33.
No host rebuild or original app replacement. All three original Play save
files match verified backup hashes. Only disposable ThorTest was manipulated.
Its seeded tools persisted through native overnight save and cold reload.

## Verification and rollback

15/15 C# checks,10/10 Python checks, Release compilation, ZIP CRC/hash and exact
five-file allowlist pass. Existing obsolete getItemCount warning remains.
No Harmony or game DLL is packaged. Controlled upload failure, missing eligible
display, cold configfalse, previous-version rollback, camera/world input,
seven tabs, touch boundaries, menus/dialogue, location and pause gates passed.
Owner reply on final physical movement, bumper/trigger and touch/tool/menu
check: "everything seems to work". No mandatory gate is untested or blocked.

Accepted ThorRelease with android.1 and its original ZIP are retained.
Select it and restart the hosted game to roll back, or set
HideToolbarWithCompanion=false in the new group's config and restart.
Never clear app data, uninstall an app, or overwrite real saves for rollback.

ThorToolbarTest retains candidate06 plus guarded fixture0.22 for reproducible
diagnostics. Multiple library revisions share android.2; always confirm the
exact build ID before using them. Root owns device work; independent review
and final test verification completed. No further review loop is required.

## Material findings

Build03's dead ViewTreeObserver crash was corrected. Whole-branch review found
stale epoch and callback ownership faults, fixed in05 with focused regressions.
Required row testing found Android shiftToolbar advances one slot rather than
rotating desktop rows. Android-only direct Bag slot selection in06 fixes the
observed0-to3 selection when slot13 was requested. Desktop behavior is unchanged.
All source-inspection artifacts, assemblies, assets, saves and raw evidence
remain private and ignored. Local outputs contain the ZIP, evidence archive,
acceptance and installation/rollback notes.
