# Visual mod acceptance

Status: accepted. Low release installed and selected; owner physical check passed.
Specification: ../superpowers/plans/2026-11-12-visual-mod-integration.md.
Device: AYN Thor57ae2a4f, Android13/API33 ARM64. Runtime: Play1.6.15.3/code245,
JunimoGate0.3.1-thor.2/code12, SMAPI4.5.2-junimogate.41,
Harmony2.4.2-junimogate.64, ThorDualScreen0.5.2-android.3/build03.

The user selected Low and relaxed the strict frame-pacing criterion: "Use the
low mode, the fps doesn't have to be perfectly60." The clean Low day run averages
59.9982FPS with a minimum rolling5sec59.8FPS. The user confirmed59.8 is fine and
requested an end to further performance work. The p95 limitation is accepted
under that changed criterion and is not a release blocker. Earlier results
remain recorded. The Low package/configuration and fixture-free cold launch are
complete. The owner completed the physical check and replied "Seems fine".

## Preservation and packages

The accepted ThorItemSwitchRelease is retained. Its complete local export
includes the original configuration. ThorTest453080632 is the only test farm;
its Spring21 save pair and backups were copied privately before new mod tests.
Package hashes: visual-mod-packages.json. Dependencies came from author-owned
CurseForge pages, recorded in the private dependency-provenance.json.
Thor production source, game assets, host and direct icon are unchanged. Fashion Sense has the narrow compatibility guard documented below.

The final fixture-free six-component ThorVisualModsRelease-Low.zip is imported
and selected:38054874bytes, SHA256
1ae18953214b5f974d2609938aa11720d8c0478d7f549b7c09e01b21d017da50.
Metadata is in outputs/visual-mods/ThorVisualModsRelease-Low-package.json.
Freshly exported exact settings are in configuration-Low/index.json, with mod
identity, config size and SHA256. The earlier Quality ZIP is superseded.
After measurement, waiting advanced ThorTest to day22 and autosaved. All four
original test-save files were restored again before Low testing. Real saves
were not changed.
For the owner check, only ThorTest was cold-loaded: a native
Spring22 save with a Hand Mirror and five tools. All four original backup files
remain retained. No real save was accessed.

## Gates

| Gate | Status |
|---|---|
| Existing coordination and packaging tests | Fresh Python10/10, C#23/23 and both compatibility self-tests pass |
| Seasonal sprites, portraits, seasons, lower People/Gifts | Passed spring/winter direct checks |
| Fashion Sense mirror, presets, recoloring, tools, previews, persistence | Passed with narrow Android guard and owner physical check |
| Radiance effects, shaders, hooks, settings, interiors/rain/resume | Direct water/reflection, shadow, interior light, night/rain and touch settings checks passed; precision fallback noted |
| Combined companion tabs, mini row, world touches, fallback/recovery | Direct checks passed; user-idle Beach touch revalidation passed |
| Matched presentation performance | User accepted Low59.9982FPS/min5sec59.8 and uneven pacing; original p95 failure retained; further performance work stopped at user request |
| Owner physical controls and new menus | Passed: owner replied "Seems fine" after movement, bumpers/triggers, tools, Hand Mirror touch/controller and Mod Options check |
| Fixture-free six-component release and direct launch | Low group imported/selected; actual Cocoon icon cold launch confirms five code mods plus one content pack, no fixture |

Performance uses completed primary game SurfaceView presentation timestamps
from SurfaceFlinger FrameTracker, not callback FPS. Same Town route, Aim active,
original zoom1.2972656, unchanged Standard power mode0, fan mode4, 120Hz,
battery saver0. Each scene runs at least120seconds after warmup.

Baseline day:120.384seconds,59.9999FPS,p95=16.919635ms,minimum rolling5sec59.8FPS.
Baseline night/rain:121.543seconds,59.9953FPS,p95=16.918802ms,minimum rolling5sec59.6FPS.
Raw timestamps and scene screenshots are retained privately. Both runs used
only the accepted Thor mod plus the guarded fixture; new mods were disabled.

Original CP2.9.1 and GMCM1.16.0 load successfully. Seasonal6.1.4 spring
portraits visibly replace Pierre/Emily on Today and Robin on dialogue, People
and Gifts; Robin/Demetrius sprites render. No content errors observed.
GMCM in-game Options > Mod Options opens by touch. Beach Outfits was toggled
off, saved, reopened as off, then restored on and saved. No keyboard required.
Title cog did not respond to touchscreen tests; the native Options route works,
so no patch is justified for task completion. The owner subsequently accepted the controller/menu check.

Winter21 ThorTest load showed Robin in a red winter outfit on the native
dialogue portrait and lower People panel, with changed NPC sprite clothing.
The spring and winter images are retained. The test session was stopped,
and both original Spring21 save files restored with matching SHA256s before
Fashion Sense testing. Seasonal supplied art and defaults remain intact.

