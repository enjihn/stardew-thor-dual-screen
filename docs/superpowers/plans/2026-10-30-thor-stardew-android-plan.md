# Stardew Valley Android on AYN Thor Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task by task. Steps use checkbox (`- [ ]`) syntax for tracking. One primary worker owns the coupled game changes and the USB device. Use independent agents for bounded research and code/evidence review when available.

**Goal:** Fork Thor Dual Screen, adapt it to the user's installed Google Play Android edition of Stardew Valley, and deliver an installed, tested, reproducible dual-screen mod on the user's USB-connected AYN Thor.

**Architecture:** Keep the upstream panels and their use of the running game's artwork. Load the mobile mod through a compatible Android SMAPI host, initially the released JunimoGate launcher, and display the companion UI through an Android `Presentation`. Add only the mobile compatibility, input, lifecycle, and frame-delivery changes demonstrated necessary by builds and device tests.

**Tech Stack:** C#, the actual installed Android Stardew assemblies, JunimoGate's matching SMAPI and MonoGame assemblies, .NET 9 Android for the initial host baseline, Android SDK/ADB, Git/GitHub CLI, and two small Python build/device helpers.

**Spec:** The embedded Design Contract and Acceptance Matrix in this document are the specification. Read this entire document before executing. Intended repository location: `docs/superpowers/plans/2026-10-30-thor-stardew-android-plan.md`.

**Prepared:** 2026-10-30. This is an execution plan, not a claim that the port has been built or tested.

## Global Constraints

- Target the user's official Google Play installation. Discover its actual version and package paths. Do not substitute the PC release, Cinderbox, an emulator, a third-party game APK, or an unrequested downgrade.
- Run execution on the computer that can access the Thor through USB ADB. A cloud Codex environment requires working, authorized USB pass-through before it can perform device tests.
- Keep the original Play installation and original saves intact. Test on a new disposable farm or an independently verified copy. Never clear application data, uninstall an existing game/host to fix a signing conflict, root the device, or bypass Android access controls.
- The modded game will launch through the compatible Android host. Launching the ordinary Play game icon does not install or activate this SMAPI mod.
- Reuse upstream code and layout, game textures, fonts, item/NPC artwork, and compatible installed content assets. Do not redraw the UI with substitute artwork or redistribute the game's assets or assemblies.
- Prefer one released host, one mod DLL/ZIP, one mobile target, and the existing bitmap transport. No companion service, streaming protocol, generalized plugin framework, or separate bottom-screen app.
- Preserve upstream attribution and GPL license obligations. Keep private game files, saves, device identifiers, raw logs, credentials, and reference DLLs out of Git and public releases.
- Retain the original desktop project. Add a separate mobile project that shares the existing source, instead of introducing a second copy of `Panels.cs`.
- Discover display identity and input/capture IDs. Do not assume display 0/1, hardcode a Thor serial, or use one display-ID namespace for both input and screenshots.
- Release acceptance requires the exact shipping artifact on the physical Thor. Compilation, mocked tests, emulator results, screenshots alone, or a diagnostic-only build cannot establish completion.
- Work autonomously through ordinary edits, dependency setup within the project, builds, fixes, installs, test-save operations, commits, and source pushes. Do not repeatedly request approval for these steps after the kickoff instruction authorizes them.
- Honor real access/approval controls. Request human action only for an actual blocker such as USB authorization, account authentication, inaccessible hardware controls, or an operation outside the approved scope. Continue all unaffected work first.

## Review Focus

1. A delayed touch after changing menu, save, layout, or display must not act on a new item or menu. Task 4 tests generation invalidation and Task 6 verifies it through Android input.
2. Full inventories, partial stacks, price rules, callbacks, and failed transactions must not lose/duplicate items or money. Task 6 verifies native menu behavior and persisted state.
3. A paused/recreated Activity or disappearing lower display must not leak resources, queue stale work, or steal controller focus. Tasks 4 and 7 test this on the device.
4. Runtime assembly differences and Java peer registration must fail visibly before harmful actions, not be papered over by bundling duplicate framework DLLs. Task 3 proves the actual release-host loading path.
5. Rendering Aim or a slowly consumed frame must not corrupt the main screen, reuse a buffer in flight, or allow an unbounded UI queue. Tasks 4 and 7 exercise graphics restoration, ownership, and frame timing.

## Design Contract

### What the finished result must do

The top screen runs the Android game with normal controller behavior. The lower screen provides the upstream companion interface: Today, Gifts, People, Bag, Craft, Aim, contextual menus, dialogue, title/load flows, and end-of-day interactions. It remains usable across save loading, background/resume, and supported display recreation. Menu actions obey the mobile game's actual rules, including callbacks, pricing, inventory capacity, progression, and save persistence.

"Same assets" means using the assets resolved by the running mobile game, including compatible content modifications that are actually installed. The upstream renderer already draws with game textures and fonts; it does not contain a separate replacement art pack. Asset keys, sprite regions, and map APIs must be checked against mobile. Arbitrary unrelated PC mods are not automatically Android-compatible. [S1][S2]

Use the existing logical 620 by 540 panel layout initially. Measure the real lower-screen drawable area and input bounds. Preserve its intended appearance, with touch mapping using the same transform as rendering. Do not assume the whole display remains available after insets or rotation. [S2]

### Verified starting points, not immutable future assumptions

