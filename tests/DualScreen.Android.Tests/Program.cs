using DualScreen;

var tests = new (string Name, Action Body)[]
{
    ("Notice requires an actual manual switch and fades at the deadline", () =>
    {
        var n = new ItemSwitchNotice(); var item = new object();
        Check(n.Opacity(1, item, 4, 100, true) == 0, "spontaneous notice");
        n.ManualSwitch(0, 1, item, 4, 100, true);
        Check(n.Opacity(1, item, 4, 1399, true) == 1, "early fade");
        Check(Math.Abs(n.Opacity(1, item, 4, 1500, true) - .5f) < .001f, "fade timing");
        Check(n.Opacity(1, item, 4, 1600, true) == 0, "expired notice");
    }),
    ("Repeated switches extend but identical selection does not", () =>
    {
        var n = new ItemSwitchNotice(); var item = new object();
        n.ManualSwitch(0, 1, item, 1, 0, true);
        n.ManualSwitch(1, 2, item, 1, 1000, true);
        n.ManualSwitch(2, 2, item, 1, 2400, true);
        Check(n.Opacity(2, item, 1, 2450, true) > 0, "repeat did not extend");
        Check(n.Opacity(2, item, 1, 2500, true) == 0, "same slot extended timer");
    }),
    ("Automatic changes and replaced item identity clear stale notices", () =>
    {
        var n = new ItemSwitchNotice(); var item = new object();
        n.ManualSwitch(0, 1, item, 1, 0, true);
        Check(n.Opacity(2, item, 1, 100, true) == 0, "automatic slot change retained notice");
        Check(n.Opacity(1, item, 1, 200, true) == 0, "return to old slot revived notice");
        n.ManualSwitch(0, 1, item, 1, 300, true);
        Check(n.Opacity(1, new object(), 1, 400, true) == 0, "inventory replacement retained notice");
    }),
    ("Bag and lifecycle ineligibility consume notices without replay", () =>
    {
        var n = new ItemSwitchNotice(); var item = new object();
        n.ManualSwitch(0, 1, item, 1, 0, true);
        Check(n.Opacity(1, item, 1, 100, false) == 0, "ineligible notice visible");
        Check(n.Opacity(1, item, 1, 200, true) == 0, "leaving Bag revived notice");
        n.ManualSwitch(1, 2, item, 1, 300, false);
        Check(n.Opacity(2, item, 1, 400, true) == 0, "ineligible switch queued");
        n.ManualSwitch(1, 2, item, 1, 500, true); n.Clear();
        Check(n.Opacity(2, item, 1, 600, true) == 0, "lifecycle clear ineffective");
    }),
    ("Companion recovery cannot revive a notice from a prior epoch", () =>
    {
        var n = new ItemSwitchNotice(); var item = new object();
        n.ManualSwitch(0, 1, item, 1, 0, true);
        Check(n.Opacity(1, item, 2, 100, true) == 0, "old epoch survived recovery");
    }),
    ("Mini row clamps consecutive slots at inventory boundaries", () =>
    {
        Check(ItemSwitchNotice.FirstSlot(0, 36) == 0, "first boundary");
        Check(ItemSwitchNotice.FirstSlot(13, 36) == 11, "middle selection not centered");
        Check(ItemSwitchNotice.FirstSlot(35, 36) == 31, "last boundary");
        Check(ItemSwitchNotice.FirstSlot(1, 3) == 0, "small inventory boundary");
    }),
    ("A committed Bag frame rejected as stale makes prior nonBag knowledge unknown", () =>
    {
        var p = new ToolbarVisibilityPolicy(); long epoch = p.BeginWindow();
        p.Present(epoch, 1, false);
        // Bag reached Android, but the active panel/drawable moved on before its callback.
        p.Present(epoch, 2, true, current: false);
        Check(p.Presentation.BagShowing == null, "old nonBag verdict survived an unrecorded Bag commit");
        Check(p.Presentation.Available, "overlay metadata changed existing toolbar fallback");
        Check(!p.IsPresented(epoch, 2), "unknown panel prevented a fresh commit");
        p.Present(epoch, 3, false);
        Check(p.Presentation.BagShowing == false, "fresh nonBag commit did not recover");
        p.Unavailable(); long next = p.CurrentEpoch; p.Present(next, 4, false);
        p.Present(epoch, 5, true, current: false);
        Check(p.Presentation.BagShowing == false, "old window erased fresh metadata");
    }),
    ("Presentation metadata rejects stale scope and old window commits", () =>
    {
        var p = new ToolbarVisibilityPolicy(); long epoch = p.BeginWindow();
        Check(p.Presentation.BagShowing == null, "unknown panel guessed");
        p.Present(epoch, 4, false);
        Check(p.Presentation.Available && p.Presentation.BagShowing == false, "nonBag not published");
        p.Present(epoch, 5, true); p.Present(epoch, 4, false);
        Check(p.Presentation.BagShowing == true, "late nonBag overwrote Bag");
        Check(p.IsPresented(epoch, 5) && !p.IsPresented(epoch, 6), "new panel skipped commit");
        p.Unavailable(); p.Present(epoch, 6, false);
        Check(!p.Presentation.Available && p.Presentation.BagShowing == null, "stale window restored panel");
        p.Present(p.CurrentEpoch, 7, false);
        Check(p.Presentation.BagShowing == false, "recovery panel failed");
    }),
    ("Partial toolbar patch installation fails open and rolls back its own patches", () =>
    {
        var monitor = new StardewModdingAPI.TestMonitor();
        HarmonyLib.Harmony.Attempts = 0; HarmonyLib.Harmony.FailAt = 3;
        var toolbar = MobileToolbar.TryInstall(monitor);
        Check(toolbar == null && monitor.Warning?.Contains("Controlled patch installation failure") == true, "failure not reported");
        Check(HarmonyLib.Harmony.Installed.Count == 0, "partial interception remains");
        Check(HarmonyLib.Harmony.RemovedOwner == "Hung.ThorDualScreen.ToolbarVisibility", "wrong patch owner removed");
        HarmonyLib.Harmony.Attempts = 0; HarmonyLib.Harmony.FailAt = 0;
        using var recovered = MobileToolbar.TryInstall(monitor);
        Check(recovered != null && HarmonyLib.Harmony.Installed.Count == 5, "clean initialization failed");
        var native = StardewValley.Menus.Toolbar._instance;
        native._nextToolIndex = 7; native._startIndex = 4; native._drawStartIndex = 9;
        native._startTapPositionX = 30; StardewValley.Menus.Toolbar.toolbarPressed = true;
        recovered!.SetHidden(true); recovered.SetHidden(false);
        Check(native._nextToolIndex == 7 && native._startIndex == 4, "deferred selection changed");
        Check(native._drawStartIndex == 4 && native._startTapPositionX == -1 && !StardewValley.Menus.Toolbar.toolbarPressed, "gesture not cleared");
    }),
    ("A failed presentation can recover with a fresh frame without reopening", () =>
    {
        var policy = new ToolbarVisibilityPolicy();
        var old = policy.BeginWindow(); policy.Present(old);
        Check(policy.Step(true, true, false), "first frame not available");
        policy.Unavailable();
        Check(!policy.Step(true, true, false), "failure did not restore toolbar");
        policy.Present(old);
        Check(!policy.Step(true, true, false), "stale completion accepted");
        policy.Present(policy.CurrentEpoch);
        Check(policy.Step(true, true, false), "fresh frame did not recover");
    }),
    ("A pending Android callback stays alive and cannot clear a replacement", () =>
    {
        var slot = new FrameCommitSlot<TestCallback>();
        var old = new TestCallback(); slot.Set(old);
        slot.Cancel(_ => false);
        Check(old.Disposals == 0 && slot.Current == null, "pending callback disposed early");
        var next = new TestCallback(); slot.Set(next);
        slot.Complete(old);
        Check(old.Disposals == 1 && ReferenceEquals(slot.Current, next), "stale callback cleared replacement");
        slot.Cancel(_ => true);
        Check(next.Disposals == 1 && slot.Current == null, "canceled callback not released");
        var final = new TestCallback(); slot.Set(final); slot.Complete(final);
        Check(final.Disposals == 1 && slot.Current == null, "completed callback retained");
    }),
    ("Toolbar hides only after a current companion frame and a released gesture", () =>
    {
        var policy = new ToolbarVisibilityPolicy();
        var epoch = policy.BeginWindow();
        Check(!policy.Step(true, true, false), "hidden before frame");
        policy.Present(epoch);
        Check(!policy.Step(true, true, true), "changed during touch");
        Check(!policy.Step(true, true, false), "changed on first release frame");
        Check(policy.Step(true, true, false), "not hidden after release");
        policy.Unavailable();
        Check(policy.Step(true, true, true), "restored during touch");
        Check(policy.Step(true, true, false), "restored before release settled");
        Check(!policy.Step(true, true, false), "failed to restore");
        policy.Present(epoch);
        Check(!policy.Step(true, true, false), "stale frame restored availability");
        var next = policy.BeginWindow(); policy.Present(next);
        Check(policy.Step(true, true, false), "new frame did not enable hiding");
        Check(!policy.Step(false, true, false), "configuration did not restore toolbar");
    }),
    ("Chest reorder invalidates a touch without replacing its menu", () =>
    {
        var menu = new object(); var wood = new object(); var stone = new object();
        var slots = new SlotIdentityRevision(); var state = new CompanionState();
        var shown = state.SetContext(menu, slots.Observe(new[] { wood, stone }));
        state.SetContext(menu, slots.Observe(new[] { wood, stone }));
        Check(state.IsCurrent(shown), "unchanged slots invalidated");
        state.Enqueue(new CompanionTouch(shown, TouchAction.Down, 40, 80));
        Check(state.TryTakeTouch(out _), "valid press rejected");
        state.SetContext(menu, slots.Observe(new[] { stone, wood }));
        state.Enqueue(new CompanionTouch(shown, TouchAction.Up, 40, 80));
        Check(!state.TryTakeTouch(out _), "release transferred the replacement chest item");
        var reordered = state.SetContext(menu, slots.Observe(new[] { stone, wood }));
        state.SetContext(menu, slots.Observe(new object[] { stone }));
        Check(!state.IsCurrent(reordered), "removed slot retained its old scope");
    }),
    ("Quantity selection rejects replacement item and menu", () =>
    {
        var menu = new object(); var item = new object();
        var selection = new ActionSelection(menu, item);
        Check(!selection.TryQuantity(menu, new object(), 2, 5, out _), "replacement item accepted");
        Check(!selection.TryQuantity(new object(), item, 2, 5, out _), "replacement menu accepted");
        Check(!new ActionSelection(null, null).TryQuantity(null, null, 1, 1, out _), "empty selection accepted");
    }),
    ("Quantity selection rejects canceled amount and clamps current stack", () =>
    {
        var item = new object(); var selection = new ActionSelection(null, item);
        foreach (int count in new[] { 0, -1, int.MinValue })
            Check(!selection.TryQuantity(null, item, count, 8, out _), "nonpositive quantity accepted");
        Check(!selection.TryQuantity(null, item, 2, 0, out _), "empty stack accepted");
        Check(selection.TryQuantity(null, item, int.MaxValue, 3, out int quantity) && quantity == 3, "current quantity not clamped");
        Check(selection.TryQuantity(null, item, 2, 3, out quantity) && quantity == 2, "valid quantity changed");
    }),
    ("Queued touch cannot cross menu or layout replacement", () =>
    {
        var state = new CompanionState();
        var old = state.SetContext(new object(), 1);
        state.Enqueue(new CompanionTouch(old, TouchAction.Up, 40, 80));
        state.SetContext(new object(), 1);
        Check(!state.TryTakeTouch(out _), "old menu touch escaped");
        var sameMenu = new object();
        old = state.SetContext(sameMenu, 1);
        state.Enqueue(new CompanionTouch(old, TouchAction.Down, 40, 80));
        state.SetContext(sameMenu, 2);
        Check(!state.TryTakeTouch(out _), "old layout touch escaped");
    }),
    ("Touch uses displayed scope, not a later game context", () =>
    {
        var state = new CompanionState();
        var shown = state.SetContext(new object(), 0);
        state.SetContext(new object(), 0);
        state.Enqueue(new CompanionTouch(shown, TouchAction.Up, 20, 30));
        Check(!state.TryTakeTouch(out _), "touch on old image targeted new menu");
    }),
    ("New session and disposal invalidate queued callbacks", () =>
    {
        var state = new CompanionState();
        var first = state.SetContext(null, 0);
        state.Enqueue(new CompanionTouch(first, TouchAction.Up, 10, 20));
        state.Invalidate();
        Check(!state.IsCurrent(first) && !state.TryTakeTouch(out _), "old session survived");
        var current = state.SetContext(null, 0);
        state.Dispose();
        state.Enqueue(new CompanionTouch(current, TouchAction.Up, 10, 20));
        Check(!state.IsCurrent(current) && !state.TryTakeTouch(out _), "disposed callback survived");
    }),
    ("Current touches preserve order and cancellation", () =>
    {
        var state = new CompanionState();
        var scope = state.SetContext(null, 0);
        foreach (var action in new[] { TouchAction.Down, TouchAction.Move, TouchAction.Cancel })
            state.Enqueue(new CompanionTouch(scope, action, 15, 20));
        foreach (var expected in new[] { TouchAction.Down, TouchAction.Move, TouchAction.Cancel })
            Check(state.TryTakeTouch(out var touch) && touch.Action == expected, "touch order changed");
    }),
    ("Drawable transform respects insets, edges and outside cancel", () =>
    {
        var transform = new DrawableTransform(20, 30, 1240, 1080, 620, 540);
        Check(transform.TryMap(20, 30, out var x, out var y) && x == 0 && y == 0, "origin");
        Check(transform.TryMap(1259, 1109, out x, out y) && x == 619 && y == 539, "far edge");
        Check(!transform.TryMap(1260, 1110, out _, out _), "outside accepted");
        Check(!transform.TryMap(19, 30, out _, out _), "inset accepted");
        Check(!new DrawableTransform(0, 0, 0, 0, 620, 540).TryMap(0, 0, out _, out _), "unlaid-out view accepted");
        Check(transform.MapAction(TouchAction.Up, 1260, 50) == TouchAction.Cancel, "outside release was actionable");
        Check(transform.MapAction(TouchAction.Cancel, 500, 500) == TouchAction.Cancel, "cancel lost");
    }),
    ("Only newest pending frame survives slow UI", () =>
    {
        var queue = new LatestFrameQueue(4);
        var first = queue.AcquireWriter()!; first.Bytes[0] = 1; queue.Publish(first, 10);
        var flight = queue.TakeLatest()!;
        var second = queue.AcquireWriter()!; second.Bytes[0] = 2; queue.Publish(second, 10);
        var third = queue.AcquireWriter()!; third.Bytes[0] = 3; queue.Publish(third, 10);
        Check(queue.PendingCount == 1, "backlog grew");
        Check(queue.TakeLatest() is null, "second transfer started while first active");
        Check(flight.Bytes[0] == 1, "in-flight memory was overwritten");
        queue.Complete(flight);
        var newest = queue.TakeLatest()!;
        Check(newest.Bytes[0] == 3 && newest.Generation == 10, "stale pending frame rendered");
        queue.Complete(newest);
        Check(queue.TakeLatest() is null, "extra frame remained");
    }),
    ("Invalidation drops pending but keeps transfer owned until completion", () =>
    {
        var queue = new LatestFrameQueue(4);
        var writer = queue.AcquireWriter()!; writer.Bytes[0] = 9; queue.Publish(writer, 1);
        var active = queue.TakeLatest()!;
        writer = queue.AcquireWriter()!; queue.Publish(writer, 1);
        queue.Invalidate();
        Check(queue.TakeLatest() is null && queue.PendingCount == 0, "pending survived invalidation");
        writer = queue.AcquireWriter()!;
        Check(!ReferenceEquals(writer, active), "active buffer reused before completion");
        queue.Abandon(writer);
        queue.Dispose();
        Check(active.Bytes[0] == 9 && queue.AcquireWriter() is null, "disposed active storage invalidated early");
        queue.Complete(active);
    }),
    ("Late producer cannot publish across invalidation", () =>
    {
        var queue = new LatestFrameQueue(4);
        var writer = queue.AcquireWriter()!;
        queue.Invalidate();
        Check(!queue.Publish(writer, 1), "old producer posted a callback");
        Check(queue.TakeLatest() is null, "old producer repopulated pending frame");
    })
};
int failed = 0;
foreach (var (name, body) in tests)
{
    try { body(); Console.WriteLine($"PASS {name}"); }
    catch (Exception ex) { failed++; Console.WriteLine($"FAIL {name}: {ex.Message}"); }
}
Console.WriteLine($"{tests.Length - failed}/{tests.Length} passed");
return failed == 0 ? 0 : 1;

static void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

sealed class TestCallback : IDisposable { public int Disposals; public void Dispose() => Disposals++; }
