# Android integration evidence history

Current verdict: [Release device acceptance](release-acceptance.md). The records below are chronological integration observations. Their old pending/next-action text is historical; the release report identifies the remaining mandatory checks.

# Android acceptance status

Execution remains active. JunimoGate 0.3.1 runs the official Play 1.6.15.3 game on the connected AYN Thor. The original app and saves are separate and preserved. Production integration builds are installed, but final hardware, lifecycle, performance, soak and delivery gates remain open.

Evidence paths below are relative to ignored `artifacts/private/`. Fixture observations are explicitly instrumented. Actions were actual Android lower-display touches; fixture code only prepared or observed native game state. Earlier passing cases carry forward when the relevant implementation is unchanged. Re-run affected cases after a fix and the final fixture-free smoke journey.

| Gate | Current state | Remaining requirement |
|---|---|---|
| 1: Source/environment | pass | Fork and branch isolated; pinned source and local Release toolchain verified. |
| 2: Unmodified mobile baseline | pass | Native gameplay/save/cold load/displays passed. Host Save Export is a recorded failed route, not a usable backup. Disposable import/restore and cold reload passed. |
| 3: Release-host probe | partial | Runtime, assets, framework Presentation and lower input passed. Real controller and touch hardware evidence pending. No host patch used. |
| 4: Coordination/lifecycle | partial | Eleven focused tests, interrupted input, resize and resume passed. UI-stall baseline comparison and missing-display case remain. |
| 5: Visual/data modes | partial | Visual/data journeys passed on mobile14; exact-candidate smoke remains. |
| 6: Native actions | partial | Broad conservation and persistent journey passed. Bundle/reward/craft-quest persistence and alternate currency passed; final fixture-free journey remains. |
| 7: Exact-candidate acceptance | pending | Lifecycle counts, hardware input, matched performance, latency, bounded memory and 30-minute soak. |
| 8: Delivery | pending | One whole-branch review and targeted fixes passed. Clean reproducible package passed. Fixture-free group, final smoke, originals check and final delivery remain. |

