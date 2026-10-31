# Disposable device fixture

This is a separate test mod, never part of the production project or release ZIP. It only accepts the explicitly named disposable farm identity in FixtureEntry.cs. Edit that guard deliberately for a newly created test farm; never target a real save.

Build with the same local SDK, Android SDK, JDK and MobileReferenceDir as the production project. Package only ThorFixture.dll and manifest.json in a ThorFixture directory, import it through JunimoGate, and select it in the temporary test group. Remove it from the final group.

The fixture polls its own external-files ThorFixture/request.json. Requests contain a unique id and operation. The acknowledgment is persisted before dispatch, so an interrupted request is not replayed. Responses are saved as id.json. tools/fixture_client.py stages a request with a serial-scoped ADB connection and retrieves one bounded response.

Supported operations: state, seed, warp, menu, export, timing-start and timing-stop. Seed replaces only the disposable player's inventory and optionally its money/time/trash level or Junimo reading progression. Menus are native game menus. No fixture method calls production companion code. User actions must be sent through actual Android touch. Export reads only the designated regular saved pair and creates a new archive, without overwriting saves.

Timing samples are intervals between SMAPI Rendered callbacks. They are instrumentation, not hardware input or final fixture-free acceptance. All responses, timing arrays, save archives, request files and raw device evidence stay under ignored private paths.

The optional latency observer runs only when armed on the guarded farm. It
observes the actual lower ImageView touch Up and a matching bitmap signature,
then registers an Android frame commit callback. A requested extra traversal
makes the measurement conservative if registration occurs after the first
matching draw. One refresh period is reported separately as a visibility
estimate. This is device event-to-commit timing, not optical latency. Stop the
observer before performance measurements or lifecycle tests.
