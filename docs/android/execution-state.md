# Android execution checkpoint

Execution is waiting for Android USB authorization. No mod build or device gate has passed yet.

- Branch: `android/thor-mobile` on `enjihn/stardew-thor-dual-screen`.
- Upstream baseline: `15f05b63d3ca852f3729e20de67fc7446bfac112` (0.5.2), identical to upstream main at setup.
- Preferred host: JunimoGate v0.3.1, source `029eafed917ae853d6c0a59330ee273524168296`.
- Current artifact/build ID: none.
- Gate 1: passed. Fork ancestry, pinned clean baseline, private-file exclusions and local build toolchain verified.
- Gates 2 through 8: not passed.
- Blocker: USB device is visible but unauthorized. Owner must accept Android's USB debugging RSA prompt before identification, extraction, or device tests.
- Original Play installation and saves: no device access or changes performed.
- Next action: after authorization, discover device properties and installed game/host packages using `tools/thor_adb.py inventory` with the authorized serial.

## Completed local work

- Setup commit: `f25059e56c1e13f98d8ce908c4b48fe1b3dd66a8`. The commit containing this checkpoint adds the tested helper and final local setup record.
- Installed project-local .NET SDK 9.0.118, Android workload 35.0.7, JDK 17.0.16+8, Android platform 35 and build-tools 35.0.0.
- A scratch Release Android library using framework Presentation/ImageView compiled with zero warnings/errors. This verifies tools only, not game or device behavior.
- Pinned JunimoGate GameInspector Release build succeeded with zero warnings/errors.
- Downloaded released host APK and verified SHA256 against publisher; extracted SMAPI, MonoGame and host bridge references into ignored `.local/refs/host/` and inspected their managed identities.
- Six focused ADB helper tests passed after their initial failing run. They cover serial scoping, timeout/exit checks, binary PNG preservation, failed capture, and staging checksum checks. No mock result counts as device acceptance.
- Actual serial-scoped inventory failed with `device unauthorized`. No device inventory file was produced.
- Source-only independent host investigation identified Activity, mod-import/group selection, and log-export APIs; it did not establish runtime success.

## Resume details

1. Read this checkpoint and `acceptance.md`. Keep the current branch and tools; do not restart setup.
2. Run `adb devices -l` after the owner accepts the RSA prompt. Confirm model/manufacturer/ABI before selecting a Thor.
3. Run `python tools/thor_adb.py inventory --serial SERIAL --out artifacts/private/device-inventory.json`.
4. Inspect actual package identities and saves before any host install/update. Continue Task 2's unmodified-host disposable-save baseline.
5. Pull the installed Play base and every split only after discovery. Run pinned GameInspector against those files, then proceed to Task 3's actual Release-host probe.

Private resumable state: `.superpowers/sdd/2026-10-30-thor-stardew-android-plan/progress.md`; host checkout is the sibling `JunimoGate/` at the pinned tag. Local setup/build logs are under `.local/`; the ADB failure is in `artifacts/private/adb-inventory-error.txt`.

## Decisions

- A fresh dedicated clone on the task branch provides isolation; no existing user checkout was modified.
- No desktop game substitutes or guessed game references were used. Exact Play version and all physical-device acceptance remain unknown pending authorized access.
- No host patch, mod DLL, mod ZIP, fixture, install, or save operation has been performed.

The complete specification is in `docs/superpowers/plans/2026-10-30-thor-stardew-android-plan.md`.
Private evidence and dependencies belong only in `.local/` or `artifacts/private/`.
