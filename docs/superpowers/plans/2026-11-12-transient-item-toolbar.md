# Temporary item-switching toolbar implementation plan

Approved user specification, 2026-11-12. Execute inline with Superpowers executing-plans,
TDD and verification-before-completion. Root owns coupled code and device work.

## Required behavior

Five passive 48-pixel Stardew slots, four-pixel gaps, bottom centered with16-pixel
safe-viewport inset. Show for1.5seconds after an actual deliberate selection;
fade in the final0.2seconds. Use original mobile slot art and item.drawInMenu.
Qualifying sources: native bumper/trigger/wheel switch methods and successful
Aim selection. Do not show or extend for automatic selection, inventory changes,
load or same-slot selection. Unrelated selection changes clear stale notices.
Never show while active or last committed companion panel is Bag; unknown
presentation suppresses. Entering Bag clears; leaving never revives. Clear on
menus/dialogue/cutscenes, warp, pause, load or companion loss. Only show when
companion is healthy and native toolbar actually suppressed. No touch routing,
camera, zoom/resolution, game behavior, assets, saves, host or launcher changes.

## Tasks

1. Add tested pure notice state and committed-panel tracking. Tests cover timer,
   fade, repeated and identical switches, eligibility resets, identity/epoch
   changes, boundaries and stale commits. Preserve existing visibility policy.
2. Add Android HUD renderer and narrow native selection observers with isolated
   Harmony ownership. Wire Aim, lifecycle and ShowItemSwitchOverlay defaulttrue.
   Respect HideToolbarWithCompanion. Failed init disables only this feature.
3. Build0.5.2-android.3, independent whole-branch review, fix material findings,
   then real device acceptance in a separate test group on ThorTest. Verify
   controller/Aim/all tabs, Bag and rapid transitions, touch-through, camera,
   failure/recovery, configfalse and previous android.2 rollback. Select a
   fixture-free release group only after acceptance. Deliver ZIP/SHA256,
   evidence, source commit and checkpoint; push only personal fork branch
   android/transient-item-toolbar. Preserve accepted android.2 and real saves.

Baseline: ce6cf1933291fed38d061ca733d2ef7714c5c2f6, game1.6.15.3/code245,
host0.3.1-thor.2/code12, SMAPI4.5.2-junimogate.41,
Harmony2.4.2-junimogate.64, Thor57ae2a4f Android13/API33.

Owner refinement during device testing: retain size and timing, but use the
original toolbar solid background behind the items. Build03 adds the exact
native slot and selection fills; its physical acceptance passed.
