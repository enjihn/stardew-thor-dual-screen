# Android Release device acceptance

Status: installed candidate, physical controls and final soak review pending.
No completion claim is made until those mandatory checks pass.

## Artifact and runtime

| Item | Exact tested identity |
|---|---|
| Mod | 0.5.2-android.1, Release |
| Source and embedded build ID | 22cf243112b710381db6db8b89dc01882958a67f |
| ZIP SHA256 | 250ccf42543906951ef68cc33305e25991c4815844c8a4f7d060595b34cd322a |
| Final selected group | ThorRelease, one enabled mod, no fixture |
| Official Play game | com.chucklefish.stardewvalley 1.6.15.3, code 245, ARM64, installer com.android.vending |
| Host | Publisher JunimoGate 0.3.1, code 11, org.junimogate.app, unchanged |
| Host source | 029eafed917ae853d6c0a59330ee273524168296 |
| Host APK SHA256 | 49bcce760ceda4b9080237000f713cfbd9485e5b237be4c38ed8b95b522295b4 |
| SMAPI | 4.5.2-junimogate.41, source 9471959851a75dfa1f2648016b78fc171a5a1999 |
| Device | AYN Thor, Android 13/API 33, arm64-v8a |
| Firmware | qti/kalama/kalama:13/TKQ1.231222.001/eng.Thor.20260206.163241:user/release-keys |
| Displays | Top logical 0, 1920x1080; companion logical 4, 1240x1080; native density 369 |
| Test power | Standard preset 0, fan setting 1 (native Smart label), 120Hz, battery saver off |

The final fan setting and both matched timing records read `system/fan_mode=1`.
Earlier prose calling Smart mode 4 was a transcription error, not a recorded
setting change. No renderer experiments or host patches are in this artifact.

## Acceptance matrix

The integration details and exact private evidence basenames are retained in
[acceptance.md](acceptance.md). The table below is the current verdict. Earlier
passing cases carry forward only where the relevant production implementation
is unchanged. The shipping artifact received affected-case checks and the final
fixture-free journey. Fixture preparation/observation is labeled separately;
production actions used actual Android input on the physical device.

| Required area | Result | Direct observation |
|---|---|---|
| Today | pass | Luck/events/birthdays match native state; watering 2 dry to 1, harvest 1 ready to 0; ginger excluded; all event rows fit. |
| Gifts and People | pass | Native gift consumed once, friendship 1000 to 1100, day/week counts 1; meeting/bundle cache refresh; known NPCs, map, birthdays and portraits. |
| Bag | pass | Merge/drag/trash/cancel/full-bag collection conserve expected items and money; final fixture-free drag survives cold reload in slot 1. |
| Quick stack | pass | Current-location chest, Junimo and fridge scope; capacity-limited remainders, held/unmatched items preserved. |
| Craft | pass | Single/multiple outputs, ingredients, capacity and refusal; Scarecrow quest 8, recipe count and ItemsCrafted persist. |
| Aim | pass | Native lighting/rain/indoor/mine/night, zoom, watering direction and valid/invalid placement; chair rotation and metadata; primary renderer intact. |
| Chest | pass | Full/partial transfers, quantity, fill, organize, page 2 and rapid input conserve stacks/metadata; stale gesture after native Organize refused; fresh take works. |
| Shop | pass | Native sale price/quality, buy 1/5/25, stock limits, number pad, alternate currency, poor/full/cancel; controller-mode sale retains native controller flags. Final candidate 500 to 604 to 584g with one seed, persisted. |
| Shipping | pass | Exact selected amounts, retrieval/re-shipping, tool refusal; 631+681=1312g once, persisted after cold load. |
| Geodes | pass | One geode and 25g produce five coal and one statistic increment; poor/full/non-geode refused; persisted. |
| Community Center | pass | Locked text hidden, correct quality/quantity, rejected item preserved, Spring Crops reward 20 SpeedGro once, saved progression. |
| Museum | pass | Five donations, rejection/duplicate/cancel conservation; nine seeds and Dwarvish translation reward persist. |
| Forge | pass | Valid/invalid/cancel, ten-shard emerald forge, five-shard unforge refund; weapon marker preserved. |
| Tailoring | pass | Cloth/parsnip creates shirt 1159; cancel/full refusal; ruby dye changes native color while retaining marked pants. |
| Dialogue/events | pass | Yes/No callbacks, stale answer refused, supported skip only, festival tabs. |
| Title/saves | pass | Native New/back, five-entry paging, disposable import/cold restore, safe exit, five normal top cold launches plus one missing-companion launch. |
| End of day | pass | One-time income/date, all 30 profession choices and native health perks, saved progression. |
| Tabs/settings | pass | Select/hide/reorder/persist, resize and touch mapping; defaults restored. |
| Lifecycle | pass | Exact candidate ten pause/resume and five vendor lower-cover/restore cycles across title, game, inventory, Aim and chest; fresh input and focus restored. |
| Missing companion | pass | Native game launched on logical 4, leaving no eligible companion display; load/movement/menu/exit work, no Presentation created. Normal top launch restored. |
| Hardware controls | pending | Real buttons, sticks and both touch panels require one grouped physical action check. Injected keys do not qualify. |
| Performance/response | pass with measured limits | Matched presentation p95 ratio 1.000321; response p95 144.333ms. Tail delays and method limits below. |
| Thirty-minute mixed soak | pending | Bounded UID-wide capture spans movement, panels, Aim, transactions, pause, save, exit and six cold starts. Final duration/log review pending. |
| Preservation | pass | Final three Play APK hashes and three original save-file hashes match the initial manifests; original package/version/installer unchanged. |
| Release hygiene | pass | Two clean byte-identical five-entry ZIPs; no game/framework DLLs, saves, APKs or fixture. Exact build ID and one-mod group observed in cold launches 05 and 06. |

