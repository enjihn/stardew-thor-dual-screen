# Android execution checkpoint

Execution active. Hardware controls and final soak review remain mandatory.

- Fork/branch: `enjihn/stardew-thor-dual-screen`, `android/thor-mobile`.
- Shipping source/build ID: `22cf243112b710381db6db8b89dc01882958a67f`.
- Release version `0.5.2-android.1`, ZIP SHA256 `250ccf42543906951ef68cc33305e25991c4815844c8a4f7d060595b34cd322a`. Two clean byte-identical five-entry builds. Do not rebuild or change this artifact for documentation updates.
- Installed and selected group: `ThorRelease`, exactly one mod, no fixture. Cold05/06 logs confirm exact build ID and one loaded mod.
- Runtime: unchanged JunimoGate0.3.1/code11, SMAPI4.5.2-junimogate.41, official Play1.6.15.3/code245, Thor Android13/API33. No host patch.
- Current disposable farm: Spring16, 584g, one seed in bag slot1. Fixture-free lower drag, native sleep/save, exit/title, cold06 reload and both screens passed. Five normal top cold starts plus one zero-eligible-companion launch passed.
- Exact candidate: ten pause/resume and five supported vendor lower-cover/restore cycles passed. Cover is not physical removal. Views15/roots2/Activities1 and binders28 settling27 remained bounded. Missing-companion native load/movement/menu/exit passed, normal top launch restored.
- Performance: primary presentation mean59.8296FPS, p95 25.352969ms versus disabled25.344844ms, ratio1.000321. Brief58.17FPS window and worse tail jitter disclosed. Ordinary response p95 144.333ms, max203.333ms across all24 retained samples, event-to-first-frame plus refresh estimate.
- Standard0, fan setting1 (native Smart), refresh120Hz, cover0, focus settings and native densities/sizes restored. Earlier prose describing Smart as4 was a transcription error; both matched route records read1.
- Final three Play APK hashes and all three original save hashes match initial manifests. No real save imported or changed.
- Soak capture `artifacts/private/release-22cf-soak`: first game03:49:30.418849UTC on2026-10-31. Must reach04:19:30UTC and review logs/exit-info before accepting. Logger stops by35minutes, with32MiB cap, UID-wide across restarts. Current scan found no exceptions/errors/crashes; recorded exits are requested cold-start force stops.
- Physical input capabilities are readable with normal `getevent -lp`: controller event9, touch event5/6. Request one grouped real buttons/sticks/both-touch-panel test only after all other work. Injected input does not satisfy this row.
- Source review: one whole-branch review plus targeted substantial-fix followups passed. Production source unchanged since22cf; later commits only fixture diagnostics/docs. Eleven coordination and ten helper tests passed, fixture21 built and used for latency observation, absent from final profile.
- Current authoritative status and evidence: `release-acceptance.md`. Keep raw logs/identifiers/APKs/saves private. Remaining: final soak review, physical hardware response, update final verdict/checkpoint, sanitized delivery files, commit/push without force.

The sections below retain earlier integration evidence and failures. Their old next-action text is historical; use the current checkpoint above to resume.

## Verified baseline

- Empty `ThorBaseline` group selected, zero mods loaded. New disposable ThorTest farm created, starting gift collected, slept, exited to title, cold-started host and loaded Spring 2 with 500g and 15 parsnip seeds intact.
- Top logical display 0: 1920x1080. Lower logical display 4: 1240x1080. Distinct physical capture IDs verified privately from screenshots and metadata.
- Native touch movement and menus worked. Injected gamepad Start produced no observed menu response. Real physical controller acceptance is pending.
- Original Play app untouched. Three original save files copied, remote/local hashes matched and archive entries verified. One complete regular save pair and one pre-existing emergency-only folder preserved. No original save imported or overwritten.
- Host installed fresh alongside Play. Publisher APK SHA256 and signing certificate verified. Exact identities in compatibility.json.
- All three installed Play APKs extracted privately by pinned GameInspector, yielding 65 assemblies. Host references extracted from the verified APK. Private dependencies remain under `.local/refs/`.
- Local tools: SDK 9.0.118, Android workload 35.0.7, API 35, build-tools 35.0.0, JDK 17.0.16+8. Scratch Release Android library and GameInspector builds passed.
- Six ADB helper tests passed. This does not establish mod acceptance.

