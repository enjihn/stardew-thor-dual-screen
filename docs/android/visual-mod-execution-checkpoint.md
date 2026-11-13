# Visual mod integration checkpoint

Status: device integration accepted. Specification: ../superpowers/plans/2026-11-12-visual-mod-integration.md.
Branch android/visual-mod-integration starts at eb23b776fec16a01d5f1cc480ce74d2a2478dac3.

Current state: the user-idle baseline/combined Quality comparison and Beach
touch revalidation are complete. Quality passes mean/sustained FPS but fails
p95 in day and night/rain. The user now selected Low and accepts uneven frame
pacing, relaxing the strict p95 criterion without claiming it passed. Clean
Low day is59.9982FPS,minimum rolling5sec59.8FPS,p95=25.367449ms.
The user confirmed59.8 is fine and stopped further performance work. The Low
night run stopped midrun without a completed or failed result. Performance is
accepted under the user's changed criterion. The refreshed Low group is imported
and selected, with fixture-free direct-icon startup verified. The owner completed
the final physical controls/menu check and replied "Seems fine". Source delivery
is on the personal fork branch android/visual-mod-integration.
Prior potentially contaminated measurements remain recorded as history.

Root owns the device and coupled code. The dedicated existing checkout is
reused on the isolated requested branch, retaining private build tools.
At execution start, Thor57ae2a4f was authorized, at Cocoon launcher, with
no org.junimogate.thor process. Safe to prepare a separate test group.

Accepted rollback: ThorItemSwitchRelease, ThorDualScreen0.5.2-android.3,
build item-switch-android3-03, ZIP SHA256
6ff42e969a9a3fb27d95163b47b1ed1db4b720efe0cdec7a28e24fdd2559421e.
Do not replace or delete its mod/version/configuration.

Dependencies acquired from official author CurseForge files: CP2.9.1 file7759981,
GMCM1.16.0 file7122408. Original requested archives hashed and staged unchanged.
CP2.8.1 file7153624 is retained as the generic official fallback archive;
Nexus designates that version for Android, but Nexus byte equality is unverified.
CP2.9.1 manifest says SMAPI4.1.10 while the author file page says4.4.0;
the current host is newer than both. CP2.9.1 passed its runtime probe; the fallback was not needed.

ThorVisualModsTest imported with7 members. Initially only Thor and guarded
fixture0.23 are enabled for baseline measurement. The five new production
components are staged disabled, to be enabled incrementally. Thor configuration
is an isolated imported copy, preserving the owner's tab order. The accepted
group remains retained. Its complete export includes one mod and one config:
private accepted-group-rollback.zip, SHA256
58d5e95c72c13e746b810088710f9af2f9da398ca1c68e6f192fd197c1091ce4.

Fresh source checks: Python10/10, C#23/23 and both compatibility self-tests pass,
with existing C# warnings.
Cold test-group launch loaded only Thor0.5.2-android.3/build03 and fixture0.23.
Disposable save is now Spring21, ID453080632; all4 files backed up under
private ThorTest-before and hashed. Real saves were not accessed or changed.

Initial baseline completed: day59.9999FPS, night/rain59.9953FPS; presentation p95
16.919635ms/16.918802ms, rolling5sec minima59.8/59.6FPS. Both runs exceed120sec,
Aim active, same route and power/display/zoom settings. Raw samples retained.

CP2.9.1/GMCM1.16.0/Seasonal6.1.4 original releases load. Spring and winter
NPC art/portraits observed on top and lower People/Gifts. GMCM native Options
route supports touch settings, save and reopen. Defaults restored. Title cog
missed touches; native Options route is usable. Physical controller gate pending.
Winter test save was restored byte exactly before Fashion Sense testing.

Fashion Sense original7.5.0 failed startup at desktop-only CharacterCustomization
ResetComponents binding. Failed session stopped before a farm load. Pinned
source confirms Android MobileCustomizer replaces this optional new-farmer
checkbox. tools/FashionSenseAndroidPatch adds a platform type guard only to
that patch group, preserving the original released code for host rewrites.
Regression: unguarded Android failed, guarded Android bypass and desktop path
pass. Candidate7.5.0-android.1 SHA256
09f5ab859d8dff44d99eede1c3f6a9c76f6cde40ac461cf6808103676348642e.
Archive comparison proves all entries except DLL/manifest byte identical.
Source equivalent, pinned revision and GPL license are in compatibility/FashionSense.

Candidate initializes fully without errors and loads bundled presets. Registered
mirror used through Aim opens original UI. Body/sleeve/shoe presets, touch
recoloring, world movement, hoe use and native inventory preview work. ThorTest
slept to Spring22 and saved selected IDs/color. Cold reload preserved the appearance and color. Mirror clear controls restored
the original look, then both Spring21 saved files were restored byte exactly.
No real saves touched.

Radiance original2.4.8 failed optional type discovery because an unrelated SMAPI
Toolkit type requires an absent Pathoschild.Http.Client library. Candidate
2.4.8-android.2 uses exact optional named lookups and skips unresolved types in
three assignability discovery loops. Focused regressions pass; rendering logic,
shaders and assets remain unchanged. Failed prior sessions stopped before farm load.
Licensed candidate SHA256:
15680f5f7e90c69a090099e965e02b9b892293e50ec6c66d996bcf33eb163b31.
Source/build/license notes are in compatibility/Radiance.

Candidate android.2 completed startup and loaded ThorTest without mod errors.
Mountain52,10 shows water and player reflection on top; lower Aim retains native
art. GMCM and the Radiance tuner work by touch. Smooth Art is off. Android Low
was selected automatically. reliefreplay falls back to its original precision
after the driver's full-precision compile refusal.