| Component | Audited starting point | Execution rule |
|---|---|---|
| Upstream mod | Version 0.5.2; commit `15f05b63d3ca852f3729e20de67fc7446bfac112`; desktop target `net10.0` | Inspect current upstream, then pin the selected commit. Record any change from this audit. |
| Play game | Package `com.chucklefish.stardewvalley`; documented host baseline 1.6.15.3 / versionCode 245 / ARM64 | The connected device is authoritative. Do not require historical split counts or overwrite a newer installation. |
| Preferred host | JunimoGate v0.3.1; commit `029eafed917ae853d6c0a59330ee273524168296`; package `org.junimogate.app` | Prefer this known released baseline if compatible. If using a newer release, revalidate and pin it before porting. |
| Host artifact | `JunimoGate-0.3.1-arm64.apk`; published SHA256 `49bcce760ceda4b9080237000f713cfbd9485e5b237be4c38ed8b95b522295b4` | Verify downloaded bytes against the corresponding publisher release. This hash applies only to that exact asset. |
| Mobile compile target | JunimoGate's sample uses `net9.0-android35.0`; host minimum API 26 | Match the selected host's target/runtime. Build a library, not a new launcher APK, in the normal path. |

Sources: upstream source and manifest [S1][S2]; JunimoGate release, mobile discovery documentation, application and sample projects [S3][S4][S5].

JunimoGate loads mods and game assemblies into a shared default assembly load context. The active mod selection is frozen for a game launch. Importing a new revision requires selecting it and restarting the game process; copying a ZIP to Downloads or replacing a DLL is insufficient. Its mod library and runtime live in application-private storage. [S6]

### Scope and execution authority

The kickoff prompt authorizes a personal GitHub fork or reuse of an existing personal fork; an isolated task branch; local tool setup; installing a compatible released host if needed; importing and enabling the mod; work on disposable/copied saves; and pushing the finished source branch to that fork. It does not authorize deleting unrelated work, force-pushing, merging to upstream, publishing someone else's game files, contacting maintainers, or overwriting real saves.

If GitHub or USB is temporarily blocked, continue source preparation, builds, and other available checks. Keep the corresponding acceptance rows blocked. A permission prompt or an absent device cannot be solved by retrying indefinitely or inventing successful evidence.

### Minimal file map

Paths below are relative to the fork. Add a file only when its owning task needs it.

| Path | Responsibility |
|---|---|
| `mod/DualScreen.csproj` | Existing desktop target; preserve its intended behavior. |
| `android/DualScreen.Android.csproj` | Mobile library target with linked shared source, explicit local references, build identity, and `THOR_ANDROID` conditional symbol. Its own directory isolates `obj`/`bin` from the desktop project. |
| `android/manifest.json` | Android mod manifest, emitted as `manifest.json` inside the release ZIP. |
| `mod/ModEntry.cs` | Existing SMAPI entry point, rendering and event integration; make narrow mobile changes. |
| `mod/MobileHost.cs` | Actual game Activity/display discovery and mobile host integration. |
| `mod/MobileGameAdapter.cs` | Version-dependent menu/API differences, resolved before use. No duplicate inventory engine. |
| `mod/CompanionState.cs` | Small, platform-independent generation/input and latest-frame ownership logic needed by production code. |
| `mod/Panels.cs`, `mod/CropTracker.cs` | Shared UI and data logic; targeted adaptations, no wholesale restructure. |
| `tools/build_android.py` | Validate references, invoke Release build, create an allowlisted mod ZIP, report SHA256. |
| `tools/thor_adb.py` | Serial-scoped discovery, binary screenshot capture, and staging a ZIP. It does not pretend to import the ZIP. |
| `tests/DualScreen.Android.Tests/` | One small test project for production coordination/guard logic, with shared source as appropriate. |
| `tests/ThorFixture/` | Optional, separately built fixture/observation mod only if existing commands cannot create needed test conditions. |
| `docs/android/README.md`, `compatibility.json`, `acceptance.md` | Installation/rollback, exact supported versions, and sanitized acceptance evidence. |
| `.local/` and `artifacts/private/` | Ignored references, APKs, copied saves, device inventory, raw evidence and scratch outputs. |
| `docs/android/execution-state.md` | Short checkpoint: commit, active artifact/build ID, completed gates, blockers, next action. No secrets or raw device serial. |

The Android project explicitly links the required existing source from `mod/`, with separate intermediate/output directories. Platform-specific files must be excluded from the desktop build, and both projects must exclude generated, fixture and test sources. Avoid broad changes to the desktop project just to make the mobile port compile.

## Execution Tasks

### Task 1: Establish a clean fork, pinned baseline, and local execution environment

**Files:** Plan, `.gitignore`, `docs/android/execution-state.md`, and initial `docs/android/compatibility.json`.

**Inputs:** This plan, upstream URL, current machine/tool availability, and existing GitHub authentication.

**Outputs:** A personal fork, a task branch, recorded upstream SHA, and a reproducible local workspace. GitHub actions may remain explicitly blocked while local work proceeds.

- [ ] Read applicable `AGENTS.md` instructions and Superpowers execution/verification skills. Use the worktree skill if an existing checkout needs isolation. Do not restart brainstorming or ask the user to choose an architecture already specified here.
- [ ] Inspect `git status`, remotes, branch, and uncommitted changes before touching an existing checkout. Use a separate directory/worktree where necessary; preserve unrelated work.
- [ ] Check `gh auth status` and the authenticated account. Query/reuse its fork if present; otherwise run `gh repo fork hung-eggie-do-covergo/stardew-thor-dual-screen --clone` in a suitable new directory. Verify fork ancestry and remote URLs. Choose a collision-free `android/thor-mobile` task branch. Do not assume the user owns upstream.
- [ ] Fetch upstream and compare its current source with the audited commit. Read README, license, FEATURES, both build files if present, and changed mod code. Pin one implementation baseline and record why it was chosen. Avoid upgrading dependencies mid-validation without restarting affected gates.
- [ ] Inspect local ADB, Python, Git, .NET, Android SDK/workload, JDK, OS, disk capacity, and network access. Install only missing tools needed for the selected build path using normal authorized mechanisms. Record versions.
- [ ] Add ignore rules before extracting any game or host files. Check that APKs, game DLLs, saves, raw captures, and `.local/` cannot be accidentally staged. Keep all proprietary dependencies outside tracked paths.
- [ ] Copy this plan into its intended repository location and create the brief execution checkpoint. Commit only the plan and safe setup changes.