## Open host export defect

Save Export opens Android document creation but produces a zero-byte ZIP. Source `LiveSavesFragment.OnStop` clears `pendingExport` while the picker opens; the result handler needs that value. Do not treat this file as a backup. Original Play backups are independently verified and untouched. The disposable hosted farm reloads successfully.

Logs > More > Export all > Export > Downloads succeeded. Pulled diagnostic ZIP passed CRC/content checks (6004 bytes, SHA256 `425e88d87cbc175db4338ae422bd33f1042048bfb7615a786bb2787d5feac1d9`). Private evidence: `artifacts/private/host-baseline-diagnostics.zip`.

The release-host probe passed without a host patch. Task 2 requires checking export/import; Gate 2 does not require repairing launcher export. The earlier self-imposed requirement to fix this host defect is withdrawn. Keep the publisher host unchanged, disclose its failed export route, verify a disposable backup restore, and never count the empty ZIP as a backup. Original-save migration remains untested.

## Resume details

Use the existing branch and local tools. Private task ledger: `.superpowers/sdd/2026-10-30-thor-stardew-android-plan/progress.md`. Sibling `JunimoGate/` is pinned to v0.3.1. Device serial, capture IDs, APK hashes, saves and screenshots are under ignored `artifacts/private/` and `.local/`. Do not republish them.

Baseline reload evidence: `baseline-reload-ready.png`, `baseline-reloaded-logcat.txt`; backup manifest: `play-save-backup-manifest.json`. Current source checkpoint before probe: `f352bd02699b041f89756da65c65024b91811ffe`.

## Release-host probe result

Release probe built with zero warnings/errors. Exact build ID observed through SMAPI in the unchanged publisher host. Framework Presentation and ImageView with Presentation context rendered the actual game cursor texture and small font on logical display 4. Touch Down/Up visibly incremented the counter, with 1240x1080 drawable and top Activity focus true. Injected gamepad A reached SMAPI as Enter; this is injected key routing, not proof of physical buttons or axes.

One Home/background/resume cycle restored the lower image and touch. One vendor top-only/dual-screen toggle via the existing `dual_screen_display_mode` setting blacked the lower panel and restored it with working touch; original mode and focus settings were restored exactly. This is the vendor cover/touch toggle, not physical disconnect or lid closure. A separate lower-size override/restoration survived; original 1080x1240 physical size (rotated logical 1240x1080), density 369, no override restored. No host patch is justified for framework window/touch loading.

Evidence: `probe1-startup.txt`, `probe1-touch.txt`, `probe1-resume.txt`, `probe1-vendor-cycle-logcat.txt`, matching private images and original/restored setting records. All remain private. Production reliability and final physical acceptance are still pending.

## Production integration checkpoint

The shared original Panels and CropTracker now compile into the mobile library. Eight production coordination tests pass: old menu/layout/session input rejection, displayed-image identity, cancellation/edge mapping, newest-frame replacement, active-buffer ownership and invalidation. One compiler warning retains the game category-aware ingredient-count API intentionally; zero build errors.

Device build `75b2e87-mobile1` exposed the native Android title release callback difference. Corrected in `75b2e87-mobile2`: bottom Load opened the native save list and selected the disposable Spring 2 farm with 500g and 15 seeds intact. Today, Gifts, People, Bag, Craft, indoor Aim and Tabs rendered. These are initial visual checks, not full row acceptance.

A lower-screen Down interrupted by Home/background/resume did not replay when Up arrived; the panel remained Today. A separate Down followed by a top menu change and Up was also discarded. Fresh Bag touch worked. Presentation reopened on resume. A temporary size change and vendor top-only/dual toggle restored with working touch; original size/mode verified equal. These are resize/cover operations, not physical panel disconnect. Top GameActivity retained its per-display focus.

Private evidence: `mobile2-startup.txt`, `mobile2-farm.txt`, `mobile2-pause.txt`, `mobile2-display-cycle.txt`, `mobile2-display-restoration.json` and associated two-screen images. Remaining Gate 4 stress/missing-display details will be exercised with the separate disposable-farm fixture; Gate 4 is not declared complete.

