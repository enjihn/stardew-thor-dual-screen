# Disposable device fixture

This is a separate test mod, never part of the production project or release ZIP. It only accepts the explicitly named disposable farm identity in FixtureEntry.cs. Edit that guard deliberately for a newly created test farm; never target a real save.

Build with the same local SDK, Android SDK, JDK and MobileReferenceDir as the production project. Package only ThorFixture.dll and manifest.json in a ThorFixture directory, import it through JunimoGate, and select it in the temporary test group. Remove it from the final group.

The fixture polls its own external-files ThorFixture/request.json. Requests contain a unique id and operation. The acknowledgment is persisted before dispatch, so an interrupted request is not replayed. Responses are saved as id.json. tools/fixture_client.py stages a request with a serial-scoped ADB connection and retrieves one bounded response.

Supported operations: state, seed, warp, menu, export, timing-start and timing-stop. Seed replaces only the disposable player's inventory and optionally its money/time/trash level or Junimo reading progression. Menus are native game menus. No fixture method calls production companion code. User actions must be sent through actual Android touch. Export reads only the designated regular saved pair and creates a new archive, without overwriting saves.

Timing samples are intervals between SMAPI Rendered callbacks. They are instrumentation, not hardware input or final fixture-free acceptance. All responses, timing arrays, save archives, request files and raw device evidence stay under ignored private paths.

The optional latency observer matches the lower ImageView bitmap in PreDraw and
registers its frame-commit callback before Android captures callbacks in
performDraw. It measures native touch Up to the first matching committed frame,
plus one refresh period as a visibility estimate. This is not optical latency.
Earlier0.18/0.19 Draw registration intentionally added a traversal and remains
archived separately. Stop the observer before performance or lifecycle tests.
Source: [Android13 ViewRootImpl](https://raw.githubusercontent.com/aosp-mirror/platform_frameworks_base/android-13.0.0_r74/core/java/android/view/ViewRootImpl.java).

The collect operation requests managed and Java garbage collection only for
resource-retention diagnosis. It is excluded from measured performance and
latency runs. A resulting drop proves those objects were collectable, not that
normal runtime memory behavior has passed acceptance.
