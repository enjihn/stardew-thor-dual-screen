# Thor Dual Screen Android

Required runtime: official Google Play Stardew Valley 1.6.15.3 (code 245,
ARM64), publisher JunimoGate 0.3.1 (code 11), SMAPI 4.5.2-junimogate.41,
and AYN Thor Android 13. The personal branded host 0.3.1-thor.2 uses the same
game and mod runtime. This toolbar update requires no host changes.
Other combinations are unverified.
The upstream UI uses the installed game's original textures and fonts.

## Install and launch

1. Preserve your saves. Use a disposable hosted farm or an independently
   verified copy until you have checked your own mod combination.
2. Copy this ZIP to Downloads on the Thor. Open JunimoGate, then Mods,
   Global Mods, Import. Select the ZIP and confirm the import.
3. Expand this exact version in the library, choose Add to group, and select
   your dedicated group. Confirm replacement of the older revision if asked.
4. Under Groups, verify that Thor Dual Screen Android is enabled and its
   version matches this ZIP. Select that group on Home.
5. Exit the hosted game to its title before stopping it. Restart JunimoGate
   after changing mod selection, then use its central game launch button.
   The ordinary Play game icon launches the original game without this mod.

The top display keeps gameplay and controller focus. Touch the lower tabs
for companion panels. Swipe the tab strip to reach hidden tabs. Tabs settings
can hide or reorder panels. Native menus still use the top display where
their original completion controls are required, including forge/tailoring.

## Saves and rollback

Version 0.5.2-android.2 hides the native toolbar only while the companion has
successfully presented a frame. It restores the toolbar after a companion
failure, pause or missing display, after the current touch is released.
Clock, money, journal, menu buttons, health and energy retain their native rules.
The companion Bag can select items in every inventory row.

To keep the ordinary toolbar, set `HideToolbarWithCompanion` to `false` in
this mod's `config.json` using the host's group file editor, then restart the
hosted game. Setting it to `true` enables automatic hiding again. The previous
accepted 0.5.2-android.1 ZIP and ThorRelease group are the rollback baseline.

The Play app and its real saves remain separate. This mod adds no required
save format. Do not clear app data or uninstall either app for rollback.
Exit to title, disable this mod in the selected group (or select a previously
verified revision), restart the host, and launch again. Keep a verified save
backup separately from the mod ZIP.

JunimoGate 0.3.1 Save Export produced a zero-byte archive on the tested device;
do not rely on it without checking the file. Save Import was tested with a
verified disposable save ZIP. Never replace a real save with a test fixture.

Build identity and current acceptance status are recorded in the source
branch's docs/android/compatibility.json and docs/android/acceptance.md.
Toolbar update source: https://github.com/enjihn/stardew-thor-dual-screen/tree/android/hide-top-toolbar