Runtime differences identified from actual Play assemblies: private bundle/source-item fields; instance save-list geometry with native release activation; mobile title day/night gradient assets and two available title buttons. The adapter currently validates its initial bindings. Full required-member/asset validation and Aim pass corrections remain in Task 5. No released-host window patch is needed.

## Mobile action checkpoint

Mobile8 and separate fixture 0.6.0 are selected in ThorBaseline; both identities were observed at startup. No accepted release exists. Earlier build observations below are integration evidence only.

- Native runtime member and asset validation now covers the shared mobile panels before enabling input. Mobile title gradients and Aim background/water/tool/above-layer passes were corrected from actual Android source. Full visual acceptance is still pending.
- Ten coordination/selection tests pass. New quantity tests failed before ActionSelection existed, then passed. They reject replacement menu/item identity, empty selection and nonpositive quantities, and clamp to the current stack.
- Mobile4 lower-screen shop tests passed: sale of 3 gold-quality parsnips credited 156g, leaving 9 with exact quality/metadata; cancel changed nothing; buying 5 seeds cost 100g and reduced stock by 5; bulk buy clamped to the remaining 2; insufficient currency and full inventory preserved money, stock and inventory. These are fixture-instrumented integration tests, not final candidate acceptance. Private m4-shop-*.json records and screenshots hold before/after state.
- Fixture snapshot of a native chest crashed with SIGSEGV. Normal top-screen chest placement/opening worked, then snapshot alone reproduced the crash. Source inspection identified the released host's rewrite of native MenuWithInventory.heldItem to a desktop facade property with a different instance layout. Mobile5 and fixture0.3 use validated native reflection instead. The same chest setup/snapshot now runs successfully; startup no longer rewrites that field. No host patch.
- Mobile5 native profession choice via lower touch added Rancher (0) once and closed the menu. Persistence/other choices remain pending.
- Mobile5 chest single tap only selected the native item, contrary to the companion's transfer UI. Geode tap did nothing because Android requires slot selection then anvil press/release. Mobile6 source now routes storage movement and geodes through those native paths; device verification is next. Fixture chest seeding was also corrected to use addItem rather than append after native empty slots.
- A regular disposable-save pair was exported by the guarded fixture before item seeding. ZIP 104672 bytes, SHA256 695c652db79536adff5efd1d6bc9ae74bf1e44c6a831f418a5a01f84ae10d51e, CRC/XML validated. This supplies the test baseline backup but does not fix launcher Save Export. Restore has not been tested.
- Mobile3 bag drag moved a 12-item quality-2 marked stack from slot 5 to slot 12 with exact total and metadata conservation.

Resume: finish mobile6/fixture0.4 build and selection, verify native chest full/partial transfers and geodes, then remaining action matrix, save/reload, visual/lifecycle/performance/hardware/soak gates and final review. No mandatory gate is waived. Latest pushed source checkpoint is 7a7afe3.

## Mobile6 to mobile8 integration

- Mobile6 native geode flow consumed exactly one geode and 25g, produced five coal, and incremented GeodesCracked once. At 24g or with no output space, money/items/stats stayed unchanged.
- Mobile6 chest whole and chosen partial transfers conserved quantities and metadata through native mobile movement. Quick stack preserved the held axe and unmatched marked parsnips while moving matching wood/stone to the current-location chest. Full-destination restrictions remain pending.
- Mobile6 Torch crafting consumed one wood and two sap, produced one torch, and incremented the recipe count. Full inventory and insufficient ingredients consumed nothing. Multi-output and persistent quest checks remain pending.
- Mobile6 tailoring through lower selection plus native top completion consumed one cloth and one parsnip, producing shirt (S)1159. Forge consumed one ruby and ten cinder shards; modifier identity was not yet observed, so that row remains partial.
- Mobile6 dialogue lower answers failed. Source showed native release commits the answer. Mobile7 verified both offered choices via actual lower touch with the expected native afterQuestion result and menu closure.
- Mobile7 now hides unreadable Junimo bundle information. With fixture progression enabled, lower selection opened the native bundle page. Spring Crops consumed one each of four items and set its reward flag. A marked gold-quality parsnip stack retained identity/quality with 11 remaining; a Quality Crops donation consumed exactly five more. Rejected and repeat inputs consumed nothing. Save persistence remains pending.
- Mobile7 Museum lower selection did nothing. Native source selects on release. Mobile8 adds that callback; its device check is next. The fixture shipping menu now uses native highlight, selection callback and initializeShippingBin geometry.
- Latest source verification: production Release build succeeded with one intentional obsolete category-count warning; fixture Release build succeeded with zero warnings/errors after adding the required message argument. Ten coordination/selection tests and ten Python helper tests passed.