**Gate 1:** A clean, identifiable source baseline exists, unrelated work is intact, and the build/device prerequisites have explicit ready or blocked status.

### Task 2: Identify the Thor, protect saves, and prove the unmodified mobile-host baseline

**Files:** `tools/thor_adb.py`, local device inventory/backups/evidence, and compatibility record.

**Tool interface:** `thor_adb.py inventory --serial SERIAL --out FILE`; `capture --serial SERIAL --physical-id ID --out PNG`; `stage --serial SERIAL --zip FILE`. All ADB calls use argument arrays, finite timeouts, checked exit codes, and the explicit serial. `capture` writes stdout as binary bytes. `stage` reports only the actual destination and checksum evidence available.

- [ ] Run `adb version` and `adb devices -l`. Select the single authorized physical Thor matching device properties. If several candidates remain ambiguous, ask one precise identification question. An `unauthorized` device requires the owner to accept the RSA prompt; an offline device requires a concrete cable/driver/debugging fix.
- [ ] Read the selected device's manufacturer/model, Android API, ABI, active Android user, installed package versions/installers, and actual package paths. Do not collect unrelated application data.

```bash
adb -s "$THOR_SERIAL" get-state
adb -s "$THOR_SERIAL" shell getprop ro.product.manufacturer
adb -s "$THOR_SERIAL" shell getprop ro.product.model
adb -s "$THOR_SERIAL" shell getprop ro.build.version.sdk
adb -s "$THOR_SERIAL" shell getprop ro.product.cpu.abilist
adb -s "$THOR_SERIAL" shell am get-current-user
adb -s "$THOR_SERIAL" shell dumpsys package com.chucklefish.stardewvalley
adb -s "$THOR_SERIAL" shell pm path com.chucklefish.stardewvalley
adb -s "$THOR_SERIAL" shell dumpsys display
adb -s "$THOR_SERIAL" shell dumpsys SurfaceFlinger --display-id
adb -s "$THOR_SERIAL" shell dumpsys input
adb -s "$THOR_SERIAL" shell input --help
adb -s "$THOR_SERIAL" shell screencap -h
```

These are Bash examples. On Windows, use the Python helper or correctly adapted shell syntax. In particular, do not route PNG bytes through text-mode PowerShell redirection.

- [ ] Map the top and lower physical panels from geometry, rotation, display metadata, screenshots, and later a visible probe label. Record **logical Android IDs for input** separately from **physical SurfaceFlinger IDs for screenshots**. Confirm the actual device's command help before using display flags. A typical supported input form is `input touchscreen -d LOGICAL_ID tap X Y`; a supported capture form is `screencap -p -d PHYSICAL_ID`. The IDs need not be equal. [S8][S9]
- [ ] Inventory existing game/host saves through legitimate readable paths or their export interfaces. Before any change involving a real save, copy it to the local backup directory, verify archive contents and required save files, record SHA256 and size, and retain an untouched backup. Do not assume `run-as`, `adb backup`, or SAF can read another application's private data. [S10]
- [ ] If original Play saves cannot be exported without new user access, leave them untouched and create a new disposable hosted farm. This does not block developing/testing the mod. Clearly mark that migration of those real saves remains untested.
- [ ] Inspect any installed host before updating it. Prefer the compatible released JunimoGate APK from the publisher. Verify its hash and package/signing compatibility. For an existing host with data, export and verify its saves first. Never uninstall or clear it to work around a signature mismatch.
- [ ] Install/launch the host through normal Android flows, complete its official-game discovery/setup, and confirm it uses this device's Play game and asset set. Use UI automation only after inspecting actual controls; do not assume a stable sequence of blind coordinates.
- [ ] Launch with a dedicated empty test mod group. Create/import a disposable test farm, play, save, quit, and reload it. Capture baseline logs, top-screen rendering, input behavior, version identities, and a short repeatable performance route.
- [ ] Check the host's export/import and log-sharing paths. A private mod/save directory is not writable merely because ADB is connected. JunimoGate's save export uses Android document creation; export to an accessible destination and pull the actual resulting file. [S7]
- [ ] Commit the small device helper and sanitized compatibility baseline.

**Gate 2:** The correct Play mobile game runs under the selected unmodified host, a disposable save survives a reload, both panels have verified identities, and original saves remain protected. If the host fails before loading this mod, diagnose that baseline before attributing the failure to the port.

### Task 3: Build and load the smallest release-mode dual-screen probe

**Files:** `android/DualScreen.Android.csproj`, `android/manifest.json`, `mod/MobileHost.cs`, `tools/build_android.py`, temporary probe code in the task branch, and compatibility record.

**Interfaces:** `MobileHost.ResolveGameActivity()` returns the actual `Android.App.Activity` hosting the game. `MobileHost.SelectCompanionDisplay(Activity activity)` returns the verified secondary `Android.Views.Display`, or null with a diagnostic if unavailable. Use concrete helpers, not a provider framework.

**Build interface:** `python tools/build_android.py --refs .local/refs/mobile --configuration Release --build-id COMMIT_SHA`. It emits one mod ZIP and SHA256, and exits nonzero on missing/ambiguous references or forbidden archive entries.

- [ ] Pull every APK path actually returned by `pm path` for the official game into an ignored directory. Include base and all installed splits; record hashes. Obtain the selected host APK and matching source/tag for inspection. Do not combine assemblies from different game versions or ABIs.
- [ ] Use JunimoGate's plain .NET GameInspector to inventory and extract the installed game's assembly stores. It supports APK store formats that are not ordinary loose DLLs. Run against a fresh output directory; inspect the resulting identities and ABI. [S11]

