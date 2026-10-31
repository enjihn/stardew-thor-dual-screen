# Thor Dual Screen Android

Required runtime: official Google Play Stardew Valley 1.6.15.3 (code 245,
ARM64), publisher JunimoGate 0.3.1 (code 11), SMAPI 4.5.2-junimogate.41,
and AYN Thor Android 13. Other combinations are unverified. No host patch is used.
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

The Play app and its real saves remain separate. This mod adds no required
save format. Do not clear app data or uninstall either app for rollback.
Exit to title, disable this mod in the selected group (or select a previously
verified revision), restart the host, and launch again. Keep a verified save
backup separately from the mod ZIP.

JunimoGate 0.3.1 Save Export produced a zero-byte archive on the tested device;
do not rely on it without checking the file. Save Import was tested with a
verified disposable save ZIP. Its landscape dialog may need a temporarily
smaller Android display size to expose the Import button. Restore your normal
display size afterward. Never replace a real save with a test fixture.

Build identity and current acceptance status are recorded in the source
branch's docs/android/compatibility.json and docs/android/acceptance.md.
Source: https://github.com/enjihn/stardew-thor-dual-screen/tree/android/thor-mobile