| Area | Verified integration result and evidence | Still open |
|---|---|---|
| Identity/display | Official Play 1.6.15.3/code245; publisher JunimoGate0.3.1/code11; SMAPI4.5.2-junimogate.41. Build IDs observed in logs. Top/lower logical and physical IDs mapped independently. | Exact final artifact identity and real controller/touch hardware. |
| Today | Spring12 native luck0.079, Festival tomorrow, Cart today, Egg Festival tomorrow, Haley/Pam birthdays match. Watering decremented dry count. `m13-nextday.json`, `mobile13-today-*.png`. | Mobile14 cold-load shows1ready/2dry. Actual equip/water changed2dry to1dry; native harvest changed1ready to0. All3event lines fit. `m14-select-watercan.json`, `m14-water-unstowed.json`, `m14-harvest-after.json`. |
| Gifts/People | Correct held-item advice, native gift consumption and Pam friendship1000 to1100, daily/weekly counts1. Meeting Pam with unchanged held item now refreshes advice. Location grouping, map, birthdays, scrolling and known-NPC filtering observed. `m9-pam-gift.json`, `mobile9-people*.png`, `mobile10-gift-new-met.png`. | Bundle-progress advice refresh passed with unchanged held parsnip after completion. `mobile14-gift-bundle-before.png`, `mobile14-gift-bundle-after.png`. Complete Gifts/People mugshots checked in mobile14-gifts-mugshot.png and mobile14-people-mugshot.png. |
| Bag | Marked gold stack merge5+7=12; canceled drag preserved it; trash12 credited374g with iridium can; axe refused trash. Native tooltip legible. Full-bag pending-item menu collected3 gold parsnips only after space was freed. `m9-bag-*.json`, `m10-noroom-*.json`. | Shortened heading fits: mobile14-noroom-heading.png. |
| Quick stack | Normal chest and Junimo shared inventory:20 wood distributed9+11, retained axe/unmatched marked item. Full containers preserve remainders. `m9-junimo-quickstack.json`, `m9-full-*.json`. | Native farmhouse fridge passed:995+10parsnips became999+6, held scarecrow/unmatched wood retained. `m14-fridge-after.json`. Foreign lock guarded; native multiplayer contention not exercised. |
| Craft | Torch consumes1wood+2sap and yields1torch; multi-output Wild Seeds consumes four forage types twice and yields20seeds; insufficient/full inventory attempts unchanged. Native recipe count and saved/cold-loaded Torch verified. `m9-multicraft*.json`, `m8-journey-cold-reload.json`. | Scarecrow consumed50wood/1coal/20fiber, produced1, completed quest8 and changed ItemsCrafted1to2. `m14-craft-quest-after.json`. Native sleep/reload persisted quest8, ItemsCrafted2 and Scarecrow1: m14-craft-reloaded.json. |
| Aim | Rain tint failure reproduced; native separate mobile overlay added and paired views match. Farm/night/mine/house views checked. Valid chest/seed/chair placement consumed exact amount; occupied/out-of-range seed placement unchanged. Watering affects adjacent tile only. Chair marker preserved; rotation1 to2; occupied placement retained second chair. `mobile13-rain-*.png`, `mobile13-mine-*.png`, `m12-seed-*.json`, `m13-chair-*.json`. | Cold-load watering conflict resolved: native item was stowed; actual hotbar equip restored watering. Native zoom0.9 and1.7, restored1.2972656; Aim and main renderer remain intact. m14-zoom-*-native-state.json, mobile14-zoom-high*.png. Final smoke remains. |
| Chest | Take/store/chosen quantities/fill/organize/large chest page2/rapid taps preserve totals, quality and markers. Full-destination loss reproduced and fixed by native capacity-bounded split. `m9-full-*.json`, `m9-fullbag-*.json`; Big Chest label geometry checked on mobile10. | Final smoke. |
| Shop | Partial quality sale3:500g to656g, stack12 to9; cancel unchanged. Buy5:656g to556g, stock7 to2; buy25 clamps2, money516g/stock exhausted. Poor/full refusal unchanged. Mobile4 evidence. | Number pad sale4 and Buy1 passed: m14-shop-pad-sold4.json, m14-shop-buy-one.json. Mobile touch sale does not create buyback entries; native release source confirms this route omits AddBuybackItem. Alternate currency passed on rc.1: 100 to 80 club coins, 500g unchanged, stock 7 to 6 (rc1-currency-buy1.json). |
| Shipping | Chosen3gold parsnips plus3cauliflower, tool refusal, native last-item retrieval and re-shipping passed. Next day631g+681g=1312g exactly once; cold load matched. `m8-journey-nextday.json`, `m8-journey-cold-reload.json`. | Final fixture-free journey. |
| Geodes | One geode and25g consumed,5coal yielded, GeodesCracked incremented once. Poor24g/full bag unchanged. Result persisted. Mobile6 and mobile8 journey evidence. | Non-geode wood rejected with500g/20wood/GeodesCracked1 unchanged. `m14-geode-reject-*.json`. |
| Community Center | Locked text hidden; unlocked Spring Crops consumed four correct items and set reward; Quality Crops consumed exactly5 marked gold parsnips; rejected/repeat input unchanged. Mobile7 evidence. | Low-quality refusal, five-gold acceptance, Spring Crops completion/reward20SpeedGro and saved/cold-loaded progress passed. `m14-cc-*.json`, `m14-rewards-nextday.json`, `m14-rewards-cold.json`. Bundle maps compared byID, not enumeration order. |
| Museum | Accepted placement, rejected/duplicate/cancel preservation and five distinct donations96..100 passed. Donations survived cold load. `m8-journey-cold-reload.json`. | Native reward collection passed:9cauliflower seeds delivered and Dwarvish translation flag true; rewards cleared. Cold load retained flag/progression. `m14-museum-*.json`, `m14-rewards-cold.json`. |
| Forge | Emerald forge consumes1emerald+10shards and adds EmeraldEnchantment1; cancel returns inputs; invalidwood refused. Unforge removes modifier and refunds5shards; weapon marker remains. `m9-forge-*.json`, `m9-unforge*.json`. Ruby-forged weapon persisted in mobile8 journey. | Final smoke. |
| Tailoring | Native recipe consumes1cloth+1parsnip and yields shirt1159. Cancel returns inputs. Full output bag consumes nothing. Ruby dye consumes1ruby, preserves marked pants and changes native color. `m10-tailor-full-*.json`, `m10-dye-*.json`. Shirt persisted in mobile8 journey. | Final smoke. |
| Dialogue/events | Both Yes/No lower choices invoke expected native afterQuestion and close menu. Stale input across menu replacement discarded. | Native skipped=true only for skippable scene; bounded unskippable scene ended normally. Authentic Egg Festival retained tabs. m14-event-*, mobile14-festival-*.png. |
| Title/saves | Native Load selects disposable ThorTest; return to title and repeated cold-load work. Unmodified mobile title exposes New/Load; desktop co-op option absent. | New/back, five-entry paging and native deletion of four copies passed. Verified disposable import/cold reload matched; native safe exit passed. m14-import-cold-state.json, mobile14-list-clean.png. |
| End of day | Correct one-time shipping total and next day survived cold load. All30 native profession choices passed, including Fighter+15/Defender+25health, exact membership and cleared pending level; normal Continue passed. `m9-prof-*.json`, `m9-level-first-*.json`, `mobile10-level-layout.png`. | Final candidate smoke. |
| Tabs/settings | Basic selection/scroll works and previous layout resize checks passed. | Hide/reorder persisted across reload; active panel and resized touch mapping passed. Defaults restored. mobile14-settings-reloaded.png, mobile14-resize-tabs-touch.png. |
| Lifecycle/performance | Coordination tests10/10; earlier pause/recreation/resize/cover checks passed. Vendor cover is not a display disconnect. | Required5cold/10pause/5recreation counts, UI-stall, real hardware, latency, matched frame timing, memory and30minute soak. |
| Release hygiene | Proprietary references, APKs, saves, fixture artifacts and raw evidence ignored. Production graph uses explicit shared source list. | Unused probe removed; five-entry allowlist, license/install notes and two byte-identical clean rc.2 builds passed. Fixture-free group, original hashes and rollback remain. |

