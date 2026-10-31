# Android execution checkpoint

Execution active. The unmodified mobile game baseline works. No mod artifact is accepted yet.

- Branch: `android/thor-mobile` on `enjihn/stardew-thor-dual-screen`.
- Upstream: `15f05b63d3ca852f3729e20de67fc7446bfac112` (0.5.2).
- Host: unchanged publisher JunimoGate v0.3.1, commit `029eafed917ae853d6c0a59330ee273524168296`, code 11, SMAPI `junimogate-4.5.2.41`.
- Game: installed Google Play 1.6.15.3 / code 245 / ARM64. Android 13 API 33.
- Gate 1 passed. Gate 2 passed its gameplay, preservation and display requirements; the checked host Save Export route has a documented defect. Gate 3 runtime boundary passed with hardware pending. Gate 4 integration partially verified. Gates 5 through 8 pending.
- Current mod artifact/build ID: `7a7afe3-mobile14`, version `0.5.2-android.dev.14`, imported and enabled in `ThorBaseline`. ZIP SHA256 `94bbd3f3ee13c581e2e20b386208ff86130288638aefdc265ea3acad93c18c32`. This is an integration build, not an accepted release.
- Next action: diagnose the cold-load Aim watering conflict with fixture0.11, then remaining rewards, events/settings, import/restore and exact-candidate acceptance. Physical controller acceptance remains pending.

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