## Fashion Sense compatibility and direct checks

Original7.5.0 fails in CharacterCustomizationPatch.Apply because the desktop
ResetComponents target is absent on Android. That partially initialized session
was stopped before loading a farm. Pinned source39e9e9e13c9f9a78416172e73a229cf8703ddb59
confirms all four hooks in that group belong to the optional desktop new-farmer
mirror checkbox. The7.5.0-android.1 guard skips this group when MobileCustomizer
exists. Other released methods and all artwork remain intact; JunimoGate still
applies its usual mobile signature rewrites. No extra Harmony is bundled.

The regression failed on the unguarded Android case, then passed for guarded
Android and unchanged desktop behavior. Archive comparison found only DLL and
manifest differences. See compatibility/FashionSense for license/source/build notes.

Device startup then completed: content tokens and GMCM registered, all local
body/sleeve/shoe presets loaded, no startup errors. A registered Hand Mirror was
seeded only into ThorTest, selected in Bag/Aim and used by touching the Aim world.
Its original menu opens. Selected local body Default(Short), sleeves Default(Tall)
and shoes Default(Short), then recolored shoes with touch sliders. Native inventory
and world previews render the modified appearance. World touch movement works;
Aim-selected hoe creates a new tilled tile. No Seasonal farmer conflict was observed,
so no Seasonal defaults were disabled.

ThorTest slept to Spring22. Its native saved XML contains the exact local preset
IDs and shoe color4294341530. Cold reload shows the same colored shoes and body
in the mirror preview. The mirror trash control cleared body/sleeves/shoes and
restored the original appearance. Both original Spring21 saved files were then
restored with matching pretest SHA256s. Private images/logs are prefixed fs1 and
fashion-patch1. Numeric color boxes use Android's larger textbox drawing and
partly overlap, but all three touch sliders and tested selection/clear/exit controls
are usable without a keyboard. Physical menu navigation remains for the owner gate.

## Radiance startup compatibility

Original2.4.8 selected Android Low automatically but failed GameLaunched while
Harmony searched all loaded types for optional Scale Up. An unrelated SMAPI
Toolkit type requires Pathoschild.Http.Client, absent on this host. Exact named
lookups replace that scan for Scale Up and DynamicShader. The next live probe
then exposed lazy base-type resolution failing at IsAssignableFrom during location
draw discovery. The same three startup discovery loops now skip unresolved types
while preserving valid assignability results. No rendering logic changed yet.

Candidate2.4.8-android.2 completes GameLaunched, GMCM registration, water labels
(263 sheets/45684 tiles), water hook(4 methods), later location/character hooks
and render-clock setup. A small own resolver DLL is included, with its GPL notice;
no additional Harmony or game assembly. Original MIT license and all assets remain.
Most shaders use the Android full-precision path; reliefreplay reports driver
compile failure at that precision and loads its original compiled precision.

On ThorTest, water and player reflection are visibly active at the mountain lake.
The lower Aim water retains the ordinary pixel-art rendering. GMCM opens through
native Options and its Hotkeys page opens the tuner after closing the settings
menus. Tuner touch navigation and quality selection work. Smooth Art checkbox
was verified off; Android logs confirm sharp zoom stands aside. Initial Low was
confirmed selected in the tuner; highest Quality selected for the first matched
performance run. No zoom, display or power settings changed.

## Combined direct checks

Root visually confirmed water/reflections, Farm tree/object shadows by on/off
comparison, Saloon night light pools and top-screen night/rain. Lower Aim's night
image matches baseline. All companion tabs draw. A mirror selected from Bag's
second row at slot12 opens the actual HandMirrorMenu through Aim with all mods
loaded. Bundled appearance presets and recoloring render in world/Aim, and hoe
use created dirt at Farm66,20. Inventory works, the mini row appears and expires,
and Home pause followed by direct resume leaves the companion available.

The clean user-idle Beach rerun, idle-touch-before/after/toolbar, moved30,15 to16,14.
Held slot0 stayed unchanged, companion available=true and toolbar hidden=true,
strip hit=false, eventUp=false and currentEvent=null. This validates world touch
through the passive strip. Earlier slot20 observations are retained in private
evidence; Town touches blocked by scenery and the Beach Willy cutscene attempt
are invalid evidence.

Real companion failure produced available=false, toolbar hidden=false, native
visible=true and native hit=true. Clicking the native bar selected slot8.
Recovery produced available=true, hidden=true, native visible=false and hit=false.
The owner completed the requested movement, bumpers/triggers, tool use, Hand Mirror
touch/controller and Options > Mod Options check on the final fixture-free
release and replied "Seems fine". This is owner-observed physical acceptance.

## Selected Low configuration and launch

