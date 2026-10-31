# Android execution checkpoint

Execution active. The unmodified mobile game baseline works. No mod artifact is accepted yet.

- Branch: `android/thor-mobile` on `enjihn/stardew-thor-dual-screen`.
- Upstream: `15f05b63d3ca852f3729e20de67fc7446bfac112` (0.5.2).
- Host: unchanged publisher JunimoGate v0.3.1, commit `029eafed917ae853d6c0a59330ee273524168296`, code 11, SMAPI `junimogate-4.5.2.41`.
- Game: installed Google Play 1.6.15.3 / code 245 / ARM64. Android 13 API 33.
- Gate 1 passed. Gate 2 gameplay, preservation and display prerequisites passed; host save export defect remains open. Gate 3 runtime boundary passed with hardware pending. Gate 4 integration partially verified. Gates 5 through 8 pending.
- Current mod artifact/build ID: `5ed81c6-mobile8`, version `0.5.2-android.dev.8`, imported and enabled in `ThorBaseline`. ZIP SHA256 `d13651a5750c4db71ee4bf86f64003c705588341bbee67818be4faa3a6735c68`. This is an integration build, not an accepted release.
- Next action: finish Task 4 stress coverage and Task 5 runtime adapter/visual corrections; prepare isolated Task 6 fixtures. Physical controller acceptance remains pending.

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

Continue the independent release-host probe before considering any host patch. Save export must be resolved or explicitly blocked before final delivery.

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

Resume: finish mobile6/fixture0.4 build and selection, verify native chest full/partial transfers and geodes, then remaining action matrix, save/reload, visual/lifecycle/performance/hardware/soak gates and final review. No mandatory gate is waived. Latest pushed source checkpoint remains 5ed81c6 until these changes are committed.

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
