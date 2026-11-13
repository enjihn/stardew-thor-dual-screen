# Second Thor setup checkpoint

Updated 2026-11-12. The accepted Low profile is installed and selected. Autonomous checks passed and the second-device owner physical check passed with **"Seems good"**. Device setup is complete. Personal save migration remains deferred while the owner locates the source files. Root owns device operations and every ADB command targets `e7eaee41`. Original Thor `57ae2a4f` is disconnected.

Target: AYN Thor, Android 13/API 33, Android user 0. Official Play package `com.chucklefish.stardewvalley` remains **1.6.15.3/code 245**, installed through Google Play. It was not replaced, launched, cleared or uninstalled. No real saves were overwritten and no farms were copied from the first Thor.

Pre-install inventory found the accessible Play `files/Saves` directory empty, with no files anywhere under the app's external data directory. Legacy `/sdcard/StardewValley` was absent. The final external-file check was still empty. Private Play storage was not inspected. The local inventory is `outputs/second-thor-e7eaee41/before/inventory.json`. **Zero personal save files were available to back up. This inventory is not a farm backup.** The owner is locating saves on another device and explicitly authorized setup while save migration remains deferred.

## Installed selection

Selected group: **ThorVisualModsRelease**, imported from the accepted complete Low archive. Exactly five code mods and one content pack loaded after an actual Cocoon icon cold launch. No test fixture is included.

| Component | Version |
|---|---|
| Stardew Valley Android | 1.6.15.3 / 245 |
| Branded JunimoGate host | 0.3.1-thor.2 / 12 |
| SMAPI | 4.5.2-junimogate.41 |
| Harmony | 2.4.2-junimogate.64 |
| Thor Dual Screen Android | 0.5.2-android.3, item-switch-android3-03 |
| Content Patcher | 2.9.1 |
| Generic Mod Config Menu | 1.16.0 |
| Seasonal Cute Characters | 6.1.4 |
| Fashion Sense | 7.5.0-android.1 |
| SDV-Radiance | 2.4.8-android.2 |

Host APK: `outputs/Stardew-Dual-Screen-0.3.1-thor.2.apk`.

SHA256: `532adbabe1c4926f0b25221786c7f295f331d2c5025f45d54654504ea45512e4`.

Complete group: `outputs/visual-mods/ThorVisualModsRelease-Low.zip`, 38,054,874 bytes.

SHA256: `1ae18953214b5f974d2609938aa11720d8c0478d7f549b7c09e01b21d017da50`.

Both artifact hashes were rechecked. Per-mod hashes and provenance remain in [the package inventory](visual-mod-packages.json).

Radiance retains **LowSpec**, automatic scaling, 100% ceiling, Smooth Art off, short water reflections and the accepted disabled effects. Thor retains native toolbar hiding, the solid temporary item row and original lower Aim preview. No new source patch, rebuild, zoom adjustment or performance tuning was performed.

## Direct device checks

Added **Stardew Dual Screen** to Cocoon Home with the branded chicken icon. This Cocoon version exposes **Top Screen** in its launch menu. Enabled **Treat as Game: Always**, kept external-display launching off, selected Top Screen, then verified normal icon use. No accessibility service or manual manager step is needed.

Evidence is local under `outputs/second-thor-e7eaee41/`. Logs, screenshots, APKs and saves are not committed.

| Check | Observed result and local evidence |
|---|---|
| Actual icon cold launch | Native splash/title on top, companion below; `cold-icon-top.png`, `cold-title-bottom.png`, `cold-startup.txt` |
| Icon resume after Home | Game on top and companion below; `one-tap-warm-top.png`, `one-tap-warm-bottom.png` |
| Native Co-op entry | Host/Join opened and Co-op persisted after cold launch. Network session not tested. `new-character.png` captures Host/Join despite its early filename; `cold-title-bottom.png` shows persistent Co-op |
| Disposable farm | New **Thor2Test**, Standard Farm; `inventory-top.png` |
| Rendering and movement | Touch movement entered/exited farmhouse; outdoor world and indoor lighting rendered; sidebar absent with HUD retained; `outdoor-top.png`, `owner-ready-top.png` |
| Companion and menus | Bag, Today, People, Gifts, Craft and Aim exercised; native inventory and touch-operated Mod Options opened; `bag-bottom.png`, `people.png`, `craft.png`, `mod-options.png` |
| Temporary row | Aim selection displayed solid five-slot row; `item-row-top.png` |
| Save/reload | Native sleep saved Spring 2, Year 1; cold Load found it and the farm reloaded; `day-two.png`, `load-menu.png`, `reload-confirmed-top.png` |
| Runtime errors | Zero SMAPI ERROR entries in final game-PID log; `final-game-log.txt` |
| Physical controls | Owner tested movement, bumpers/triggers, tool use, lower Bag/Aim and inventory on this device, then replied **"Seems good"** |

Radiance logged a full-precision warning for `reliefreplay.mgfxo`, then loaded its original-precision fallback. Sprite relief is disabled in Low. No new patch was warranted by the smoke check. Prior full acceptance is in [the visual-mod report](visual-mod-acceptance.md); those broader tests and performance benchmarks were not repeated.

## Save backup and migration

New disposable farm `Thor2Test_454206409` is in the host's separate external Saves directory. Its four files were copied to local `outputs/second-thor-e7eaee41/Thor2Test-backup/`. All four PC SHA256 values matched the device files, recorded in `Thor2Test-backup-sha256.json`. This backs up only the disposable farm.

Later save import: preserve source files, back up complete farm directories to the PC, verify copy hashes, import into the host's separate save area with conflict protection, and validate an isolated copy. Play and host saves do not synchronize. Host Save Export is not a reliable backup route in this version; use readable files and verified PC copies.

## Rollback and resumption

The original Play app remains available independently. To restore mod configuration, save and exit, then import the retained complete Low group ZIP through the host's Groups interface and select it. Do not clear data or uninstall either app. Keep save backups separate from mod archives.

The first Thor's `ThorItemSwitchRelease` rollback group was not transferred or altered. Its retained PC artifacts and prior setup remain available. This second Thor has Default plus the selected six-component production group.

Source fork: `https://github.com/enjihn/stardew-thor-dual-screen`, branch `android/visual-mod-integration`. Accepted implementation commit: `2b97d1bd4f822f8e39ab6f258bd5bae70762fa03`. This deployment adds documentation only and publishes no proprietary files.

Remaining: personal farm backup/import only, separately deferred until source saves are supplied. The game was left available after owner acceptance. Do not restart or interrupt live play when continuing save discovery.