```bash
dotnet run --project tools/JunimoGate.GameInspector -- inventory <actual-apk-paths...>
dotnet run --project tools/JunimoGate.GameInspector -- extract-assemblies <fresh-output-directory> <actual-apk-paths...>
dotnet run --project tools/JunimoGate.GameInspector -- inspect-assemblies <output-directory>
```

Run these from the separately pinned JunimoGate source checkout. Replace angle-bracket placeholders with discovered local paths passed as individual arguments.

- [ ] Extract host references from its actual ZIP entries. The audited release bundles SMAPI at `assets/smapi-managed/StardewModdingAPI.dll`; enumerate entries to locate its MonoGame and optional host bridge uniquely. Use the host's matching SMAPI build, not a separately downloaded desktop SMAPI. [S4][S5]
- [ ] Create the mobile library project matching the selected host. Set `<OutputType>Library</OutputType>` and `<AssemblyName>DualScreen</AssemblyName>` explicitly so the project filename does not change the entry DLL. Explicitly reference required game, GameData, host SMAPI, and MonoGame assemblies with `Private=false`; add xTile/BmFont/other dependencies only as actually required. Let the Android workload supply framework references. Do not wildcard-reference extracted `System.*`, `Mono.Android`, `Java.Interop`, or duplicate host DLLs. Do not use automatic desktop-game discovery.
- [ ] Preserve `EntryDll: DualScreen.dll` and upstream attribution. Retain the mod's UniqueID for an exclusive replacement in the dedicated mobile group, with a clearly distinguishable Android version/build ID. Prevent loading both upstream and mobile variants together. Disable any inappropriate automatic desktop update route.
- [ ] Compile a Release probe that loads through SMAPI, obtains the actual Activity, shows a framework `Presentation` and `ImageView` on the identified lower screen, draws one real game texture and font, and records a lower-screen touch while top-screen gamepad input remains active. The audited upstream `Game.Activity` access and JunimoGate `GameHostBridge.GetActivity()` are concrete candidates. Validate the runtime behavior. [S2][S5]
- [ ] Specifically test managed event adapters and any Java peer/listener creation in the **released installed host**. A dynamically loaded mod can reference an Android API yet still lack generated Java callable wrappers for new managed subclasses. This is an early experiment, not a presumed failure. Prefer existing Android framework peers and event adapters before adding a host patch.
- [ ] Package only the probe's own DLL and manifest. Resolve the compiled DLL from the project's `TargetPath`; stage `android/manifest.json` as `manifest.json` and assert its `EntryDll` exists exactly once in the mod root. Stage the ZIP to Downloads, then actually import it through the host's Mod library, select the new revision in the test group, and cold-start the game. Verify the running build ID in logs. Pushing a file alone is not deployment.
- [ ] Pause/resume the probe and test a supported lower-display recreation. Confirm no duplicate windows, lost gamepad focus, or peer-registration failures. Remove/replace probe-only behavior before proceeding.
- [ ] Commit the working mobile build, package helper, and host integration baseline.

**Gate 3:** A release-mode DLL imported by the real released host renders the real game's assets on the real lower screen and receives touch without breaking top-screen input.

**Only if this gate proves a host change necessary:** Inspect the selected host's documented build path and make the smallest integration patch, preferably exposing an existing host-owned window/listener facility. Do not develop multiple host adapters in parallel. Full JunimoGate builds have pinned Linux/Bash and Android/.NET/JDK/MonoGame/Harmony requirements beyond compiling this mod. On Windows, keep one Windows ADB server owning USB while an appropriate Linux/WSL environment builds if practical. Follow the pinned build documentation and record any patched dependency commits. Preserve signing/data: use a separate application identity for an unsigned-compatible development host if needed, with copied saves. Never remove a working installation. Re-run Gates 2 and 3 using the resulting release host and include its reproducible source/artifact in the deliverable. [S12]

### Task 4: Make display, input, rendering, and lifecycle reliable

**Files:** `mod/ModEntry.cs`, `mod/MobileHost.cs`, `mod/CompanionState.cs`, and `tests/DualScreen.Android.Tests/`.

**Internal contract:** Each presentation/session has a monotonically increasing generation. A queued touch contains generation, menu identity, layout revision, action, and logical coordinates. Delivery succeeds only for the current generation/menu/layout and revalidates the target. Frame delivery owns its buffers until the UI has finished consuming them. No cross-file interface needs a general service container.

- [ ] Add small failing tests against production coordination code: a touch from a previous menu/session is dropped; replacing a pending frame keeps only the newest frame; an in-flight buffer is not reused; disposal invalidates queued callbacks; pointer coordinates match the actual drawable transform, including nonzero insets and cancel/out-of-bounds handling. Run and confirm failures before the corresponding changes.
- [ ] Wire the existing renderer to mobile Activity/display discovery. Select the validated secondary panel, exclude the Activity's primary display, and re-enumerate on display changes. If it disappears, stop companion work cleanly while allowing the top-screen game to continue. Reconnect without requiring a game restart where the host supports it.
- [ ] Use the secondary display's Presentation context and actual view dimensions. Keep the window nonfocusable for gamepad routing while accepting touch. Convert touch through the exact render transform; discard stale events after save/menu/layout/display transitions.
- [ ] Keep rendering and state mutation on the game thread. Perform window/Bitmap/View operations on the Android UI thread. Use one active transfer and at most one pending latest frame; drop/replace superseded frames. Never reuse a buffer or dispose a Bitmap while the UI may still access it. Avoid adding a general-purpose queue framework.
- [ ] Restore render targets, viewport and other modified graphics state with `try/finally`, including exceptional Aim-render paths. Confirm that temporary secondary rendering cannot leave the main SpriteBatch or lighting state corrupted.
- [ ] Subscribe/unsubscribe deterministically to game, Activity and display events. Invalidate all queued input/frame work on disposal or new save/session. Recreate resources after Activity/display loss. Replace the upstream Cinderbox-specific delayed `KillProcess()` exit behavior with the host/mobile lifecycle action. [S2]
- [ ] Run the focused tests and the release probe journeys again. On the device, deliberately background during touch/frame delivery and change menus before a queued action is consumed. Confirm no replay into a new menu, no UI backlog, and unchanged gamepad focus.
- [ ] Commit when the focused tests and device lifecycle gate pass.

