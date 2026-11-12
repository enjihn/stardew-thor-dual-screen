# Temporary item-switch toolbar acceptance

Status: PASS on 2026-11-12. All mandatory gates passed, including owner physical
controls. The final release is installed and selected in ThorItemSwitchRelease.

Candidate: 0.5.2-android.3, Release build item-switch-android3-03.
ZIP SHA256: 6ff42e969a9a3fb27d95163b47b1ed1db4b720efe0cdec7a28e24fdd2559421e.
Branch android/transient-item-toolbar, based on ce6cf1933291fed38d061ca733d2ef7714c5c2f6.

Runtime: Thor 57ae2a4f, Android 13/API33 ARM64, official Play Stardew Valley
1.6.15.3/code245, personal branded JunimoGate 0.3.1-thor.2/code12,
SMAPI 4.5.2-junimogate.41, host Harmony 2.4.2-junimogate.64.
The host APK, direct-launch icon, game assets, real saves, zoom and resolution
were not changed. Tests use disposable ThorTest 453080632 only.

The mini row is passive: five 48-pixel HUD slots, four-pixel gaps, 16-pixel bottom
inset, 1500ms lifetime and 200ms fade. It observes native manual switch methods
and successful Aim selections without replacing selection behavior. Both the
active lower panel and its last committed frame must be outside Bag. Unknown
presentation, companion loss and lifecycle transitions clear the notice.

## Evidence

Private captures and observations are in artifacts/private/item-switch. Build 02
has the final state/input logic; build 03 only adds the original native background
and selected-slot fill under the existing outlines. The user observed the missing
background on 02 and confirmed its size and timing were good. Native Toolbar.draw
shows those fills are separate from the transparent outlines. Build 03 keeps all
geometry/timing and uses the existing game textures without bundling assets.

| Gate | Result and evidence |
|---|---|
| Focused automated checks |23/23 C# tests and10/10 Python checks pass. Covers manual/automatic changes, repeated and identical selections, timeout/fade, boundaries, Bag/lifecycle resets, stale epochs and rejected stale Bag commits. |
| Review correction | One independent whole-branch review found stale presented-panel metadata after a rejected Bag commit. Regression failed 22/23 before the fix, then passed 23/23. Unknown metadata now suppresses until a fresh committed frame. |
| Release/packaging | Build 03 succeeds with the existing obsolete getItemCount warning only. Exactly five owned ZIP files, CRC and manifest valid; no Harmony/game DLL or assets. final-automated.txt records checks. |
| Rendering | solid03-native and solid03-aim-actual show five opaque native slots with selected highlight and item icons at bottom center. Other HUD remains visible and the full toolbar stays hidden. |
| Native/Aim switching and tabs | tab02-* observes native switching on Today, Gifts, People, Craft, Aim and Tabs. aim02-switch and solid03-aim-actual use actual lower touch. Native calls in the fixture are not counted as physical controls. |
| Timer and same selection | repeat02-second restarts at 1499.93ms. same02b-before/after count down 1234 to 634ms when retapping the same Aim slot. today02-expired clears; exact1500/200ms fade behavior passes deterministic tests. |
| Automatic changes | auto02-change/after: assigning a different native slot without a manual switch clears the notice and does not create a new one. |
| Bag and rapid transitions | bag02-enter/native/leave and bag03-enter show immediate clearing, no trigger while Bag and no revival on exit. Four rapid Bag/Aim cycles remain cleared; rapid02-new-switch starts a fresh notice. |
| Touch-through | touch02-active/after: tapping through a visible slot at top pixel 960,1024 moves the player from tile 30,15 to 30,21, with selected slot 2 unchanged. |
| Camera | camera02-on/off have identical viewport 1212,940,1481,833 while the row appears/expires. The native toolbar remains suppressed. |
| Menus, dialogue, location | menu02-state, dialogue02-state and warp02-cleared record noticeSlot=-1 for native inventory, native dialogue and Town warp. |
| Pause/resume | pause02-resumed: notice cleared, fresh epoch 5 committed and companion available. No old notice returns. |
| Companion failure/recovery | fault02-active: real upload fails using recycled test upload destinations, companion unavailable, native toolbar visible, mini cleared. fault02-recovered: fresh valid presentation, native toolbar hidden, old mini stays absent. A new manual switch displays it again. |
| HideToolbarWithCompanion=false | config02-hide-off-switch: native toolbar visible, mini suppressed. Restored true. |
| Cutscene reset | event03-active records eventUp=true and noticeSlot=-1 during a native 15-second event. event03-ended remains cleared. |
| Cold config rollback | config03-cold/cold-switch show ShowItemSwitchOverlay=false after a cold DirectLaunch restart; manual native switching does not create a row. Restored true before the final group launch. |
| Final03 failure/recovery | fault03-active/recovered repeats the real upload-failure gate on the final solid-background build. Native toolbar returns during failure, row stays cleared, fresh presentation hides native toolbar, and a new switch renders the solid row. |
| Previous-version rollback | rollback03-logcat confirms exactly one mod, accepted toolbar-android2-06/game1.6.15.3. ThorTest loads; Aim selection works with no mini row. The retained android.2 ZIP hash matches the accepted hash. |
| Final selection | final03-selected shows ThorItemSwitchRelease with exactly one enabled mod. A cold DirectLaunchActivity launch logs Loaded 1 mods, item-switch-android3-03, game1.6.15.3 and companion display4. final03-switchb shows the solid mini row after an Aim touch, without the fixture. |
| Physical controls | Owner replied "seems good" after testing both bumpers/triggers on Aim or Today, Bag suppression, selection visibility, movement and tool use on the final fixture-free build. Exact question/reply are recorded in physical03-confirmation.json. |

The final group is the selected group used by the existing direct-launch icon.
No host or launcher rebuild was required. No mandatory gate is outstanding.
Source is retained on android/transient-item-toolbar and pushed only to the
personal fork. The delivery metadata records the exact resulting source commit.

## Rollback

Preserve ThorToolbarRelease and its accepted 0.5.2-android.2 ZIP, SHA256
4f3c4062ff90b52a28643250507446b736fe8b78d1ed459736e65ec08334a8d3.
Select that group and restart the hosted game for the previous behavior.
Alternatively set ShowItemSwitchOverlay=false in the selected mod's config.json
and restart. Set HideToolbarWithCompanion=false to restore the full native bar.
Never clear app data, uninstall an app or replace real saves for this update.
