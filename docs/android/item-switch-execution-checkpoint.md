# Item-switch overlay execution checkpoint

Status: device acceptance complete on 2026-11-12. Source commit/push and local
artifact metadata are the final delivery actions. Branch android/transient-item-toolbar
starts at ce6cf1933291fed38d061ca733d2ef7714c5c2f6. Specification:
../superpowers/plans/2026-11-12-transient-item-toolbar.md.

Root owned device and coupled source throughout. The dedicated clean checkout
was reused on the requested isolated branch, retaining private tools/references.
No upstream changes, host rebuild, launcher replacement, game-asset changes,
real-save writes, zoom/resolution changes, app removal or app-data clearing.

Accepted Release: 0.5.2-android.3, item-switch-android3-03.
ZIP SHA256: 6ff42e969a9a3fb27d95163b47b1ed1db4b720efe0cdec7a28e24fdd2559421e.
Thor57ae2a4f: Android13/API33, Play1.6.15.3/code245, branded JunimoGate
0.3.1-thor.2/code12, SMAPI4.5.2-junimogate.41, Harmony2.4.2-junimogate.64.
ThorItemSwitchRelease is selected and has exactly one enabled mod, no fixture.
Cold direct launch confirmed build03 and companion display4. ThorTest453080632
is the only farm used. The game is left running for the owner.

Checks: 23/23 C# and10/10 Python passed on the final state/input logic. Build03
adds only source-backed native background fills; final device rendering and
five-file packaging checks passed. One whole-branch review found stale Bag
presentation metadata; regression failed22/23, then passed23/23 after the fix.
No repeated review was needed. The owner requested solid backgrounds after
observing build02 and confirmed size/timing were already good. Build03 uses
native fills behind the same outlines, dimensions, spacing and timing.

Device gates passed: native/Aim selection, every non-Bag tab, same/repeated and
nonmanual changes, Bag/rapid transitions, world touch-through, stable camera,
menu/dialogue/warp/cutscene/pause reset, actual upload failure/fallback/recovery,
cold ShowItemSwitchOverlay=false and full-bar config behavior, and cold rollback
to retained android.2. The final fixture-free build received owner physical
bumper/trigger/movement/tool-use and visual acceptance: "seems good".
See item-switch-acceptance.md and private artifacts/private/item-switch.

Preserved: ThorToolbarRelease with toolbar-android2-06 and accepted .2 ZIP,
SHA256 4f3c4062ff90b52a28643250507446b736fe8b78d1ed459736e65ec08334a8d3.
ThorItemSwitchTest remains separate with guarded fixture0.23. No fixture is in
release. Tests changed only disposable test configuration/state. Final new-row
setting is restored to true and confirmed by fixture-free rendering.

Delivery files are under the workspace outputs directory, outside the repository.
Push only source to origin/enjihn on android/transient-item-toolbar. No upstream
merge or proprietary files. Record commit/push outcome in local delivery JSON.
If interrupted before push, inspect git status and remote branch, then finish
only those delivery actions. No gameplay gate needs repeating unless code changes.