**Gate 4:** The companion behaves correctly under delayed input, UI stalls, missing displays, pause/resume and recreation. Its rendering does not damage the primary game view.

### Task 5: Port every display mode while retaining game artwork and layout

**Files:** `mod/Panels.cs`, `mod/CropTracker.cs`, `mod/MobileGameAdapter.cs`, and acceptance record.

**Adapter contract:** `MobileGameAdapter.ValidateRuntime()` returns `IReadOnlyList<string>` describing unsupported required members/assets before interactive panels are enabled. An empty list means validation passed. Resolve and cache version-dependent APIs once per runtime. Keep narrow helpers next to the action they support; use the actual target assembly signatures rather than guessed field names.

- [ ] Inventory every dependency on desktop-only members, private reflection, WorldMaps, menu geometry, title animations, professions, lighting and mobile asset regions. Check the actual extracted mobile metadata and running objects. Record the few required differences in the adapter.
- [ ] Reuse `Game1` textures, fonts, item/NPC rendering and loaded assets. Validate sprite-region bounds, mobile UI scaling, and content reloads. If an asset key differs, map to the corresponding authentic mobile asset, not a generated substitute.
- [ ] Port and visually verify Today, Gifts, People/map, Bag/tooltips, Craft and tab settings. Check empty and full states, long names/descriptions, scrolling, current held-item updates, hidden/reordered tabs, and persisted selections.
- [ ] Port title/new/load/back/exit, save selection, dialogue choices, cutscene skip, festivals, contextual menu displays and end-of-day states. Match the mobile game's available operations. Check unsupported desktop title options against the unmodified mobile baseline; report a genuinely absent native capability explicitly rather than adding a nonfunctional button.
- [ ] Port Aim using the mobile renderer and its actual viewport, zoom, lighting, tool targeting and placement rules. Test outside, indoors, at night and in a mine, with both valid and invalid placement. Restore global state even when a draw fails.
- [ ] Complete a mode-by-mode visual review on both real screens using the Acceptance Matrix. Compare against upstream screenshots for intended layout while allowing documented mobile art/API differences. Confirm no clipped controls or unreadable text at the tested settings.
- [ ] Commit the complete visual/data port. Interactive money/item/progression actions remain unaccepted until Task 6.

**Gate 5:** Every required panel renders correctly with the real mobile game's state and assets. Missing native capabilities and additional-mod assumptions are documented explicitly.

### Task 6: Verify all consequential actions against the mobile game's rules

**Files:** `mod/Panels.cs`, `mod/MobileGameAdapter.cs`, focused tests where meaningful, optional `tests/ThorFixture/`, and acceptance record.

**Action contract:** Validate active menu/session and selected item/quantity immediately before mutation. Prefer the actual mobile menu handler/callback that the vanilla UI uses. If a direct operation is unavoidable, establish all prerequisites before modifying inventory, preserve required callbacks/stats/quests, and verify the final result. Do not build a second transaction engine.

- [ ] Audit the upstream direct mutations in selling, crafting, chest transfer, shipping, furniture rotation and professions. These are porting risks, not a claim that the upstream desktop implementation is broken. Verify reflected members and callback behavior before enabling their controls. [S2]
- [ ] Create a disposable fixture farm with deterministic items, stacks, money, recipes, menu access and progression. Use compatible existing test commands first. If necessary, build a separate fixture mod that refuses non-test-farm identities and only seeds/observes state. Never let it invoke the production Sell/Craft/touch handler as the end-to-end test.
- [ ] Write targeted failing tests for guard/quantity/state logic newly added by the port. Use real game behavior on device for inventory semantics instead of a mock reimplementation. Verify failure before the fix and success afterward.
- [ ] Implement the minimal mobile action adaptations. Shop sell/buy must preserve prices, currency, stock and callbacks; crafting must respect ingredient sources/output space/recipe count; chest moves must preserve item metadata and menu callbacks; shipping and trash must apply correct amounts/refunds; professions must use native progression behavior. Check the return value of operations that can fail.
- [ ] Preserve established quick-stack scope unless a documented upstream bug must be corrected. The audited code scans eligible player chests in the current location and farmhouse fridge, without a distance filter; do not silently reinterpret the README's word "nearby". Verify compatible-container restrictions on mobile. [S2]
- [ ] Drive every action through actual bottom-screen Android input. Assert before/after quantities, item identity/metadata, money, relevant callbacks/stats/quests, and resulting visible state. Test full inventory, full destination, partial stacks, insufficient currency/materials, canceled quantities, rapid double taps, and menu changes during pending input.
- [ ] Validate the action groups in the Acceptance Matrix, including geodes, bundles, museum, forge, tailoring, dialogue/cutscene and level-up/profession choices. Prepare fixtures instead of playing weeks of progression manually.
- [ ] Save, quit, cold-load, and recheck consequential results. For fixture-dependent transient states, label instrumented evidence and repeat the consequential user journey without the helper wherever possible. Do not count a debug-only pass as shipping release verification.
- [ ] Remove the fixture mod from the final test group. Check the production build graph and ZIP allowlist exclude all fixture code and commands. Commit the action fixes and their meaningful tests.

**Gate 6:** Required lower-screen actions follow mobile game rules, survive save/reload, and do not lose or duplicate items, money, or progression under the tested cases.

### Task 7: Run the complete physical-device acceptance and performance pass