The supported vendor operation covers/restores the lower screen and touch. It
is not physical display removal or Activity recreation. Task 7 explicitly calls
for repetitions of the actual supported toggle/recreation operation. Ordinary
pause now reuses the same hidden Presentation to avoid native peer accumulation.
The candidate held 15 views, two roots, one Activity, six or seven contexts and
28 settling to 27 local binders across ten cycles. Interrupted gestures were
discarded and fresh gestures worked. UI-stall testing matched the disabled
baseline and recovered without queued stale work.

Native capabilities absent from this mobile route: desktop co-op entry and
shop touch-sale buyback. Both were established from the native baseline/source;
they are not claimed as implemented features. Multiplayer lock contention and
additional mod combinations were not qualified.

## Timing and limitations

The matched farm route used Standard mode, the same save/route/graphics/cap,
warmup, and the primary game SurfaceView's SurfaceFlinger FrameTracker actual
presentation timestamps. The exact candidate recorded 7,370 intervals over
123.18 seconds: mean 59.8296 FPS, p95 25.352969ms, p99 33.797970ms, max
42.259375ms. Disabled p95 was 25.344844ms, mean 59.9982 FPS. The p95 ratio is
1.000321, below the 1.20 bound. A five-second window dipped to 58.17 FPS;
there is no claim of perfect 60 FPS or unchanged tail jitter.

Earlier SMAPI Rendered callback cadence showed p95 ratio 1.4266. That is retained
as a callback-jitter result, not relabeled as presentation timing. Renderer
experiments did not improve it and were removed. Primary presentation is the
claim-relevant final measurement; Android UI gfxinfo was not used as a game FPS
proxy. Method references: [AOSP FrameTracker](https://android.googlesource.com/platform/frameworks/native/+/cb31cabec0411d3647be742d9be0605a4dcf6622/services/surfaceflinger/FrameTracker.cpp)
and [Android FrameStats](https://developer.android.com/reference/android/view/FrameStats).

All 24 shipping-artifact response samples are retained: median 91.333ms,
p95 144.333ms, max 203.333ms; 23 of 24 are below 150ms. The plan's ordinary
response target is assessed at p95, not as an unstated hard maximum. Method:
native lower TouchUp event to first matching bitmap frame commit plus one
120Hz refresh estimate. It is not optical photon latency. The observer was
detached after testing and is absent from ThorRelease. The isolated 203ms delay
has no established cause and remains a limitation.

JunimoGate 0.3.1 Save Export produced a zero-byte file. It is not a usable backup.
Disposable Save Import/cold restore passed with an independently CRC/XML-checked
ZIP. No real save was imported. The original Play app and real saves are intact.

## Final fixture-free journey and evidence

Only the shipping mod was enabled. Native lower Bag drag moved the sole parsnip
seed from slot 0 to slot 1. Native bed dialogue advanced Spring 15 to Spring 16
with 584g. Exit to title, host process restart, native Load and selection restored
Spring 16, 584g and the seed in slot 1. Both screens rendered; startup reported
one loaded mod with the exact source build ID.

Private evidence basenames: `release-final-members-verified.png`,
`release-cold05-fixturefree-startup.txt`, `release-fixturefree-bag-moved.png`,
`release-fixturefree-day16-top.png`, `release-cold06-fixturefree-startup.txt`,
`release-cold06-fixturefree-top.png`, `release-cold06-fixturefree-bag.png`,
`release-originals-preserved.json`, `release-restored-settings.json`,
`release-pause-summary.json`, `release-cover01-title` through
`release-cover05-chest`, `release-present-enabled-present-summary.json`,
`release-response-summary.json`, and `release-22cf-soak/`.

Raw logs, save backups, reference assemblies and device identifiers stay local
under ignored `artifacts/private/` or `.local/`. Sanitized summaries and source
are publishable. Installation and rollback: [INSTALL.md](../../android/INSTALL.md).