Pending: mobile8 museum/shipping checks, remaining action and visual cases, a saved/cold-reloaded action journey, production stress/performance/lifecycle/hardware/soak acceptance, fixture-free final group, one independent whole-branch review, and final artifact delivery. No mandatory gate is waived.

Mobile8 device follow-up: museum lower selection and native placement passed, including duplicate/rejected-item preservation and cancellation. Five unique donations were recorded. Shipping partial/full quantities, tool rejection and native last-item retrieval passed. A pending persistence journey has 631g, three shipped gold parsnips plus three cauliflower, one crafted Torch, one cracked geode, one tailored shirt and a Galaxy Sword with RubyEnchantment level 1 and preserved marker. It has not been saved yet. Current next action is native profession choice and overnight save/cold reload, then remaining cases.

Mobile8 persistence passed: one lower Continue advanced to Spring 3 with 1312g (631g plus 681g shipping). After title exit and full process restart, fixture snapshots matched day, money, every item/count/quality/marker/enchantment, professions, crafting recipe counts, GeodesCracked and all five museum donations. Regular-save export passed ZIP CRC and XML checks; SHA256 cb75b8b7a77ed879e9ad6a7b36b0ffcfeb95405b76f602936a84eff5b83119f1. Private evidence m8-journey-nextday.json, m8-journey-cold-reload.json and mobile8-journey-save-validation.json. This is instrumented integration evidence on dev8, not final-candidate acceptance.

Fixture0.7 is being imported for the remaining required container, visual/progression and bounded UI-stall test conditions. It remains separate from the production ZIP. The device has no supported display-disable command in cmd display help; do not count vendor cover toggles as physical display disappearance.

## Mobile9 container and native-action follow-up

Mobile9 plus separate fixture 0.7 are imported and selected; both startup identities were observed. Mobile9 fixes a real mobile8 failure: storing 20 wood into a full chest with nine remaining stack spaces erased the 11-item remainder through a native whole-item callback. The adapter now uses the native split path bounded to the chosen destination slot. Device checks passed for nine moved and eleven retained, the mirrored nearly-full-bag transfer, full-destination rejection, marked-quality partial quantities, fill stacks, organize, repeated input and second-page Big Chest take. Junimo quick stack now reads the native player/global inventory and passed conservation across normal and Junimo chests. Private evidence: m9-full-*, m9-fullbag-*, m9-split-stored.json, m9-fill-organize.json, m9-rapid-storage.json, m9-big-page2-take.json, m9-junimo-quickstack.json.

Forge invalid material, cancel, EmeraldEnchantment creation and native unforge passed. A marked Galaxy Sword retained its marker, forging consumed one emerald and ten shards, and unforge removed the modifier and returned five shards. Tailoring selected ingredients through the lower panel and used native top ingredient slots: cancellation returned both ingredients; completion consumed one cloth and parsnip and made one shirt. Two Spring Seeds crafts consumed two of each native forage ingredient, yielded twenty seeds and recorded twenty recipe outputs; a third insufficient-input attempt changed nothing. Evidence: m9-forge-*, m9-unforge*.json, m9-tailor-placed*.json, m9-tailor-output.json, m9-tailor-return.json, m9-multicraft*.json.

Current source also shortens the paged chest heading and moves profession cards below the level title after observed overlapping text. These visual edits and a native foreign-mutex guard for quick stack are not yet built/device-checked. All thirty native profession choices are being checked with the isolated fixture. Current device state is unsaved test state; retain the verified Spring 3 regular-save backup and discard this setup before final acceptance.

Next: finish profession choices, remaining visual/data/action conditions, build the affected fixes, then Task 7 exact-candidate lifecycle/performance/physical-controls/30-minute soak and Task 8 independent review and fixture-free delivery. No hardware result is inferred from injected ADB input. No final release is accepted.

## Mobile10 follow-up and current diagnostic build