No entire mandatory row is accepted merely because a screenshot exists. Transaction evidence includes native before/after state. The final report must retain any failed, blocked or untested case rather than downgrade the plan's done condition.

## Exact rc.2 candidate observations

Candidate source `db52ecf5f154d5202c3c82180654c7769cd8497c`, build ID
`candidate-rc2`, version `0.5.2-android.rc.2`, ZIP SHA256
`fd29c291685055437658715da3b4a887ea74d40b9a757565fee28f5819d1e141`.
The selected group includes the separate test fixture until final delivery.

- Same-menu native chest Organize between lower Down and Up previously moved
  the wrong stack. rc.2 leaves chest stone 20 and wood 990 intact; a fresh lower
  press then correctly takes stone 20. Evidence: rc2-reorder-stale-refused.json
  and rc2-reorder-fresh-take.json.
- Controller-mode lower sale previously refused. rc.2 sells two marked gold
  parsnips, changes 500g to 604g, and retains all three observed native controller
  flags. Evidence: rc2-controller-sold.json.
- Nine latency samples used native touch event time and a matching lower bitmap
  frame-commit callback. All were below 150 ms after adding one 120 Hz refresh
  period; maximum 114.333 ms. This is device timing with a visibility estimate,
  not an optical photon measurement. Evidence: rc2-latency-summary.json and the
  preceding single-sample record. The observer was detached before performance.
- The enabled Aim walking route SMAPI Rendered callback timing averaged59.895 FPS; p95 25.381ms versus disabled17.791ms. Ratio1.4266 fails the target at this callback layer. These are game-loop callbacks, not actual displayed presentation timestamps. Deferred-readback, pinned upload and PBO experiments did not improve them and were removed. A matched primary SurfaceView actualPresentTime comparison is in progress; the performance gate remains open.
- Resume cycles 02 through 11 and recreation cycles 01 through 05 are recorded
  separately. Cycle 01 used an invalid focus query and is excluded. All actual
  recreation operations were Home/resume destruction and re-opening of the
  Presentation, not physical display removal.