**Files:** `docs/android/acceptance.md`, ignored timestamped evidence, and any narrowly necessary fixes.

- [ ] Build a clean Release candidate and record commit, build ID, mod ZIP hash, game/host versions, active mod group and test save. Import and select that exact revision, then cold-start the game. Verify its build ID in the running logs. Re-run affected rows whenever a fix changes the candidate.
- [ ] Execute every mandatory Acceptance Matrix row on the real Thor. Each row records setup, actual action, expected state change, observed result and evidence path. Use `pass`, `fail`, `blocked`, or `not applicable with verified native-capability reason`. A screenshot without the corresponding state check is incomplete evidence for a transaction.
- [ ] Capture both displays using their physical capture IDs and route touches using their logical IDs. MonoGame controls may not appear in UIAutomator XML; use visible coordinates, screenshots and observed game state. Do not infer a successful touch from ADB's zero exit code.
- [ ] Verify gamepad buttons and axes while the lower UI has been touched and while contextual menus are open. `adb input gamepad keyevent` checks only the supported injected key route. It does not prove real analog sticks, physical button mapping or touch hardware. Use available legitimate device input instrumentation; if physical actions remain necessary, request one short grouped hardware test and keep those rows blocked until results arrive.
- [ ] Perform five cold launches, ten pause/resume cycles, and five repetitions of the actual supported lower-display toggle/recreation operation. Include title, loaded save, a menu and Aim. Record what was really exercised; do not label Activity recreation as lid/display reconnection.
- [ ] Compare mod-disabled and mod-enabled runs on the same save, route, frame cap, graphics settings and power mode after warmup. Use game-loop frame timing or an appropriate profiler; Android UI `gfxinfo` alone may not describe MonoGame/OpenGL performance. Record method, duration and noise.
- [ ] Apply these engineering targets: no sustained loss of a frame cap the baseline holds; no more than 20% increase in p95 game frame time on a matched route at a saturated cap; ordinary bottom-screen visible response within 150 ms; bounded frame delivery; no continuing memory/resource growth across repeated recreation after stabilization. These are acceptance targets, not claims of existing measurements. Investigate real regressions before changing the renderer.
- [ ] Run a 30-minute mixed-session soak on the Release candidate with movement, panels, Aim, transactions, background/resume, save, exit and reload. Collect bounded startup/session/crash logs across process restarts. PID-only capture must not silently omit a crashed/restarted process. Look for exceptions, ANRs, missing assets, lost input, blank panels, stale windows and resource growth.
- [ ] Fix reproducible failures and repeat only affected tests plus the final smoke journey. Stop optional testing once the declared gates pass. Commit fixes, update the checkpoint and produce sanitized evidence summaries.

**Gate 7:** All mandatory feature, save-integrity, hardware, lifecycle and performance checks pass on the actual Release candidate. Any blocked hardware evidence remains visibly blocked and prevents a claim of complete device verification.

### Task 8: Review, package, install the final build, and deliver

**Files:** Build/package output, installation/rollback README, compatibility record, completed acceptance report, and execution checkpoint.

- [ ] Request one independent whole-branch review of source and evidence, focusing on the five Review Focus items. Give the reviewer the exact commit/diff, target runtime, test results and ZIP listing. Resolve substantial findings and rerun their affected checks. Do not require multiple ceremonial reviews of unchanged work.
- [ ] Make the final clean Release build from the recorded commit. Archive allowlist: Android manifest, the mod's DLL, required mod-owned assets/i18n if any, license/attribution and installation notes. Reject game/host/framework DLLs, APKs, test helpers, saves, private paths, `.aar` files and unrelated build outputs. Include no desktop runtimeconfig/deps files unless the verified loader explicitly requires them.
- [ ] Record SHA256, build/tool versions and exact game/host identities. Inspect the ZIP's single valid mod root, manifest entry DLL, version and dependencies. Rebuild once from the documented inputs to verify the packaging procedure; exact byte reproducibility is required only if timestamps are normalized and the build supports it.
- [ ] Import and select the final ZIP in a clean dedicated release group, with fixtures/diagnostic mods absent. Cold-start and verify the actual build identity, top/bottom rendering, touch/gamepad focus, one item action, save and reload. If the final artifact differs from the tested candidate, rerun every affected mandatory row before claiming completion.
- [ ] Leave the working mod installed and selected. Keep a copy of the ZIP in an accessible host location and on the Thor. Document the actual launch route, supported versions, installation/import/revision-selection steps, and rollback: disable the mod or select the previous verified revision, then restart the game. Original Play saves remain unchanged.
- [ ] Run the final diff/staging check for proprietary files, personal save data, raw device identifiers and unintended changes. Commit/push the task branch to the user's fork without force. Create a release/PR only if separately requested; do not contact upstream maintainers.
- [ ] Deliver the fork URL, branch and commit; downloadable mod ZIP and SHA256; any necessary separately built host with source and exact requirements; completed acceptance report; install/rollback notes; and concise residual limitations. Mark execution complete only if the Definition of Done below is satisfied.

**Gate 8:** The reviewed release is installed, its source is recoverable, its build/package procedure is documented, and its actual device evidence supports every completion claim.

## Acceptance Matrix

Use the exact shipping mod for final rows. Fixtures may establish the save state, but the recorded user action must follow the production Android input path. Extend a row if source review finds another materially different upstream behavior; do not drop one because the test is inconvenient.