All thirty native profession choices passed on mobile9, including Fighter/Defender health increases, exact profession membership, menu closure and pending-level removal. A normal level Continue also passed. Profession list order is native and is not required to be insertion order. Mobile10 verified the corrected profession-card geometry and a choice at its new position. The shortened Big Chest heading and counter no longer overlap.

Mobile9 Gifts advice failed to refresh after meeting Pam while retaining the same parsnip. Mobile10 includes friendship membership and bundle progress in that cache dependency; the same real lower-panel sequence now displays Pam under Loves without changing the item. Actual native gifting consumed one gold parsnip and changed Pam from 1000 to 1100 points with daily/weekly gift counts 1; the lower marker updated. People map, location grouping, birthday marker, Gifts subview and scrolling were observed with native character data. Some mobile mugshots were cut off by the upstream fixed 16-pixel crop; the next build uses the complete native mugshot rectangle fitted to the existing icon area.

Bag merge and canceled drag preserved a marked quality stack. Trashing twelve gold parsnips with the iridium trash can credited 374g; the axe refused trash. The full parsnip tooltip rendered readably. Native no-room menu refused a waiting stack until lower drag trashed a bag stack, then lower take collected all three gold parsnips. Its long heading clipped and is shortened in the next build. Tailoring with a full bag consumed nothing, and dyeing consumed one ruby, preserved the pants marker and changed native clothing color. Private evidence: m9-prof-*.json, m9-bag-*.json, m9-pam-gift.json, mobile9-people*.png, m10-gift-*.json, mobile10-level-layout.png, m10-noroom-*.json, m10-tailor-full-*.json, m10-dye-*.json.

Aim valid chest placement consumed one chest; tapping the occupied target consumed no second chest. Outdoor and night views exposed a tint mismatch against the primary view, so Aim is not accepted. Current source contains a temporary one-shot THOR_NIGHT_DIAGNOSTIC in RenderMobile to report nonopaque pixel count and actual lighting/viewport settings. Remove this diagnostic after the cause is identified. It is not a finished feature. Mobile11 (SHA256 9f3ee0ce7b132a4fcb9c5a5389acbf80640a49600c7d5673a1d15bc911921108) and fixture0.9 (SHA256 39feaf99d14380f1781b2cf4ebc60b9d05026368a84c6aad652cd164ec75ba6e) were built and staged; import/selection is in progress. The fixture adds only nearby object/furniture/soil observations for Aim assertions.

Mobile10 production and fixture0.8 Release builds passed, ten coordination/selection tests and ten Python tests passed. Mobile11 and fixture0.9 also compile. Current unsaved tests were discarded by native title exit; the saved farm remains Spring 3, 1312g with the verified persistent journey. Latest pushed commit remains 7a7afe3; newer source changes are uncommitted.


## Mobile13 rendering and data checks

The rain mismatch was reproduced on mobile12. Marking the Android bitmap opaque did not fix it and was reverted. Native mobile DrawWorld applies a separate blue rain overlay after DrawLightmapOnScreen. Mobile13 adds that same pass, including the native green-rain variant. Paired screenshots now match the top view's rain/night tint. Mine 1 and farmhouse rendering also passed visual inspection. The temporary night diagnostic was removed; MobileHost is unchanged from the prior verified architecture.

Aim watering changed only the adjacent soil tile. Seed placement consumed one seed at a valid adjacent tile, and neither an occupied tile nor a tile outside placement range consumed another. Indoor chair placement preserved its marker and selected rotation; tapping the placed chair changed rotation 1 to 2 without duplicating it. Another chair refused the occupied tile and remained in inventory. Evidence: m12-water-after.json, m12-seed-*.json, m13-chair-*.json, mobile13-rain-*.png, mobile13-mine-*.png and mobile13-furniture-*.png. Mobile13 ZIP SHA256: 78f9f74164265c7808565b4e03daab59c2cd399fda02eec0e9a0981ee7c5f538. Fixture0.10 SHA256: 4dafcb2894f4fb9569d4437a0e1cf0fc8389173f0542b8cf16e196e7671e4788.

The disposable farm advanced to Spring 4 during a long night diagnostic. The Spring 3 journey ZIP remains intact and is the persistence reference. A deliberate fixture setup then slept normally from Spring 11 to Spring 12 for Today coverage. The saved Spring 12 pair is independently exported and CRC/XML checked in m13-today-export.zip. No original Play save was used.