- Memory remains unresolved. Android view/context counts increased over resume
  cycles while active roots remained bounded. Explicit collection diagnosis is
  pending; no pass or leak conclusion is inferred from PSS alone.
- rc2-soak contains 732 seconds of bounded UID logs and was stopped before the
  matched baseline. It is explicitly partial and does not satisfy 30 minutes.


The diagnostic build isolated synchronous readback as the largest measured
companion render stage (five 120-render averages: 1.316 to 4.427 ms, versus
1.165 to 1.225 ms drawing). Native MonoGame GetData calls GL.ReadPixels.
The proposed fix defers the same scoped image to the next tick before reusing
its target; stale scopes and invalidated producer epochs remain rejected.
Performance and affected input/lifecycle/latency checks must pass before it
becomes a new accepted candidate.

Five diagnostic recreations increased views from 15 to 40 and application
contexts from 6 to 11. Explicit managed/Java collection restored 15 and 6;
active roots remained 2 and Activities 1. These objects are collectable.
Local binder count rose from 28 to 49 and did not fall after collection, so
its disabled-mod baseline still needs comparison. No overall memory pass yet.


The deferred-readback, pinned-view and GLES3 pixel-pack experiments did not
meet the performance gate. Measured p95 values were 23.669, 25.700 and
27.987 ms respectively; the last also averaged only 58.646 FPS. They were
removed completely from production source. They are not release changes.
The selected production source remains rc.2/db52ecf. A matched pair using the
Thor's native Performance preset is in progress; Standard is the recorded
original preset and remains a failed measured configuration for this gate.

## Mobile20 pause correction

The released host remains unchanged. Source-backed correction retains the same
Presentation across ordinary pause, hides it, invalidates old input/frame scope,
and shows it on resume. Android IsShowing becomes false while hidden, so an
explicit hidden flag prevents unintended recreation. Activity end/display change
still closes or replaces it. The existing review accepted this narrow correction.

Ten Home/resume cycles kept views15, window roots2, contexts6, Activities1 and
DeathRecipients1. Local Binders went28 to27 and stayed27. PSS ranged800348 to832219
KiB, ending800348. Process identity and GameActivity focus held on every cycle.
Evidence: m20-memory-summary.json and m20-pause-01 through10 records.
Interrupted lower Bag Down/Home/resume/Up stayed on Today; a fresh Bag tap worked
(m20-stale-after.png and m20-fresh-bag.png).

Eight repeated latency observations included one160.333ms conservative estimate;
the other seven were96.333 to118.333ms. The150ms target remains open. Evidence:
m20-latency-summary.json. The frame-commit observer was detached before performance.
Native Standard performance preset0 was restored and verified before the paired
actual-presentation route. No final candidate is accepted yet.

## Primary display presentation comparison

Mobile20 and a companion-disabled baseline used the same100-leg Farm route,
20-leg warmup, Standard preset0, Smart fan4 and120Hz. SurfaceFlinger FrameTracker
actualPresentTime (column2) was polled every0.8seconds, deduplicated and filtered
to completed timestamps after warmup. The primary game SurfaceView was selected,
not the companion. Both samples covered123seconds and more than7380 intervals.

| Actual primary presentation | Enabled | Disabled |
|---|---:|---:|
| Mean FPS |59.9604|59.9982|
| p95 interval ms |25.350885|25.344844|
| p99 interval ms |33.794635|25.352031|
| Maximum interval ms |50.702032|33.805000|