| Area | Minimum device checks and state assertions |
|---|---|
| Installation and identity | Correct Play mobile version/assets; selected host/mod revision; no duplicate runtime assemblies; cold launch reports expected build ID. |
| Display and input | Correct physical lower panel; correct coordinate edges/insets; taps, drags, long press, cancel; top-screen controller focus before/after touch; safe missing/recreated display. |
| Today | Luck/weather/crop readiness and watering/birthdays/festival/cart information matches the test game's actual state; updates on day/location/save change. |
| Gifts | Held-item and nearby-NPC changes update likes/loves, friendship/gift counts and bundle relevance correctly; no hidden-character information leaks. |
| People | Met characters, local ordering, birthday indicator, map/player/NPC positions, Gifts subview, scrolling and navigation; valid mobile map assets. |
| Bag and tooltips | Empty/full inventory, selecting, dragging/reordering, stack merge, trash/cancel/refund, no-room menu; preserve quality/mod-data; readable full tooltips. Extra tooltip mods only if installed and independently compatible. |
| Quick stack | Existing eligible-chest/fridge scope, matching-stack rules, full destination, inaccessible container behavior and item conservation. |
| Craft | Known/unknown recipe visibility, ingredient counts/sources, output quantity and stacking, insufficient ingredients, full inventory, recipe/stat/quest callbacks; save/reload. |
| Aim | Valid/invalid furniture/chest/seed placement, tool direction/range, hotbar, rotation, indoors/outdoors/night/mines, lighting/zoom and unchanged main renderer. |
| Chest | Take/store, full/partial stacks, long-press quantity, fill stacks, organize, metadata/callbacks and rapid repeated input; no duplicate/lost items. |
| Shop | Buy 1/5/25 and stock limits; sell chosen quantity and number pad; price/currency/quality/refund/buyback behavior where available; poor/full/cancel/double-tap cases. |
| Shipping | Full/partial chosen quantities, correct shipping list and totals, game-supported retrieval behavior, save/reload and next-day accounting. |
| Geodes | Accepted/rejected input, required money, output capacity, animation completion, correct one-time consumption and output. |
| Community Center | Correct bundle selection, accepted quantity/quality, rejected item remains, completion/reward progression and persistence. |
| Museum | Accepted/rejected/duplicate items, donation placement and reward progression; no item removal on cancellation or rejection. |
| Forge | Valid/invalid materials, cost, output, cancel and game-supported unforge behavior; item/modifier preservation and no duplicate consumption. |
| Sewing/tailoring | Correct input acceptance, ingredient consumption, output/dye behavior and cancel; full-bag handling. |
| Dialogue and events | All offered choices, selection sync, no stale answer after menu replacement; cutscene skip only when supported; festival tabs remain correct. |
| Title and saves | New/load/list/scroll/back and safe exit; correct selected save; mobile-supported co-op entry only if the unmodified target exposes it. Record absent native co-op as a platform capability, not a passing feature. |
| End of day | Matching date/income/total animation, one Continue action, all available level-up/profession choices, correct native perks and next-day progression after reload. |
| Tabs/settings | Hide/reorder/select/persist; screen/scale change; no unreachable active panel or incorrect touch mapping. |
| Lifecycle and hardware | Five cold launches, ten pause/resume cycles, five real supported display recreation cycles, actual controller/touch checks and 30-minute soak. |
| Release hygiene | Final ZIP/group has no fixtures or bundled game/framework DLLs; clean import/restart/build-ID proof; original game/saves intact; rollback and build instructions work. |

For transactions, record an appropriate conservation check rather than merely "UI looked correct": moved items retain total count; selling/shipping/trashing changes the specified count and monetary outcome; crafting/processing consumes the correct inputs and yields the correct outputs. Where the game intentionally transforms identity or quantity, assert that rule explicitly.

## Definition of Done and Honest Blockers

Completion means all eight gates and every mandatory matrix row pass for the recorded Play game, host, Thor firmware and mod artifact. A platform-absent conditional capability must have evidence from the unmodified mobile baseline and be clearly disclosed. Additional-mod compatibility is limited to the combinations actually tested.

The user receives a working installation and distributable mod, not merely a repository that compiles. The final report distinguishes verified results from reasonable expectations and cannot use "perfect" as a substitute for evidence. Future game/firmware/host updates may require another compatibility pass.

If a material gate cannot be completed, finish all unblocked work and report the exact remaining prerequisite, artifact state, reproduction and next command/action. Examples include an unauthorized USB connection, no local USB access, a Play update unsupported by available hosts, a rejected privileged tool install, or a real controller test that cannot be performed through available instrumentation. State any automatic approval rejection and its reason. Do not hide a blocker by silently reducing features, changing the game edition, or counting an untested path as passed.

Keep `execution-state.md` current at gate boundaries so another Codex session can resume without rebuilding the investigation. Do not continue endless speculative work after an external prerequisite is conclusively blocked.

## Copy-Paste Kickoff for Local Codex

```text
@Superpowers @GitHub

Execute the attached 2026-10-30-thor-stardew-android-plan.md from start to finish on this computer, which has my AYN Thor connected by USB. Read the complete plan and applicable repository instructions first. Use Superpowers executing-plans and verification-before-completion. This is an execution request, not another planning request.

Target my official Google Play Android Stardew Valley installation. Discover its installed version and the correct Thor serial. Preserve the original Play app and real saves. Use the released compatible JunimoGate host first, keep the original mod's UI and game assets, and make only changes supported by source inspection and real tests. Do not substitute the PC game or Cinderbox.

I authorize you to create or reuse my personal fork of https://github.com/hung-eggie-do-covergo/stardew-thor-dual-screen, work on an isolated branch, set up the necessary local build tools through authorized mechanisms, install a compatible host if needed, import and enable the mod, create/use disposable or verified copied test saves, build, debug, run ADB/device tests, fix failures, commit, and push the finished source branch to my fork. A minimal host patch is authorized only if the release-host probe proves it necessary and the plan's preservation rules are followed. Do not merge to upstream, contact maintainers, clear app data, uninstall existing apps, overwrite real saves, root the device, bypass access controls, or publish proprietary files.

Proceed autonomously through all unblocked tasks. Do not stop after scaffolding, compilation, or writing instructions, and do not repeatedly ask me to approve routine steps. Use one owner for the device/coupled code and independent review where useful. If a real human action is unavoidable, continue everything else first and ask for the smallest specific action with the reason.

Finish with the working Release build installed and selected on the Thor, the mod ZIP and SHA256, fork/branch/commit, exact compatibility versions, complete physical-device acceptance evidence, and installation/rollback notes. Never claim complete if a mandatory gate is failed, blocked, or untested. Keep a resumable execution checkpoint in the repository.
```