Today correctly showed native luck 0.079, tomorrow's Festival, today's Traveling Cart, tomorrow's Egg Festival, and Haley/Pam birthdays. Its third event line overlapped the card border. More materially, it counted seven native IslandWest wild ginger plants as dry, in addition to two actual unwatered farm crops. The native needsWatering fallback returns true for ginger even though it is harvested with a hoe and does not need cultivation. Mobile14 excludes forage crops from dry counts and narrows only the Android event-line spacing. The observed watering action decremented the existing count by one. Verify the corrected total and geometry after cold loading mobile14 before accepting this fix.

Mobile14 build: 7a7afe3-mobile14, ZIP SHA256 94bbd3f3ee13c581e2e20b386208ff86130288638aefdc265ea3acad93c18c32. Release compilation passed with the existing category-count warning. Import into ThorBaseline is in progress; fixture0.10 remains selected separately. Latest pushed commit is still 7a7afe3. No final candidate or hardware acceptance is claimed.


Mobile14 follow-up: cold-load Today displays exactly 1 ready and 2 dry crops, and all three upcoming-event lines fit. Instrumented farm state has one mature and two immature dry crops; the saved IslandWest data confirms seven forage ginger plants, now excluded. Evidence: m14-start.json and mobile14-today-cold.png. Runtime mobile14 identity observed in mobile14-startup.txt.

A subsequent Aim watering attempt after cold load did not change soil and ended facing right instead of the tapped direction. Earlier mobile12/13 watering passed. This conflict remains open, not accepted. Fixture0.11 adds native CanMove, lastClick, tool-location, water, event and zoom observations plus bounded native event and fridge setup. It compiles with zero warnings/errors. ZIP SHA256 87f1b68b80777be633e7f0f2a2f87a7dd2b2b0116e304df1e6eb3168730166eb; import is in progress. Do not alter production tool logic until this state is observed.


Mobile14 watering conflict resolved without a production change: after cold load native CurrentItem is stowed, so CurrentTool is null even though CurrentToolIndex remains0. Real Aim hotbar selection equipped the can; the next tap faced down, changed tile59,16 from dry to watered, and WaterLeft40 to39. Today changed2dry to1dry. Native harvest then changed ready1 to0 and normal parsnips9 to10. Evidence: m14-water-debug-before.json, m14-select-watercan.json, m14-water-unstowed.json, m14-harvest-after.json and mobile14-today-live.png.

Mobile14 geode rejected wood without changing500g,20wood or GeodesCracked1. Museum lower reward take delivered9cauliflower seeds and consumed the Dwarvish translation reward through the native special-item callback; reopening had no rewards. Community Center low-quality refusal, five-gold-parsnip donation, Spring Crops completion and20Speed-Gro reward take passed. The reward flag cleared and Gifts no longer suggested the completed Spring Crops bundle while holding the same normal parsnip. Evidence: m14-geode-reject-*.json, m14-museum-*.json, m14-cc-*.json, mobile14-gift-bundle-before/after.png. The first quality attempt used an empty second-row slot and is not counted; the suffixed2 files use the visible correct bag row.

Native sleep advanced to Spring13 and produced a verified regular-save ZIP, m14-rewards-save-export.zip, SHA25610d87739dae62232cbcf421c37c8a62f1bca9fbee3a414d5c8c7770f7ff868a4. The subsequent attempted native title exit instead showed the Pantry menu, so it is not safe-title-exit evidence. The saved disposable process was stopped for fixture update. No real save was involved. Cold reload is now in progress and must compare saved reward/inventory/progression state.

Fixture0.12 adds native Dwarvish/ItemsCrafted/quest observations, an existing Scarecrow quest setup, and optional native farmhouse kitchen upgrade for fridge scope. It compiles cleanly. ZIP SHA256ca5e2c8db9dd5d13c5a903b00f4fc5d47d1e7fc22ef0fd802cabf51664bf8655 was imported and selected. Production remains mobile14, unchanged. Independent narrow source review confirmed that native checkForCraftingAchievements recomputes ItemsCrafted from recipe counts; do not add a duplicate increment. Final whole-branch review is still pending.


