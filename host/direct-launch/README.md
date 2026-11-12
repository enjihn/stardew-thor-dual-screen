# Native direct launch for Thor

Verified on AYN Thor on 2026-11-12. See the [device acceptance and checkpoint](../../docs/android/one-tap-launcher.md).

This personal JunimoGate variant calls the existing LauncherCoordinator and GameSessionRegistry and starts SmapiGameActivity through a normal prepared launch handle. Normal loading and the original intro remain. There is no manager navigation or accessibility automation. Game assets and production mod are unchanged.

Package: `org.junimogate.thor`, version `0.3.1-thor.1`, code 11. It is separate from publisher `org.junimogate.app` and the Play game. Base: JunimoGate `029eafed917ae853d6c0a59330ee273524168296` (v0.3.1); SMAPI `9471959851a75dfa1f2648016b78fc171a5a1999`.

## Use

Tap **Stardew Dual Screen**. In Cocoon Edit App, **Launch on main** must be enabled for correct warm resume. It is configured on the tested Thor. Exit one host before using the other.

Launchers supporting Android static shortcuts expose **Manage mods** on long press. Cocoon's context menu did not expose it during testing. With the game exited, ADB can open it:

```sh
adb -s YOUR_SERIAL shell am start -n org.junimogate.thor/org.junimogate.app.ManageModsShortcutActivity
```

During a live game the manager resumes that session. Its separate trampoline prevents static-shortcut CLEAR_TASK from destroying the game task. A preparation failure also exposes an **Open mod manager** button.

## Rebuild

Use Linux or WSL, a persistent personal signing key, and an owned official Android game. Never publish game references, Content, private saves or signing keys.

```sh
git clone --recursive https://github.com/leontismaro/JunimoGate.git
cd JunimoGate
git checkout 029eafed917ae853d6c0a59330ee273524168296
git submodule update --init --recursive
git apply /absolute/path/to/junimogate-0.3.1-direct-launch.patch
```

Follow the pinned repository's AGENTS.md and docs/build-environment.md. Use LF line endings. Verified toolchain: .NET 9.0.118, Android workload 35.0.7, Temurin 17.0.16+8, API 35/build-tools 35.0.0, Zig 0.14.1. Bootstrap with upstream scripts, then:

```sh
source build/android-env.sh
export JUNIMOGATE_GAME_REFERENCE_DIR=/absolute/path/to/owned-android-game-references
bash build/build-harmony-android.sh
bash build/build-monogame-android.sh
bash build/build-mono-android.sh
bash build/build-cacheflush.sh
"$DOTNET_ROOT/dotnet" build src/JunimoGate.App/JunimoGate.App.csproj \
  --configuration Release --maxcpucount:1 \
  --property:ContinuousIntegrationBuild=true \
  --property:PathMap="$PWD=/_/junimogate" \
  --property:AndroidSdkDirectory="$ANDROID_SDK_ROOT" \
  --property:JavaSdkDirectory="$JAVA_HOME"
```

Sign the ARM64 APK with build-tools apksigner and your personal key. Accepted certificate SHA256: `8e877ccf02ee37a1e740037ce286ab30e959928ba9c56d3c90e173b0ab96f7e4`. It is not the publisher certificate. Future updates need the same key. Run the upstream artifact verifier with this variant's package, DirectLaunchActivity and personal APK path, retaining commercial-payload and runtime exclusions. Check the generated manager trampoline has empty task affinity. The accepted local package passed these gates.

## Install and saves

Install beside the originals. Open Manage mods once, acknowledge the notice, import the production ThorDualScreen ZIP, enable it in a group and select that group. First direct launch prepares the installed Play game. The tested configuration is ThorRelease with exactly one production mod.

Import verified save copies through the manager's save-import UI. The accepted import was a ZIP containing one disposable farm folder and four files. Android rejected direct ADB writes, so the host performed import. Original saves were never moved or overwritten. Installations do not synchronize saves. The landscape import sheet can put its buttons below the viewport; keyboard Tab navigation reaches Import.

## Updates and rollback

The Play Store continues updating the official game. This personal host does not receive publisher-signed JunimoGate updates. Keep its patch and signing key and recheck compatibility when changing host or game.

To roll back, exit this game and open original JunimoGate and its existing group. Both originals remain installed. Back up newer personal-host saves before any future removal. No uninstall or data clearing is needed to switch back. The rejected helper package is disabled and unnecessary.

The game's documented [mobile multiplayer feature](https://www.stardewvalley.net/mobile-multiplayer-guide/) was enabled here. Host/Join menus were observed. This launcher change does not certify a peer connection or multiplayer session.

## License and source

The patch is GPL-3.0-only with JunimoGate's MonoGame linking exception. LICENSE and LICENSE-EXCEPTION accompany it. The host retains its notices and license assets. [Pinned upstream source](https://github.com/leontismaro/JunimoGate/tree/029eafed917ae853d6c0a59330ee273524168296), recursive dependencies and this patch provide its source. See upstream THIRD-PARTY-NOTICES.md for component licenses/source obligations. The mod's license is separate. No commercial game assets are part of this source patch or host APK.
