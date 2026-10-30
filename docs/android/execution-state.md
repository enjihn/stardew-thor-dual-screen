# Android execution checkpoint

Execution active. The unmodified mobile game baseline works. No mod artifact is accepted yet.

- Branch: `android/thor-mobile` on `enjihn/stardew-thor-dual-screen`.
- Upstream: `15f05b63d3ca852f3729e20de67fc7446bfac112` (0.5.2).
- Host: unchanged publisher JunimoGate v0.3.1, commit `029eafed917ae853d6c0a59330ee273524168296`, code 11, SMAPI `junimogate-4.5.2.41`.
- Game: installed Google Play 1.6.15.3 / code 245 / ARM64. Android 13 API 33.
- Gate 1 passed. Gate 2 gameplay, preservation and display prerequisites passed; host save export defect remains open. Gates 3 through 8 pending.
- Current mod artifact/build ID: temporary `6b39d60-probe1`, version `0.5.2-android.probe.1`, imported and enabled in `ThorBaseline`. ZIP SHA256 in compatibility.json.
- Next action: Task 4 production coordination tests and lifecycle integration, retaining the shared panels. Probe runtime boundary passed; physical controller acceptance remains pending.

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