Mobile14 reward cold load passed: inventory, day, donations and reward flags match; bundles match by ID (native dictionary enumeration order changed without state change). Dwarvish translation is true. Scarecrow craft then consumed50wood/1coal/20fiber, yielded1, completed quest8 and increased ItemsCrafted1to2. Native farmhouse fridge quick stack moved10parsnips into995 existing, resulting999+6 with held scarecrow and unmatched5wood retained. These new unsaved checks still need persistence.

Skippable scene lower touch set native skipped=true and ended. Non-skippable lower touch left skipped=false. The initial fixture's global pause did not finish, so it was discarded by cold loading the verified Spring13 save. Source confirms both native skippable commands exist; fixture0.13 uses native precisePause for a bounded scene. Fixture0.13 SHA2568acf87a7a88d6f528153dff7ce3643ba8c2946a4f5b4b93cdcef7fb0416a629f is selected with unchanged mobile14. No production fix was made for this fixture issue.


## Pre-candidate completion

Mobile14 completed native bounded skippable/non-skippable events, the actual
Egg Festival, fridge quick stack and Scarecrow quest/stat persistence. Native
sleep and exit/reload retained Spring14, ItemsCrafted2, quest8 complete and
one Scarecrow. The verified regular-save ZIP m14-craft-save-final.zip has
SHA256 1cb0ce8cfe798ad1a011cb00877d81d3747ea34b590bafa3e2b15e0f36708349.
The earlier export captured Spring13 before the asynchronous save finished;
it is not Day14 evidence.

Normal host Save Import restored the verified Spring13 disposable ZIP.
The import and cold-load snapshots matched inventory and progression.
Native New/back and five-entry Load paging passed. Four list-only test copies
were deleted through the native Load UI after verifying their names; ThorTest
is the only remaining active test slot. Temporary top density220 and lower
size900x1100 were restored to physical defaults, density369 on both panels.
Tab hiding/reordering persisted across title/load, then defaults were restored.

Fixture0.14 corrected zoom setup to native PinchZoom. Actual game zoom0.9
and1.7 preserved Aim and top rendering; original1.2972656 restored. Gifts/People
show complete native mugshot regions and the no-room heading fits. Number pad
sale4 changed500g to708g, marked gold parsnips12to8; Buy1 charged20g and reduced
seed stock7to6. Native mobile touch sale exposes no buyback entry: its release
path does not call AddBuybackItem. Desktop buyback methods existing in the
assembly do not imply that this mobile sale route supports them.

A deliberate1200ms Android UI stall recovered with working Today touch and no
new menu/state mutation. Rendered timing contained one1232.87ms gap, then normal
16.7ms frames. Compare the same stall with the mod disabled before attributing
that gap to the companion. Missing-display removal is still untested because
no supported device command has been found; vendor cover is not disappearance.

Production code is unchanged from2473270. Packaging now includes the license,
attribution and installation notes, and removes unused probe source. Exact
rc.1 identity, clean build, reproducibility and acceptance are next. Fixture0.15
adds observation/setup for supported alternate shop currency only and remains
excluded from the release archive. No final release is accepted.

Clean rc.1 rebuild initially changed only DLL identity/debug hashes despite
unchanged code. The Android project now omits its unused generated Resource
class and PDB metadata. The SDK designer-assembly feature remains enabled, as
required by the actual host/game references. Two clean verification builds now
produce identical ZIP bytes; the DLL contains no local Windows path. Earlier
rc.1 attempts were staged but never imported or selected. Build the committed
correction before device acceptance.

## Candidate and independent review

Candidate rc.1 source20bc710cfefbff75b4b9c8e49c5ca52f3a67ff8c, buildID
candidate-rc1, SHA2560b3f23803e59a6846f7399173bf7d4de1f8f68a95fa2b1553fc46a959955bbfd
was imported and its identity observed. Five-entry allowlist and two clean
byte-identical builds passed. Alternate-currency Buy1 charged20clubcoins
(100to80), left500g unchanged and reduced stock7to6.

The single independent whole-branch review completed and found two P2 issues.
A279ms lower Down/top-native-Organize/lower Up reproduced stale chest selection:
stone20 transferred instead of the originally displayed wood990. Private
rc1-chest-reordered-stale-up.json/png. Uncommitted fix adds chest slot identity
revision to the published scope. Its new test failed before the helper existed,
then11/11focused tests passed and Release compiled. Device fix check pending.

