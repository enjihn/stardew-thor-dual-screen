# Android acceptance status

The unmodified mobile game baseline is verified. There is no accepted candidate or final mod artifact.

USB authorization is resolved. The original Play app and saves are preserved. New disposable hosted farm saved and cold-reloaded at Spring 2, 500g, 15 seeds. Save Export produced an empty file; diagnostic export succeeded. See execution-state.md.

## Gates

| Gate | Status | Evidence or blocker |
|---|---|---|
| 1: Source and environment | pass | Personal fork ancestry and pinned upstream checked; local Android Release library and GameInspector compiled; private paths ignored. |
| 2: Device and unmodified host | partial | Play 1.6.15.3 under released JunimoGate 0.3.1; disposable save cold-reloaded; displays mapped; originals backed up. Save-export defect open. |
| 3: Release host probe | pending | Exact references prepared; real runtime probe next. |
| 4: Lifecycle/input/frame ownership | blocked | Requires real release-host probe. |
| 5: Visual/data modes | blocked | Requires actual mobile metadata and working display path. |
| 6: Consequential actions | blocked | Requires disposable hosted farm and native mobile behavior. |
| 7: Physical acceptance/performance | blocked | Requires exact Release candidate on authorized Thor. |
| 8: Reviewed installed delivery | blocked | No working mobile Release artifact or device evidence yet. |

## Mandatory device matrix

| Area | Status | Observed result |
|---|---|---|
| Installation and identity | blocked | Not yet run on a shipping mod artifact. |
| Display and input | blocked | Not yet run on a shipping mod artifact. |
| Today | blocked | Not yet run on a shipping mod artifact. |
| Gifts | blocked | Not yet run on a shipping mod artifact. |
| People | blocked | Not yet run on a shipping mod artifact. |
| Bag and tooltips | blocked | Not yet run on a shipping mod artifact. |
| Quick stack | blocked | Not yet run on a shipping mod artifact. |
| Craft | blocked | Not yet run on a shipping mod artifact. |
| Aim | blocked | Not yet run on a shipping mod artifact. |
| Chest | blocked | Not yet run on a shipping mod artifact. |
| Shop | blocked | Not yet run on a shipping mod artifact. |
| Shipping | blocked | Not yet run on a shipping mod artifact. |
| Geodes | blocked | Not yet run on a shipping mod artifact. |
| Community Center | blocked | Not yet run on a shipping mod artifact. |
| Museum | blocked | Not yet run on a shipping mod artifact. |
| Forge | blocked | Not yet run on a shipping mod artifact. |
| Sewing/tailoring | blocked | Not yet run on a shipping mod artifact. |
| Dialogue and events | blocked | Not yet run on a shipping mod artifact. |
| Title and saves | blocked | Not yet run on a shipping mod artifact. |
| End of day | blocked | Not yet run on a shipping mod artifact. |
| Tabs/settings | blocked | Not yet run on a shipping mod artifact. |
| Lifecycle and hardware | blocked | Not yet run on a shipping mod artifact. |
| Release hygiene | blocked | Not yet run on a shipping mod artifact. |

## Local checks, not device acceptance

- Six Python helper boundary tests: pass. Initial failing run occurred before helper implementation.
- Scratch `net9.0-android35.0` Release library using framework Presentation and ImageView: build pass, zero warnings/errors.
- Pinned JunimoGate GameInspector Release build: pass, zero warnings/errors.
- JunimoGate v0.3.1 APK: publisher SHA256 matches downloaded bytes.
- Selected host references: managed identities inspected; SMAPI 4.5.2.0, MonoGame.Framework 1.0.0.0, JunimoGate.GameHost 1.0.0.0.
- Actual inventory invocation: fail, `device unauthorized`; no successful device properties obtained.
- Original Play app and saves: no authorized device access or modifications performed.

No cold launches, pause/resume cycles, display recreation, hardware input checks, transactions, save/reload journey, performance comparison, or soak have been run.
