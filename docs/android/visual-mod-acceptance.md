# Visual mod acceptance

Status: in progress. No combined release is accepted yet.
Specification: ../superpowers/plans/2026-11-12-visual-mod-integration.md.
Device: AYN Thor57ae2a4f, Android13/API33 ARM64. Runtime: Play1.6.15.3/code245,
JunimoGate0.3.1-thor.2/code12, SMAPI4.5.2-junimogate.41,
Harmony2.4.2-junimogate.64, ThorDualScreen0.5.2-android.3/build03.

The user reported moving the character during tests. Prior performance and
movement/touch attribution may be contaminated, so clean matched performance
and touch reruns are being coordinated with the user idle. Retain the actual
measurements and visual outcomes below; source checks and captured functional
images remain valid. No final performance-blocked conclusion is made yet.

## Preservation and packages

The accepted ThorItemSwitchRelease is retained. Its complete local export
includes the original configuration. ThorTest453080632 is the only test farm;
its Spring21 save pair and backups were copied privately before new mod tests.
Package hashes: visual-mod-packages.json. Dependencies came from author-owned
CurseForge pages, recorded in the private dependency-provenance.json.
Thor production source, game assets, host and direct icon are unchanged. Fashion Sense has the narrow compatibility guard documented below.

## Gates

| Gate | Status |
|---|---|
| Existing coordination and packaging tests | Fresh Python10/10, C#23/23 and both compatibility self-tests pass |
| Seasonal sprites, portraits, seasons, lower People/Gifts | Passed spring/winter direct checks |
| Fashion Sense mirror, presets, recoloring, tools, previews, persistence | Passed with narrow Android guard; physical controls pending |
| Radiance effects, shaders, hooks, settings, interiors/rain/resume | Direct water/reflection, shadow, interior light, night/rain and touch settings checks passed; precision fallback noted |
| Combined companion tabs, mini row, world touches, fallback/recovery | Visual/UI outcomes observed; coordinated movement/touch recheck pending |
| Matched presentation performance | Prior runs potentially contaminated by user movement; clean coordinated reruns pending |
| Owner physical controls and new menus | Pending |
| Fixture-free six-component release and direct launch | Pending |

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
so no patch is justified for task completion. Controller check remains pending.

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

The open-Beach strip test, combined-strip-real-before/after, observed movement
from30,15 to16,14 while held slot20 stayed unchanged and strip hit=false.
Earlier Town touches blocked by scenery and the earlier Beach Willy cutscene
attempt are invalid evidence. The open-Beach input attribution will be
rechecked with the user idle.

Real companion failure produced available=false, toolbar hidden=false, native
visible=true and native hit=true. Clicking the native bar selected slot8.
Recovery produced available=true, hidden=true, native visible=false and hit=false.
Owner physical controller/menu navigation remains pending after automated
acceptance. No physical-control pass is claimed.

## Current performance result

All four combined day runs exceed120seconds, with the same Town touch route,
Aim, zoom, display and power settings as baseline. Mean and minimum rolling5sec
FPS pass, but p95 exceeds the baseline16.919635ms plus20% bound20.303562ms.
No tested Radiance setting has passed the combined day performance gate.

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

Source: baseline-recheck-day/night present-summary.json. The later baseline
does not support attributing the pacing difference to the added mods. The user's
reported concurrent character movement materially limits the earlier matched
comparisons. Performance remains unresolved until the clean coordinated reruns.
The original16.92ms baseline and its20% bound remain recorded; rebasing to later
results does not constitute a pass. No speculative compatibility or performance
patch is justified by the existing measurements.

Root is preparing the configured test-group export and fixture-free six-component
candidate/local package, then accepted-group reselection and original test-save
restoration. Those actions are not yet recorded as complete. The performance
gate, coordinated touch recheck, owner physical controls and final fixture-free
release checks remain open.