The refreshed export confirms Radiance Enabled=true, ActivePerfPreset=LowSpec,
RenderScale=1.0 and RenderScaleAuto=true. Smooth Art, object shadows, chromatic
aberration, tilt-shift, God Rays, sun rays, cloud shadows, sprite relief, fog,
color grading, windy spells and wet ground are disabled. WaterReflection=true,
WaterReflectReach=0.2 and ShadowCastsPerCharacter=1. Directional character
shadows remain enabled. Earlier Farm object-shadow visual checks used the
previous configuration.

Root imported and selected the six-component Low group, then cold-launched the
actual Cocoon Stardew Dual Screen icon. Startup confirmed five code mods and
one content pack with no fixture. This verifies selected-group/direct-icon
startup; the owner subsequently accepted the physical check.
Final release-farm-top/bottom images show ThorTest Spring22, upper lighting,
lower Aim native art, Hand Mirror available in slot6 and companion available.

## Current performance result

The coordinated user-idle baseline and combined Quality runs are complete.
Each exceeds120seconds with the matched Town route, Aim, zoom, power and display
settings. Quality's approximately25.36ms p95 exceeds the clean baseline plus20%
bounds:20.304876ms day and20.300125ms night/rain. Mean and sustained FPS pass.
This is a controlled Quality failure; it does not identify an individual mod
as the cause. The user subsequently selected Low with the criterion below.

| Clean run | Duration seconds | Mean FPS | Minimum rolling5sec FPS | p95 ms |
|---|---:|---:|---:|---:|
| Baseline day | 120.271 | 59.9813 | 59.6 | 16.920730 |
| Quality combined day | 120.471 | 59.9898 | 59.6 | 25.360052 |
| Baseline night/rain | 120.161 | 60.0029 | 59.8 | 16.916771 |
| Quality combined night/rain | 121.387 | 59.9980 | 59.6 | 25.359479 |

Source: idle-baseline-day/night and idle-combined-day/night present-summary.json.
These clean measurements supersede the prior contamination uncertainty for the
Quality comparison. They do not pass the original allowance.

The completed clean Low day run, idle-low-day-present-summary.json, lasted
120.937seconds:mean59.998163943FPS,minimum rolling5sec59.8FPS,p95=25.367449ms.
There was no sustained drop into the40s in this measured route; individual
frames still arrive unevenly. The original approximately20.3ms p95 allowance
fails. The user explicitly accepts that uneven pacing and chose Low for the
release, relaxing the strict gate rather than changing its recorded result.
The follow-up Low night benchmark was stopped midrun at the user's request;
it is neither a completed result nor a failure. The existing Quality night run
remains the completed night measurement. No further performance tests are
required by the user. The latest release-low-confirmed.png shows persisted
LowSpec with automatic scaling on and a100% ceiling.

### Earlier measurements

The following results preceded the coordinated idle window and remain recorded
without establishing a final verdict for Low or a causal attribution.

| Radiance state | Duration seconds | Mean FPS | Minimum rolling5sec FPS | p95 ms |
|---|---:|---:|---:|---:|
| Quality | 120.354 | 59.9812 | 59.6 | 25.364114 |
| Balanced | 120.153 | 59.9986 | 59.6 | 25.365521 |
| Low | 120.249 | 59.9840 | 59.6 | 25.364427 |
| Toggled OFF, still loaded | 120.525 | 59.9709 | 59.6 | 25.369167 |

Source: private combined-quality-day, combined-balanced-day, combined-low-day
and combined-radiance-off-day present-summary.json files. Their raw timestamps
are retained. Day Aim screenshots show the same native art as baseline.

Quality night/rain:121.138seconds,59.980936FPS,minimum rolling5sec59.4FPS,
p95=25.364323ms. Mean and sustained FPS pass; p95 fails the original night
baseline plus20% allowance.

Toggled OFF still means loaded. The first combined-radiance-unloaded-day summary
is also invalid isolation: startup still loaded Radiance, and its JSON now states
isolationValid=false. The valid combined-radiance-unloaded2-day run recorded
p95=25.355052ms. With both Radiance and Fashion Sense unloaded,
seasonal-dependencies-day recorded25.357187ms. These do not establish causality.

Cold baseline rechecks verified only Thor and fixture as the two loaded mods:

| Scene | Duration seconds | Mean FPS | Minimum rolling5sec FPS | p95 ms |
|---|---:|---:|---:|---:|
| Day | 121.165 | 59.9926 | 59.6 | 25.363802 |
| Night/rain | 120.952 | 59.9988 | 59.8 | 25.362083 |

Source: baseline-recheck-day/night present-summary.json. The user's reported
concurrent movement limits those earlier comparisons. They do not replace the
clean baseline to declare a pass, and no speculative patch follows from them.

The selected Low package, exact configs and fixture-free direct-icon cold launch
and owner physical controls are complete. Source is delivered on the personal
fork branch android/visual-mod-integration; the delivery commit contains this report.
ThorItemSwitchRelease is retained as the separate accepted rollback.