The reviewer also identified lower Sell rejecting native SnappyMenus after
controller-mode entry. A separate fixture now observes and prepares that
condition for a real lower touch. Fixture0.16 had a JSON field-name collision
(snappyMenus/SnappyMenus), fixed in0.17; no production behavior changed for it.
The fixture's latency observer uses native touch timestamps, bitmap signatures
and Android frame commit callbacks, with no production handler invocation.
Raw screenshot receipt is too slow to measure the150ms target: six lower PNG
captures had median186.56ms host overhead. Precise frame-commit measurements
and their display/optical limitations must be recorded separately.

No other material source finding was raised. The reviewer will inspect only
targeted fixes and final evidence as an update to this same review. Required
lifecycle counts, matched performance, bounded memory,30minute soak, physical
controls and fixture-free delivery remain open. Android device_state exposes
only DEFAULT, with no supported folded/display-absent state.

Controller-mode refusal reproduced on rc.1 with fixture0.17: optionEnabled,
gamepadControls and active SnappyMenus were all true; actual lower Sell left
500g and both marked gold parsnips unchanged. rc1-controller17-sale-attempt.json.
The adapter now temporarily uses native pointer mode only during the sale,
restoring previous gamepadControls in finally. Together with the chest scope
fix this forms rc.2; device regression tests are still pending.


## Performance correction in progress

rc.2 failed the matched p95 gate (25.381 versus 17.791 ms). Native MonoGame
GetData is synchronous GL.ReadPixels. A bounded diagnostic measured readback
1.316 to 4.427 ms and drawing 1.165 to 1.225 ms per update. Deferring the same
scoped target read until the next game tick improved p95 to 23.669 ms on
mobile15, still above the 21.349 ms threshold.

A second bounded diagnostic measured the managed-array to direct-buffer copy
at 2.283 to 2.599 ms, versus only 0.131 to 0.143 ms for Bitmap's own copy.
Mobile16 gives each of the three owned queue buffers a cached pinned Java
ByteBuffer view. Bitmap copies synchronously before queue completion; pins
are released only during serialized UI teardown. No new GPU readback API,
UI redesign or update-rate reduction was introduced. The same review's
narrow updates accepted deferred ownership and pin lifetime. Real performance
and affected device checks are pending. Private diag1-profile.txt and
diag2-profile.txt retain the measurements. Fixture0.19 ZIP SHA256 is
027b1fd3b3de2da7eadf128154a3ba4ea715091572adca2a63060b937ee2ae4d.


Mobile16 also failed the frame-time gate: p95 25.700 ms and mean 59.734 FPS.
Do not claim the pinned-view change alone solved performance. Mobile17 tests
nonblocking GPU completion, with at most one transfer outstanding. Actual
startup logs show GLES3.2, compatible with this path. It restores pixel-pack
state, defers mapping until signaled, discards obsolete scoped images and
checks the current EGL context before touching native names. Native/context
lifecycle review is pending alongside device results. No new candidate has
passed performance yet; do not start the final soak prematurely.


Mobile17 asynchronous readback failed: mean 58.646 FPS, p95 27.987 ms.
It also exposed a context-lifetime review issue, corrected in an uninstalled
mobile18 build, but the whole unsuccessful renderer experiment was then
removed. Do not install or deliver dev15 through dev18. Production source
and manifest now exactly match rc.2/db52ecf again; only docs and fixture0.19
collection diagnosis remain uncommitted. Current diagnostic direction is a
matched baseline/rc.2 comparison in the native Performance preset, using the
observed Quick Settings menu. Preserve Standard=0 as rollback. No Performance
preset pass is claimed before the paired result.


The disabled baseline resource comparison stayed bounded (binders20 to19,
views10, roots1). AOSP Android13 Presentation creates a separate WindowContext;
its cleanup depends on context finalization. The minimum mobile19 fix reuses
the existing Presentation for ordinary pause, with Ready=false and full input/
frame invalidation while hidden. Resume shows it again. Finishing/destroyed
Activity still disposes, and actual display removal/replacement still closes
and recreates. Native hide/show tests and resource counts are pending. Do not
call these operations window recreation in the final report; the supported
vendor lower-display toggle remains available for the required toggle cases.
