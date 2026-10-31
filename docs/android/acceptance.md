# Android acceptance status

The unmodified mobile game baseline is verified. There is no accepted candidate or final mod artifact.

USB authorization is resolved. The original Play app and saves are preserved. New disposable hosted farm saved and cold-reloaded at Spring 2, 500g, 15 seeds. Save Export produced an empty file; diagnostic export succeeded. See execution-state.md.

## Gates

| Gate | Status | Evidence or blocker |
|---|---|---|
| 1: Source and environment | pass | Personal fork ancestry and pinned upstream checked; local Android Release library and GameInspector compiled; private paths ignored. |
| 2: Device and unmodified host | partial | Play 1.6.15.3 under released JunimoGate 0.3.1; disposable save cold-reloaded; displays mapped; originals backed up. Save-export defect open. |
| 3: Release host probe | partial | Release DLL loads, real assets render, lower taps and injected top key route pass; one pause/resume and vendor toggle pass. Physical controls remain pending. |
| 4: Lifecycle/input/frame ownership | partial | Ten pure coordination/selection tests pass; interrupted input, resize, resume and vendor cover tested on integration builds. Stress cases remain. |
| 5: Visual/data modes | partial | Shared panels render; mobile runtime bindings/assets validated. Full visual matrix remains. |
| 6: Consequential actions | partial | Native shop, chest, geode, craft, dialogue and bundle integration cases passed on development revisions. Persistence and remaining cases are pending. |
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

## Integration evidence, not final candidate acceptance

See execution-state.md for current artifact identities and private evidence names. Mobile4 shop sale, purchase, stock limit, canceled quantity, insufficient currency and full inventory checks passed through actual lower-screen input. Mobile5 fixed the native held-item read after a reproducible fixture snapshot crash. Chest transfer and geode failures were fixed and verified on mobile6. Dialogue and bundle fixes were verified on mobile7; museum release selection is next. A native profession choice passed on the disposable farm. Ten pure coordination/selection tests pass.

The matrix above remains unaccepted until tested on the exact shipping candidate. Earlier USB-blocked statements are superseded: authorization is resolved, original saves are independently backed up, and the released host runs the official mobile game. Final lifecycle counts, hardware controls, performance comparison, soak, save/reload action journey and release hygiene remain pending.