Earlier day presentation runs all exceed120seconds. Baseline p95=16.919635ms;
the allowed20% bound is20.303562ms. Every combined run below passes mean and
rolling5sec FPS, but fails that p95 bound:

| Radiance state | Mean FPS | Minimum rolling5sec FPS | p95 ms |
|---|---:|---:|---:|
| Quality | 59.9812 | 59.6 | 25.364114 |
| Balanced | 59.9986 | 59.6 | 25.365521 |
| Low | 59.9840 | 59.6 | 25.364427 |
| Toggled OFF, still loaded | 59.9709 | 59.6 | 25.369167 |

Combined Quality night/rain:59.980936FPS, rolling5sec minimum59.4FPS,
p95=25.364323ms. Mean and rolling FPS pass; the original p95 bound still fails.
The first combined-radiance-unloaded-day run is invalid isolation: its startup
log still loaded Radiance, and the JSON is explicitly flagged. The valid unloaded2
run gives p95=25.355052ms; the run without both Radiance and Fashion Sense gives
25.357187ms. Neither identifies a mod as the cause.

Baseline rechecks cold-loaded only Thor and fixture, confirmed by startup's two
mods. Day:59.9926FPS, rolling5sec59.6FPS, p95=25.363802ms.
Night/rain:59.9988FPS, rolling5sec59.8FPS, p95=25.362083ms.
The user's reported concurrent movement limits those earlier comparisons. They
do not replace the clean baseline to declare a pass or establish mod causality.

Completed clean user-idle runs:

| Run | Mean FPS | Minimum rolling5sec FPS | p95 ms |
|---|---:|---:|---:|
| Baseline day | 59.9813 | 59.6 | 16.920730 |
| Combined Quality day | 59.9898 | 59.6 | 25.360052 |
| Baseline night/rain | 60.0029 | 59.8 | 16.916771 |
| Combined Quality night/rain | 59.9980 | 59.6 | 25.359479 |

All exceed120seconds. Quality conclusively fails the clean20% p95 bounds,
20.304876ms day and20.300125ms night, while mean and sustained FPS pass.
This supersedes contamination uncertainty for Quality without attributing the
change to one mod. The clean Low day run lasted120.937seconds,mean59.998163943FPS,
minimum rolling5sec59.8FPS,p95=25.367449ms. This shows no sustained drop into
the40s on the measured route, while the original approximately20.3ms p95 bound
still fails. The user selected Low and accepts the uneven pacing; this is an
explicit criterion change, not a pass of the original gate. The user requested
no further performance work; the follow-up Low night run stopped midrun and
is not counted as complete or failed. release-low-confirmed.png shows persisted
LowSpec, automatic scaling on and a100% ceiling.

Combined functional checks: root observed Radiance water/reflections, Farm tree
and object shadows by on/off comparison, Saloon night light pools, and night/rain
on top. Lower Aim night art matches baseline. All tabs draw; Bag's second-row
mirror at slot12 opens the actual HandMirrorMenu through Aim. Bundled presets
and recoloring render in world/Aim, and hoe use created dirt at Farm66,20.
Inventory works; mini row appears and expires; Home pause and direct resume
recover companion available=true.

Clean idle-touch-before/after/toolbar confirms Beach movement30,15 to16,14,
held slot0 unchanged, companion available=true, toolbar hidden=true, strip
hit=false, eventUp=false and no current event. Earlier Town scenery-blocked
and Beach Willy cutscene attempts are invalid. Real fallback showed available=false, hidden=false,
native visible=true and hit=true; a native bar click selected slot8. Recovery
showed available=true, hidden=true, native visible=false and hit=false.

Root owns the device. The refreshed fixture-free six-component
ThorVisualModsRelease-Low.zip is imported and selected:38054874bytes,
SHA2561ae18953214b5f974d2609938aa11720d8c0478d7f549b7c09e01b21d017da50.
Metadata is outputs/visual-mods/ThorVisualModsRelease-Low-package.json.
Exact freshly exported configs and their mod/hash mapping are in configuration-Low
and index.json. The earlier Quality package is superseded.

Selected Radiance settings:Enabled=true,LowSpec,RenderScale=1.0,Auto=true.
Smooth Art, object shadows, chromatic aberration, tilt-shift, God Rays, sun rays,
cloud shadows, sprite relief, fog, color grading, windy spells and wet ground
are off. Water reflection stays on with reach0.2; one shadow cast per character.
The actual Cocoon Stardew Dual Screen icon cold-launched the selected group;
startup confirmed five code mods and one content pack, no fixture.
Waiting after measurement advanced ThorTest to day22 and autosaved; all four
original test-save files were restored again before Low testing.
Only disposable ThorTest was cold-loaded for the owner check: Spring22 with a
Hand Mirror and five tools, saved natively. All four original backup files remain
retained; real saves were not accessed.
Final release-farm-top/bottom images show upper lighting, native lower Aim art,
Hand Mirror available in slot6 and companion available. The owner accepted movement, bumpers/triggers, tool use, Hand Mirror touch/controller
and Mod Options operation with "Seems fine". Leave the running game alone.
The original p95 gate remains failed; the user accepted that limitation, so it
does not block the accepted release. No mandatory device gate remains open under
the revised user instruction. On resume, do not repeat completed testing. The
source delivery commit contains this checkpoint.
Private summaries/raw evidence: artifacts/private/visual-mods/*-present-summary.json
and related files. Dependency packages and delivery files are in workspace
outputs/visual-mods, outside the source repository.