## Primary Sources and Revalidation Points

- **[S1] Upstream baseline and features:** [repository at audited commit](https://github.com/hung-eggie-do-covergo/stardew-thor-dual-screen/tree/15f05b63d3ca852f3729e20de67fc7446bfac112), [feature inventory](https://github.com/hung-eggie-do-covergo/stardew-thor-dual-screen/blob/15f05b63d3ca852f3729e20de67fc7446bfac112/docs/FEATURES.md).
- **[S2] Actual implementation:** [ModEntry.cs](https://github.com/hung-eggie-do-covergo/stardew-thor-dual-screen/blob/15f05b63d3ca852f3729e20de67fc7446bfac112/mod/ModEntry.cs), [Panels.cs](https://github.com/hung-eggie-do-covergo/stardew-thor-dual-screen/blob/15f05b63d3ca852f3729e20de67fc7446bfac112/mod/Panels.cs), [project](https://github.com/hung-eggie-do-covergo/stardew-thor-dual-screen/blob/15f05b63d3ca852f3729e20de67fc7446bfac112/mod/DualScreen.csproj).
- **[S3] Official game and host baseline:** [Google Play listing](https://play.google.com/store/apps/details?id=com.chucklefish.stardewvalley), [JunimoGate discovery results](https://github.com/leontismaro/JunimoGate/blob/v0.3.1/docs/game-discovery-result.md), [compatibility](https://github.com/leontismaro/JunimoGate/blob/v0.3.1/docs/compatibility.md).
- **[S4] Released host and build target:** [v0.3.1 release](https://github.com/leontismaro/JunimoGate/releases/tag/v0.3.1), [application project](https://github.com/leontismaro/JunimoGate/blob/v0.3.1/src/JunimoGate.App/JunimoGate.App.csproj), [SMAPI bundle](https://github.com/leontismaro/JunimoGate/blob/v0.3.1/build/JunimoGate.SmapiBundle.targets).
- **[S5] Host bridge and mod example:** [GameHostBridge.cs](https://github.com/leontismaro/JunimoGate/blob/v0.3.1/src/JunimoGate.GameHost/GameHostBridge.cs), [smoke mod project](https://github.com/leontismaro/JunimoGate/blob/v0.3.1/samples/JunimoGate.SmapiSmokeMod/JunimoGate.SmapiSmokeMod.csproj).
- **[S6] Mod loading and import:** [assembly binding](https://github.com/leontismaro/JunimoGate/blob/v0.3.1/docs/mod-assembly-binding.md), [startup chain](https://github.com/leontismaro/JunimoGate/blob/v0.3.1/docs/startup-chain.md), [archive import](https://github.com/leontismaro/JunimoGate/blob/v0.3.1/src/JunimoGate.Mods/ModArchiveImport.cs).
- **[S7] Save export and app constraints:** [LiveSavesFragment.cs](https://github.com/leontismaro/JunimoGate/blob/v0.3.1/src/JunimoGate.App/LiveSavesFragment.cs), [Android manifest](https://github.com/leontismaro/JunimoGate/blob/v0.3.1/src/JunimoGate.App/Properties/AndroidManifest.xml).
- **[S8] ADB and real-device setup:** [ADB documentation](https://developer.android.com/tools/adb), [hardware-device setup](https://developer.android.com/studio/run/device), [Android 15 input implementation](https://android.googlesource.com/platform/frameworks/base/+/refs/heads/android15-release/services/core/java/com/android/server/input/InputShellCommand.java).
- **[S9] Screenshot display IDs:** [Android 15 screencap implementation](https://android.googlesource.com/platform/frameworks/base/+/refs/heads/android15-release/cmds/screencap/screencap.cpp). The connected device's help and actual behavior remain authoritative.
- **[S10] Storage/backup boundaries:** [app-specific storage](https://developer.android.com/training/data-storage/app-specific), [document access](https://developer.android.com/training/data-storage/shared/documents-files), [Android 12 ADB backup restrictions](https://developer.android.com/about/versions/12/behavior-changes-12#adb-backup-restrictions), [run-as implementation](https://android.googlesource.com/platform/system/core/+/refs/heads/main/run-as/run-as.cpp).
- **[S11] Assembly extraction tool:** [GameInspector commands](https://github.com/leontismaro/JunimoGate/blob/v0.3.1/tools/JunimoGate.GameInspector/Program.cs), [project](https://github.com/leontismaro/JunimoGate/blob/v0.3.1/tools/JunimoGate.GameInspector/JunimoGate.GameInspector.csproj).
- **[S12] Full-host fallback build:** [build environment](https://github.com/leontismaro/JunimoGate/blob/v0.3.1/docs/build-environment.md), [JunimoGate SMAPI fork](https://github.com/leontismaro/JunimoGate-SMAPI/tree/android/junimogate). Select the dependency commits pinned by the chosen host tag, not an unrelated branch tip.

## Plan Review Record

Author self-review covered specification coverage, actionable step granularity, consistency of named files/interfaces, all five Review Focus items, and proportion. Independent acceptance and build reviews found no remaining material gaps after making the Android project's isolated build directories and exact output DLL name explicit. These are reviews of the plan, not execution results.

The plan deliberately keeps the existing renderer/panels, proves the risky runtime boundary first, confines tests to meaningful behavior, and uses one acceptance matrix instead of a new orchestration system. Execution must refresh version assumptions and supply the device evidence this planning document cannot supply.
