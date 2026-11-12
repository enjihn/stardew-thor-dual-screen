# Native toolbar hiding acceptance

Status: PASS on 2026-11-12. Required device gates and owner physical-control check passed.
The final release is installed and selected in ThorToolbarRelease.

Accepted version: 0.5.2-android.2, Release build `toolbar-android2-06`.
ZIP SHA256: `4f3c4062ff90b52a28643250507446b736fe8b78d1ed459736e65ec08334a8d3`.
Branch: `android/hide-top-toolbar`, based on `ec3ac6a` in the personal fork.

Runtime verified on Thor serial 57ae2a4f: Android 13/API33, official Play
Stardew Valley 1.6.15.3/code245 ARM64, personal branded JunimoGate
0.3.1-thor.2/code12, SMAPI 4.5.2-junimogate.41, host Harmony
2.4.2-junimogate.64. The existing host, launcher, game artwork and Play
installation were not rebuilt or replaced. Tests target only the disposable
ThorTest farm 453080632. Instrumented tests used the separate ThorToolbarTest
group; final acceptance used ThorToolbarRelease with no fixture.

The implementation hides native Toolbar.visible plus only that toolbar's
touch, hover and hit testing. Native update and the instance remain active.
Availability requires an Android frame commit, with session/epoch rejection.
Both hide and restore transitions wait through a released-touch frame.
Configuration defaults to HideToolbarWithCompanion=true and fails open.

## Direct results

Private raw evidence is under `artifacts/private/sidebar-research/`. The local
delivery includes toolbar-device-evidence.zip with the relevant captures and
observations. Tests on build05 cover unchanged toolbar code; build06 adds the
Android Bag slot fix and was used for final fallback, row and physical acceptance. It is
excluded from source distribution, along with game assemblies and content.

| Gate | Evidence and result |
|---|---|
| Release build and packaging | Build06 succeeds, one existing getItemCount obsolete warning, no errors. ZIP contains only the five owned release files, no Harmony or game DLL. |
| Focused automated checks | 15 coordination/installation tests and 10 packaging/helper tests pass. Partial installation rollback was deliberately broken, failed with partial interception remaining, then passed after restoration. |
| Visual and world input | Build04/05 and build06 native state confirm toolbar absent with other HUD elements present. Build05 former-strip tap moved the player from tile30,15 to27,14 without changing selected slot0. Screenshots strip05-before/after. |
| Camera | Paired left05-hidden/visible: viewport x0 versus x-103, same1593x896 extent. Hiding removes native left allowance. Boundary and interior images show world content, with no new gap. No zoom/resolution setting was changed. |
| Tabs | Today, Gifts, People, Bag, Craft, Aim and Tabs stay hidden with available companion (tab05-*). First four repeated on build06. |
| Bag and Aim | Direct lower touches select tools. Row test exposed original Android shiftToolbar mismatch, including with hiding disabled: slot13 incorrectly selected slot3. Android-only direct selection fixes it; row06-fixed selects exact slot13, item(O)82. Desktop behavior is unchanged. |
| Controlled upload failure | fault06-active: native visible=true, available=false. Native top tap changes selection13 to2. fault06-recovered: visible=false, available=true after original bitmap references are restored. Same sequence passed build05. |
| Touch transitions | gesture05-held/released: native toolbar press stays visible until release, then hides. Reverse world press stays hidden until release, then restores without selecting a toolbar slot. |
| Menus/location/pause | Native inventory, beach event entry/exit, Town/Beach/FarmHouse warps and Home/resume passed on build05. Build06 native DialogueBox choice through the lower screen records TestYes, closes and leaves toolbar hidden. |
| Config cold restart | config06-coldb: after writing false and restarting, enabled=false, available=true, native visible=true. Restored true afterward. |
| Missing companion | Launched the custom host on logical display4, leaving no eligible companion display. missing06-state/select: enabled=true, available=false, visible=true, hit=true; native tap changed slot0 to2. Returned game to top display0. This tests no eligible display, not a physical unplug. |
| Previous-version rollback | ThorRelease cold launch loaded only 0.5.2-android.1, build22cf243112b710381db6db8b89dc01882958a67f. rollback06-farm shows the normal toolbar; log confirms companion display4. |
| Physical controls | Owner confirmed "everything seems to work" on the final fixture-free build after checking left-stick movement, both bumpers/triggers, second-row Bag selection, tool selection/use through Bag or Aim, Start/B inventory and D-pad/A navigation, with the sidebar absent. physical06-confirmation.json records the exact scope and reply. |
| Final selection | ThorToolbarRelease selected with exactly one enabled mod. DirectLaunchActivity cold launch logs Loaded 1 mods, toolbar-android2-06, game1.6.15.3 and companion display4. No fixture is loaded. final06-ready and final06-bottom show gameplay on top, toolbar absent and populated Bag below. Disposable seeded tools persisted through native overnight save and cold reload. |

## Preservation and rollback

Fresh preservation06.json confirms all three original Play save files still
match the pre-existing verified backup hashes. Play version/code are unchanged.
Native display size1080x1920 and density369 have no overrides. Test seed, warp,
configuration and fault operations refuse every farm except ThorTest.

The custom direct-launch icon now uses the selected ThorToolbarRelease group.
It loads exactly one mod, toolbar-android2-06. The installed APK is unchanged.

The accepted 0.5.2-android.1 ZIP and ThorRelease group remain available.
Select ThorRelease and restart the hosted game to roll back. Alternatively,
set HideToolbarWithCompanion=false in the selected group's config and restart.
Do not clear app data, uninstall apps or replace real saves.

## Decisions and observed fixes

Build03's dead ViewTreeObserver crash was fixed before acceptance. Build04
started successfully. Independent review identified stale epochs after commit
registration failure and unsafe disposal/ownership of already pending callbacks.
Build05 fixed both; tests cover fresh-frame recovery and late callback ownership.

The required row test justified one additional narrow Android fix: direct
full-inventory selection, because the native Android shiftToolbar advances
one slot instead of rotating rows. This was reproduced with hiding disabled
before the change and passed on build06. No unrelated menu rewrite or host
patch was introduced.
