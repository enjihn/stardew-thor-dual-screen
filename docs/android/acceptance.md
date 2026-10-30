# Android acceptance status

No physical-device acceptance row has passed. There is no candidate or final mod artifact.

The sole connected USB candidate returned `device unauthorized` from the serial-scoped inventory command. Android RSA authorization is required before the device model, Play version, host state, displays, or saves can be inspected. The actual serial and raw error are kept outside Git.

## Gates

| Gate | Status | Evidence or blocker |
|---|---|---|
| 1: Source and environment | pass | Personal fork ancestry and pinned upstream checked; local Android Release library and GameInspector compiled; private paths ignored. |
| 2: Device and unmodified host | blocked | USB authorization required. Device identity, game version, installed host and save persistence unknown. |
| 3: Release host probe | blocked | Requires Gate 2 and actual game references. |
| 4: Lifecycle/input/frame ownership | blocked | Requires real release-host probe. |
| 5: Visual/data modes | blocked | Requires actual mobile metadata and working display path. |
| 6: Consequential actions | blocked | Requires disposable hosted farm and native mobile behavior. |
| 7: Physical acceptance/performance | blocked | Requires exact Release candidate on authorized Thor. |
| 8: Reviewed installed delivery | blocked | No working mobile Release artifact or device evidence yet. |

## Mandatory device matrix

| Area | Status | Observed result |
|---|---|---|
| Installation and identity | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Display and input | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Today | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Gifts | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| People | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Bag and tooltips | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Quick stack | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Craft | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Aim | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Chest | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Shop | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Shipping | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Geodes | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Community Center | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Museum | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Forge | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Sewing/tailoring | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Dialogue and events | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Title and saves | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| End of day | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Tabs/settings | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Lifecycle and hardware | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |
| Release hygiene | blocked | Not run. Requires authorized USB, installed-game identity, and the unmodified-host baseline. |

## Local checks, not device acceptance

- Six Python helper boundary tests: pass. Initial failing run occurred before helper implementation.
- Scratch `net9.0-android35.0` Release library using framework Presentation and ImageView: build pass, zero warnings/errors.
- Pinned JunimoGate GameInspector Release build: pass, zero warnings/errors.
- JunimoGate v0.3.1 APK: publisher SHA256 matches downloaded bytes.
- Selected host references: managed identities inspected; SMAPI 4.5.2.0, MonoGame.Framework 1.0.0.0, JunimoGate.GameHost 1.0.0.0.
- Actual inventory invocation: fail, `device unauthorized`; no successful device properties obtained.
- Original Play app and saves: no authorized device access or modifications performed.

No cold launches, pause/resume cycles, display recreation, hardware input checks, transactions, save/reload journey, performance comparison, or soak have been run.
