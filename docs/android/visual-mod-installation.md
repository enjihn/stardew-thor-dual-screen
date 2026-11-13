# Visual mods: installation and rollback

**ThorVisualModsRelease** is installed and selected with Radiance on **Low**. The owner completed the physical controls and menu check and replied "Seems fine". Low averaged60.0FPS with a minimum five-second rate59.8FPS. Full functional results and the accepted frame-pacing limitation are in the [acceptance report](visual-mod-acceptance.md).

The current complete profile archive is **ThorVisualModsRelease-Low.zip**, 38054874 bytes, SHA256 `1ae18953214b5f974d2609938aa11720d8c0478d7f549b7c09e01b21d017da50`. It has been imported and selected. A cold launch through the actual Cocoon **Stardew Dual Screen** icon confirmed five code mods and one content pack, with no fixture. Owner physical acceptance passed. The earlier Quality ZIP is superseded.

The existing **ThorItemSwitchRelease** group is retained as the accepted rollback. JunimoGate's **Stardew Dual Screen** direct-launch icon opens the currently selected group.

## Selected versions

| Component | Version |
|---|---|
| Thor Dual Screen Android | 0.5.2-android.3, build item-switch-android3-03 |
| Content Patcher | 2.9.1 |
| Generic Mod Config Menu | 1.16.0 |
| Seasonal Cute Characters | 6.1.4 |
| Fashion Sense | 7.5.0-android.1 |
| SDV-Radiance | 2.4.8-android.2, licensed package |

Exact package hashes, original provenance and official dependency links are in [the package inventory](visual-mod-packages.json). Original Fashion Sense 7.5.0 and Radiance 2.4.8 fail startup on this host. Use the documented compatibility packages for those two components.

## Import and select

For another installation, import **ThorVisualModsRelease-Low.zip** through JunimoGate's Groups interface. Keep ThorItemSwitchRelease. Select the imported Low group before using the existing direct-launch icon.

For a manual setup, create a separate group and import the six packages listed above: the unchanged releases of Thor, Content Patcher, GMCM and Seasonal, plus the patched Fashion Sense and licensed Radiance ZIPs. Enable exactly one version of each component. The production group has six components and no Thor Disposable Farm Fixture. Keep this group separate from the accepted group and its configuration.

The final exported settings are in `configuration-Low`, with each config mapped to its mod and hash in `index.json`. Radiance is enabled with `LowSpec`, automatic scaling on and a100% render-scale ceiling. Water reflection remains enabled with short reach0.2; characters use one shadow cast each. Smooth Art, object shadows, chromatic aberration, tilt-shift, God Rays, sun rays, cloud shadows, sprite relief, fog, color grading, windy spells and wet ground are disabled.

The owner check used the disposable **ThorTest** farm, cold-loaded at Spring22 with a Hand Mirror and five tools saved natively. All four original test-save backup files remain retained. Real saves were not accessed.

## Settings and use

Open the native inventory, then **Options > Mod Options** for GMCM. For Radiance, choose **Hotkeys & diagnostics > Open the Radiance tuner**. Save and close the GMCM pages and all native menus; the tuner then opens. Save and close the tuner after making changes.

Fashion Sense uses the Hand Mirror. Its source registers the mirror at Pierre's for 750g; an actual shop purchase has not been tested. Select a mirror in Bag and use it in the world through Aim to open the original menu. Appearance checks used local body, sleeve and shoe presets. External Fashion Sense packs are outside the tested candidate.

## Rollback

Save and quit the game, open JunimoGate Groups, and select **ThorItemSwitchRelease**. Launch with the same direct-launch icon. Do not remove the accepted group or replace its configuration.

Rollback requires no uninstall or app-data clearing. Keep save backups separate and do not overwrite real saves with the disposable ThorTest files.

## Source and licenses

The source-only fork work is on branch `android/visual-mod-integration`, foundation `81b53c9`. The [Fashion Sense](../../compatibility/FashionSense/README.md) and [Radiance](../../compatibility/Radiance/README.md) notes provide pinned author sources, minimal compatibility changes, build instructions and retained licenses. Proprietary game assemblies, private build references and game assets are not published.