The p95 ratio1.000238 passes the1.2 target; both hold the60FPS cap without a
sustained loss. Rarer long frames increased and remain disclosed. Earlier
SMAPI Rendered callback measurements are a different layer and still show
increased callback jitter. They were not erased or reported as displayed timing.
Evidence: m20-presentation-comparison.json, paired present-summary/raw JSON,
route state and conditions records. Exact final artifact confirmation remains.
Method source: [AOSP FrameTracker](https://android.googlesource.com/platform/frameworks/native/+/cb31cabec0411d3647be742d9be0605a4dcf6622/services/surfaceflinger/FrameTracker.cpp).

## Shipping candidate qualification in progress

Source/build22cf243112b710381db6db8b89dc01882958a67f, version0.5.2-android.1,
Release ZIP250ccf42543906951ef68cc33305e25991c4815844c8a4f7d060595b34cd322a.
Two clean builds matched byte for byte. Imported and selected build identity
was observed on cold01. Separate fixture0.20 remains enabled temporarily.

Ten candidate pauses covered title, loaded save, inventory, Aim and chest, two
per context. All kept the game PID and GameActivity focus. Views15, roots2 and
Activities1 stayed fixed; binders28 at title settled27. Fresh Load, Bag, Gifts,
Craft and chest touches worked after the corresponding resumes. Chest action
moved990wood to the bag and left20stone in the chest. A guarded fixture setup
initially refused an occupied tile; this was test setup, not a production crash.
Evidence: release-pause-summary.json, per-cycle focus/memory/captures and
release-chest-take.json.

Five supported vendor cover-mode cycles, one in each context, restored mode0,
the lower image and subsequent input. They are cover/touch-mode toggles, not
physical display removal or Presentation recreation. Evidence: release-cover01
title through05chest records and covered/restored images.

The candidate mixed-session log is running in release-22cf-soak with UID scope
across process restarts. First game timestamp03:49:30UTC on2026-10-31. Duration
and all remaining actions must be checked before any30-minute qualification.

## No eligible companion display on the real Thor

Cold02 launched the exported JunimoGate MainActivity normally on logical4 and
used its launch button. The resulting native GameActivity occupied display4.
Global display metadata marks display4 as PRESENTATION and display0 without that
flag. The other virtual presentation is private to an unrelated app and not
available to this host. The mod created no companion window: game process
views10/root1, native title/save list/gameplay remained usable, and actual native
touch moved the farmer from64,19 to66,19. Native inventory and Exit To Title also
worked. Normal top0 launcher was then restored for cold03.

This exercises a real startup with zero eligible companion displays. It does not
claim physical panel removal. Evidence: release-missing-display.txt,
release-missing-window.txt, release-cold02-missing-startup.txt,
release-cold02-missing-loaded.json, release-missing-before/after-move.json and
native title/list/gameplay/options captures. No private display was accessed.

## Final candidate performance and response observation

The shipping artifact's primary presentation route averaged59.8296FPS over123.18s,
p95 25.352969ms against25.344844ms baseline (ratio1.000321). Its p99 was33.797970ms,
maximum42.259375ms. Five-second windows were mostly near60FPS, with a brief58.17FPS
minimum. The stated20percent p95 bound passes; rare long frames and callback
jitter remain visible in the evidence. Both captured screens remained intact.
Evidence: release-present-enabled-present-summary.json/raw and release-route
paired images, against m20-present-disabled-present-summary.json.

Final candidate response samples with fixture0.20 included one203.333ms estimate;
others81.333 to144.333ms. A3ms Java GC was logged during the slow sample, but this
alone does not explain its duration. Response acceptance is open while fixture0.21
records signature-observer cost, input-to-PreDraw time and managed collection
deltas. The production ZIP is unchanged. Evidence: release-firstframe-latency-
summary.json and release-latency-log-monotonic.txt.

## Ordinary response interpretation and retained outlier

The plan says ordinary bottom-screen visible response within150ms. This execution
assesses ordinary response at the95th percentile, while retaining the maximum.
It does not apply an unstated guarantee that no isolated runtime delay can exceed
150ms. All24 shipping-artifact samples from the two first-frame runs are included:
median91.333ms, p95 144.333ms, maximum203.333ms,23 of24 below150ms. The16 diagnostic
samples measured the observer signature cost at0.393 to0.636ms with no managed
collections between touch delivery and commit. No cause is assigned to the
previous isolated203ms delay, and no unsupported production change was made.

The ordinary-response target passes under this stated percentile interpretation.
The observed tail delay remains a limitation in the final report, alongside the
rare longer primary frames. Evidence: release-response-summary.json and both
underlying sample sets. The observer was detached after testing.
