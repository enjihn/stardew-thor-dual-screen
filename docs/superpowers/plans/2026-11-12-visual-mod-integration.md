# Add the three mods to Stardew Dual Screen

User-approved execution specification, 2026-11-12.

## Constraints and deliverable

Branch android/visual-mod-integration starts at eb23b77. Preserve accepted
ThorItemSwitchRelease, its ZIP/configuration, the official Play game, branded
host, direct icon, all game artwork and real saves. Root alone owns device
and coupled source changes. Use only disposable ThorTest453080632. Wait for
a safe exit if a farm is open. Source pushes go only to the personal fork.

Install original Fashion Sense7.5.0, SDV-Radiance2.4.8 and Seasonal Cute
Characters6.1.4 first. Patch only failures reproduced on Thor, in the
responsible mod before changing ThorDualScreen. Preserve identities and one
version per mod. Retain source revisions, patches, build instructions and
licenses; never bundle proprietary references or another Harmony copy.

Prioritize smooth60FPS. Radiance extra effects belong on the top screen;
preserve the existing Aim preview. Fashion Sense bundled presets suffice;
additional wardrobe packs are outside scope.

## Execution

1. Preserve accepted state and acquire official Content Patcher2.9.1 and
   Generic Mod Config Menu1.16.0. Record exact versions, URLs and SHA256s.
   If CP2.9.1 fails on the host, test the author's Android2.8.1 before patches.
   Seasonal content format is2.8.0.
2. Create ThorVisualModsTest. Add dependencies and Seasonal first, Fashion
   Sense next, Radiance last. Keep Seasonal artwork/defaults. Reject partial
   Fashion Sense Harmony initialization and restart before further tests.
   Begin Radiance automatic AndroidLow with Smooth Art disabled. Verify
   required hooks, shaders and graphics-state restoration.
3. Seasonal: visible NPC sprites/dialogue portraits, representative seasons,
   lower People/Gifts portraits, no missing textures/content errors.
4. Fashion Sense: Hand Mirror, bundled body/sleeve/shoe presets, select/recolor,
   movement/tools/menu previews, save/reload, restore original appearance.
   If Seasonal farmer edits conflict, disable only its farmer customization.
5. Radiance: visible shadows/water/reflections/night lighting, keyboardless
   settings open/save, interiors/rain/pause/resume; readable functional lower
   panels. Verify GMCM touch and controller operation, not desktop assumptions.
6. Combined: every companion tab, Bag/Aim selection, inventory, mini toolbar,
   touch-through, companion loss/recovery, cold launch through existing icon.
7. Matched two-minute baseline/combined runs in busy outdoor and night/rain
   scenes, Aim active, unchanged power/display settings. Mean FPS >=58, no
   sustained five-second window <55FPS, presentation-frame p95 <=1.2 baseline.
   Tune highest Radiance quality meeting these limits; report disabled effects.
8. Run existing coordination/packaging checks. Add focused regression tests
   only for compatibility code actually changed. Request one short owner
   check of movement, bumpers/triggers, tools and new menus after autonomous
   checks pass.
9. Create/select ThorVisualModsRelease with exactly six production components:
   ThorDualScreen0.5.2-android.3, the three mods, Content Patcher and GMCM.
   No test fixture. Existing icon launches selected group directly.
10. Deliver installed configuration/versions, hashes, acceptance evidence,
    source branch/commit, patches if needed, installation/rollback notes and
    resumable checkpoint. Push source only. All three must function together;
    import/startup alone is insufficient. If any mandatory gate is unresolved,
    preserve accepted release selection and explicitly report the blocker.

## Official sources

- https://www.nexusmods.com/stardewvalley/mods/9969
- https://github.com/Floogen/FashionSense/tree/v7.5.0
- https://www.nexusmods.com/stardewvalley/mods/49397
- https://github.com/PHUICMT/SDV-Radiance
- https://www.nexusmods.com/stardewvalley/mods/5450
- https://www.nexusmods.com/stardewvalley/mods/1915?tab=files
- https://www.nexusmods.com/stardewvalley/mods/5098?tab=files
- https://github.com/spacechase0/StardewValleyMods/blob/develop/framework/GenericModConfigMenu/docs/README.md
