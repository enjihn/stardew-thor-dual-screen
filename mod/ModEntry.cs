using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Hardware.Display;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Stopwatch = System.Diagnostics.Stopwatch;
using XColor = Microsoft.Xna.Framework.Color;

namespace DualScreen;

#if !THOR_ANDROID
/// <summary>Hosts the bottom-screen window: draws panels offscreen with the game's own
/// SpriteBatch, copies the pixels into an Android ImageView, and routes touches back.</summary>
public class ModEntry : Mod
{
    // Half the bottom display (1240x1080); shown at 2x with no filtering so pixel art stays crisp.
    public const int W = 620, H = 540;

    Activity activity;
    Presentation window;
    ImageView view;
    readonly RenderTarget2D[] targets = new RenderTarget2D[2];
    SpriteBatch batch;
    Panels panels;
    readonly byte[][] bytes = { new byte[W * H * 4], new byte[W * H * 4] };
    readonly Bitmap[] bitmaps = new Bitmap[2];
    readonly BitmapDrawable[] drawables = new BitmapDrawable[2];
    readonly Java.Nio.ByteBuffer[] pixels = new Java.Nio.ByteBuffer[2];
    readonly ConcurrentQueue<(MotionEventActions action, int x, int y)> touches = new();
    int flip, frames;
    int pendingRedraws, lastKey;
    long drawTicks, readTicks;
    readonly Stopwatch timer = new();

    public override void Entry(IModHelper helper)
    {
        panels = new Panels(helper, Monitor);
        diagnostics = panels.diagnostics = helper.ReadConfig<ModConfig>().Diagnostics;
        helper.Events.GameLoop.GameLaunched += (_, _) => OpenWindow();
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.GameLoop.DayStarted += (_, _) => FullCropCount();
        // Picking a crop by hand isn't a tool swing, but it lands in the bag.
        helper.Events.Player.InventoryChanged += (_, e) => { if (e.IsLocalPlayer) panels.OnInventoryChanged(e.Removed, e.QuantityChanged); };
    }

    void OpenWindow()
    {
        activity = Microsoft.Xna.Framework.Game.Activity;
        if (activity == null) { Monitor.Log("No Android activity; bottom screen disabled.", LogLevel.Warn); return; }

        activity.RunOnUiThread(() =>
        {
            try
            {
                var dm = (DisplayManager)activity.GetSystemService(Android.Content.Context.DisplayService);
                var displays = dm.GetDisplays(DisplayManager.DisplayCategoryPresentation);
                if (displays.Length == 0) { Monitor.Log("No second screen found.", LogLevel.Warn); return; }

                // Before the view exists: Render posts to these as soon as it sees the view.
                ui = new Android.OS.Handler(Android.OS.Looper.MainLooper!);
                pushFrame = new Java.Lang.Runnable(PushFrame);
                exitCheck = new Java.Lang.Runnable(CheckExit);
                window = new Presentation(activity, displays[0]);
                // Never take key focus: the pad must keep driving the game.
                window.Window.AddFlags(WindowManagerFlags.NotFocusable);
                view = new ImageView(activity);
                view.SetScaleType(ImageView.ScaleType.FitXy);
                view.SetBackgroundColor(Android.Graphics.Color.Black);
                view.Touch += (_, e) =>
                {
                    // Consumed here, so the game never sees it and keeps its controller prompts.
                    e.Handled = true;
                    if (view.Width > 0)
                        touches.Enqueue((e.Event.ActionMasked, (int)(e.Event.GetX() * W / view.Width), (int)(e.Event.GetY() * H / view.Height)));
                };
                window.SetContentView(view);
                window.Show();
                ui.PostDelayed(exitCheck, 500);
            }
            catch (Exception ex) { Monitor.Log($"Couldn't open the bottom screen: {ex}", LogLevel.Error); }
        });

        // Hide while the app is paused or the Thor sleeps, so the panel never keeps it awake.
        Microsoft.Xna.Framework.AndroidGameActivity.Paused += (_, _) => window?.Hide();
        Microsoft.Xna.Framework.AndroidGameActivity.Resumed += (_, _) => window?.Show();
    }

    /// <summary>The game is quitting (title screen Exit): close the bottom window too, or it outlives the game.</summary>
    void CloseWindow()
    {
        if (window == null) return;
        var w = window;
        window = null;
        view = null;
        Monitor.Log("Game is exiting; closing the bottom screen.", LogLevel.Trace);
        activity.RunOnUiThread(() => w.Dismiss());
        // Title-screen Exit only closes the game's screen; Cinderbox keeps the process (and its music) running.
        // Nothing is in progress on the title, so end the app once SMAPI has had a moment to flush its log.
        if (Game1.quit)
        {
            Monitor.Log("Exit pressed; ending the app.", LogLevel.Trace);
            // A plain background timer: Android's main thread stops running posted work once the game screen closes.
            System.Threading.Tasks.Task.Delay(1500).ContinueWith(_ => Android.OS.Process.KillProcess(Android.OS.Process.MyPid()));
        }
    }

    /// <summary>The game loop stops when the game exits, so also watch from Android's side: once the game's
    /// activity is finishing, close the bottom window.</summary>
    void CheckExit()
    {
        if (window == null) return;
        if (activity.IsFinishing || activity.IsDestroyed) { CloseWindow(); return; }
        ui.PostDelayed(exitCheck, 500);
    }

    // One Java Runnable each, made once: a new one per frame (RunOnUiThread) meant ~30 new Java objects a
    // second for the garbage collector to cross-check, which costs pauses.
    Android.OS.Handler ui;
    Java.Lang.Runnable pushFrame, exitCheck;
    volatile int readyFlip = -1;

    /// <summary>UI thread: copy the latest read-back frame into its bitmap and show it.</summary>
    void PushFrame()
    {
        var v = view;
        int f = readyFlip;
        if (v == null || f < 0) return;
        var buf = bytes[f];
        var direct = pixels[f] ??= Java.Nio.ByteBuffer.AllocateDirect(buf.Length);
        Marshal.Copy(buf, 0, Android.Runtime.JNIEnv.GetDirectBufferAddress(direct.Handle), buf.Length);
        direct.Rewind();
        var bmp = bitmaps[f] ??= Bitmap.CreateBitmap(W, H, Bitmap.Config.Argb8888);
        bmp.CopyPixelsFromBuffer(direct);
        v.SetImageDrawable(drawables[f] ??= NewDrawable(bmp));
        v.Invalidate();
    }

    void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        if (view == null) return;
        if (!diagnostics) { Update(e); return; }
        long a0 = GC.GetAllocatedBytesForCurrentThread(), t0 = Stopwatch.GetTimestamp();
        try { Update(e); }
        finally
        {
            modBytes += GC.GetAllocatedBytesForCurrentThread() - a0;
            modTicks += Stopwatch.GetTimestamp() - t0;
        }
    }

    bool diagnostics;
    long modBytes, modTicks;

    void FullCropCount()
    {
        long t0 = Stopwatch.GetTimestamp();
        panels.Crops.FullCount();
        if (diagnostics) Monitor.Log($"Crop full count at day start: {(Stopwatch.GetTimestamp() - t0) * 1000.0 / Stopwatch.Frequency:0.00} ms", LogLevel.Trace);
    }
    int renders, keyChanges;

    void Update(UpdateTickedEventArgs e)
    {
        if (Game1.quit) { CloseWindow(); return; }
        if (diagnostics) TrackFrame();
        panels.Tick();

        while (touches.TryDequeue(out var t))
        {
            panels.Touch(t.action, t.x, t.y);
            // Two renders: readback lags one frame behind the draw.
            pendingRedraws = 2;
        }

        if (pendingRedraws > 0) { pendingRedraws--; Render(); return; }
        // Redraw as soon as what the screen shows changes (two renders, as readback lags a frame): checked
        // every other tick so switching items with the triggers tracks smoothly, even on animated screens.
        if (e.IsMultipleOf(2))
        {
            int key = panels.StateKey();
            if (key != lastKey)
            {
                lastKey = key;
                keyChanges++;
                pendingRedraws = 1;
                Render();
                return;
            }
        }
        int interval = panels.AnimationInterval;
        if (interval > 0 ? e.IsMultipleOf((uint)interval) : e.IsMultipleOf(600)) Render();
    }

    void Render()
    {
        var gd = Game1.graphics.GraphicsDevice;
        batch ??= new SpriteBatch(gd);
        // Draw into one target while reading back the other: the previous frame is already
        // finished on the GPU, so the readback doesn't stall on this frame's draws.
        flip ^= 1;
        var draw = targets[flip] ??= new RenderTarget2D(gd, W, H);
        var read = targets[flip ^ 1] ??= new RenderTarget2D(gd, W, H);

        long allocBefore = GC.GetAllocatedBytesForCurrentThread();
        timer.Restart();
        var oldTargets = gd.GetRenderTargets();
        var oldViewport = gd.Viewport;
        try { panels.PrepareOffscreen(gd, batch); }
        catch (Exception ex) { Monitor.LogOnce($"Item info failed: {ex}", LogLevel.Error); }
        gd.SetRenderTarget(draw);
        gd.Clear(new XColor(0, 0, 0));
        batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        try { panels.Draw(batch); }
        catch (Exception ex) { Monitor.LogOnce($"Panel draw failed: {ex}", LogLevel.Error); }
        batch.End();
        gd.SetRenderTargets(oldTargets);
        gd.Viewport = oldViewport;
        long drawn = timer.Elapsed.Ticks;

        int f = flip;
        var buf = bytes[f];
        read.GetData(buf);
        long readDone = timer.Elapsed.Ticks;

        // The Java-side copy runs on the UI thread so the game thread only pays for draw + readback.
        // Each flip has its own byte array, bitmap and drawable, so the next readback can't race it.
        readyFlip = f;
        ui.Post(pushFrame);

        drawTicks += drawn; readTicks += readDone - drawn;
        allocBytes += GC.GetAllocatedBytesForCurrentThread() - allocBefore;
        maxUpdate = Math.Max(maxUpdate, readDone);
        maxDraw = Math.Max(maxDraw, drawn);
        maxRead = Math.Max(maxRead, readDone - drawn);
        renderedThisTick = true;
        renders++;
        if (diagnostics && ++frames % 300 == 0)
        {
            Monitor.Log($"Bottom screen ms/update: draw {drawTicks / frames / 1e4:0.00}, read {readTicks / frames / 1e4:0.00}, worst {maxUpdate / 1e4:0.0} (draw {maxDraw / 1e4:0.0}, read {maxRead / 1e4:0.0}) on {panels.ShowingName}; mod allocates {allocBytes / frames / 1024.0:0.0} KB/update", LogLevel.Trace);
            drawTicks = readTicks = frames = 0;
            allocBytes = 0;
            maxUpdate = maxDraw = maxRead = 0;
        }
    }

    // Stutter diagnostics: every game frame over 50 ms (with what was in it), and the game's allocation rate.
    readonly Stopwatch frameClock = Stopwatch.StartNew(), rateClock = Stopwatch.StartNew();
    long maxUpdate, maxDraw, maxRead, allocBytes, rateBytes;
    bool renderedThisTick;
    int slowFrames, lastG0, lastG1, lastG2, rateG0, rateG1, rateG2;

    void TrackFrame()
    {
        double ms = frameClock.Elapsed.TotalMilliseconds;
        frameClock.Restart();
        int g0 = GC.CollectionCount(0), g1 = GC.CollectionCount(1), g2 = GC.CollectionCount(2);
        if (ms > 50)
        {
            slowFrames++;
            string gc = g2 != lastG2 ? "full" : g1 != lastG1 ? "gen1" : g0 != lastG0 ? "gen0" : "none";
            Monitor.Log($"Slow frame {ms:0} ms on {panels.ShowingName}: bottom-screen update {(renderedThisTick ? "yes" : "no")}, GC {gc}", LogLevel.Trace);
        }
        lastG0 = g0; lastG1 = g1; lastG2 = g2;
        renderedThisTick = false;

        if (rateClock.Elapsed.TotalSeconds >= 10)
        {
            long total = GC.GetTotalAllocatedBytes();
            double secs = rateClock.Elapsed.TotalSeconds;
            Monitor.Log($"[{panels.ShowingName}] mod: {modBytes / secs / 1024:0.0} KB/s, {modTicks * 1000.0 / Stopwatch.Frequency / secs:0.00} ms/s on the game thread, " +
                $"{renders / secs:0.0} renders/s ({keyChanges} from changes) | game: {(total - rateBytes) / secs / 1048576:0.00} MB/s, GCs gen0 {g0 - rateG0} gen1 {g1 - rateG1} full {g2 - rateG2}, slow frames {slowFrames}", LogLevel.Trace);
            rateBytes = total; rateG0 = g0; rateG1 = g1; rateG2 = g2; slowFrames = 0;
            modBytes = modTicks = 0; renders = keyChanges = 0;
            rateClock.Restart();
        }
    }

    BitmapDrawable NewDrawable(Bitmap bmp)
    {
        var d = new BitmapDrawable(activity.Resources, bmp);
        d.SetFilterBitmap(false);
        return d;
    }
}
#else
/// <summary>Mobile game-thread renderer and guarded input integration.</summary>
public class ModEntry : Mod
{
    public const int W = 620, H = 540;
    readonly CompanionState state = new();
    readonly LatestFrameQueue frames = new(W * H * 4);
    readonly ToolbarVisibilityPolicy toolbarPolicy = new();
    MobileToolbar toolbar;
    bool hideToolbar, showItemSwitchOverlay;
    MobileItemSwitchOverlay itemSwitchOverlay;
    MobileHost host;
    Panels panels;
    SpriteBatch batch;
    RenderTarget2D target;
    CompanionScope lastScope;
    int lastKey;
    bool gestureActive, stopped;

    public override void Entry(IModHelper helper)
    {
        panels = new Panels(helper, Monitor);
        var config = helper.ReadConfig<ModConfig>();
        hideToolbar = config.HideToolbarWithCompanion;
        showItemSwitchOverlay = config.ShowItemSwitchOverlay;
        panels.ManualItemSelected += ManualItemSelected;
        helper.Events.Display.RenderedHud += DrawItemSwitchOverlay;
        var build = System.Reflection.CustomAttributeExtensions.GetCustomAttributes<System.Reflection.AssemblyMetadataAttribute>(GetType().Assembly)
            .Single(a => a.Key == "ThorBuildId").Value;
        Monitor.Log($"THOR_ANDROID build={build}; game={typeof(Game1).Assembly.GetName().Version}", LogLevel.Info);
        helper.Events.GameLoop.GameLaunched += Launched;
        helper.Events.GameLoop.UpdateTicked += UpdateMobile;
        helper.Events.GameLoop.SaveLoaded += SaveLoaded;
        helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle;
        helper.Events.GameLoop.DayStarted += DayStarted;
        helper.Events.Display.MenuChanged += MenuChanged;
        helper.Events.Player.InventoryChanged += InventoryChanged;
        helper.Events.Player.Warped += Warped;
    }
    void Launched(object sender, GameLaunchedEventArgs e)
    {
        try
        {
            var unsupported = MobileGameAdapter.ValidateRuntime();
            if (unsupported.Count != 0)
            {
                Monitor.Log("Mobile companion disabled: " + string.Join("; ", unsupported), LogLevel.Error);
                return;
            }
            toolbar = MobileToolbar.TryInstall(Monitor);
            itemSwitchOverlay = MobileItemSwitchOverlay.TryInstall(toolbarPolicy, OverlayEligible, Monitor);
            host = new MobileHost(MobileHost.ResolveGameActivity(), Monitor, state, frames, toolbarPolicy);
        }
        catch (Exception ex) { Monitor.Log($"Mobile companion initialization failed: {ex}", LogLevel.Error); }
    }
    void InvalidateInput()
    {
        itemSwitchOverlay?.Clear();
        state.Invalidate(); frames.Invalidate();
        panels.CancelGesture(); gestureActive = false;
    }
    void SaveLoaded(object sender, SaveLoadedEventArgs e) { ResetPanels(); panels.Crops.FullCount(); }
    void ReturnedToTitle(object sender, ReturnedToTitleEventArgs e) => ResetPanels();
    void ResetPanels()
    {
        InvalidateInput(); panels.DisposeOwnedTargets();
        panels.ManualItemSelected -= ManualItemSelected;
        panels = new Panels(Helper, Monitor);
        panels.ManualItemSelected += ManualItemSelected;
    }
    void DayStarted(object sender, DayStartedEventArgs e) { InvalidateInput(); panels.Crops.FullCount(); }
    void MenuChanged(object sender, MenuChangedEventArgs e) => InvalidateInput();
    void Warped(object sender, WarpedEventArgs e) { if (e.IsLocalPlayer) InvalidateInput(); }
    void InventoryChanged(object sender, InventoryChangedEventArgs e)
    {
        if (!e.IsLocalPlayer) return;
        InvalidateInput();
        panels.OnInventoryChanged(e.Removed, e.QuantityChanged);
    }
    CompanionScope Scope() => state.SetContext(
        Game1.activeClickableMenu is StardewValley.Menus.TitleMenu
            ? StardewValley.Menus.TitleMenu.subMenu ?? Game1.activeClickableMenu
            : Game1.activeClickableMenu,
        panels.InputLayoutKey);
    void UpdateMobile(object sender, UpdateTickedEventArgs e)
    {
        if (stopped) return;
        if (Game1.quit) { Stop(); return; }
        bool pressed = Game1.input.GetMouseState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
        toolbar?.SetHidden(toolbarPolicy.Step(hideToolbar, Context.IsWorldReady, pressed));
        itemSwitchOverlay?.Tick();
        if (host?.Ready != true) return;
        try
        {
            var scope = Scope();
            bool redraw = !state.IsCurrent(lastScope);
            if (redraw) { panels.CancelGesture(); gestureActive = false; }
            while (state.TryTakeTouch(out var touch))
            {
                if (touch.Action == TouchAction.Down) gestureActive = true;
                if (!gestureActive && touch.Action != TouchAction.Cancel) continue;
                panels.Touch(touch.Action switch
                {
                    TouchAction.Down => MotionEventActions.Down,
                    TouchAction.Move => MotionEventActions.Move,
                    TouchAction.Up => MotionEventActions.Up,
                    _ => MotionEventActions.Cancel
                }, touch.X, touch.Y);
                if (touch.Action is TouchAction.Up or TouchAction.Cancel) gestureActive = false;
                scope = Scope();
                redraw = true;
            }
            panels.Tick();
            itemSwitchOverlay?.Tick();
            scope = Scope();
            int key = panels.StateKey();
            int interval = panels.AnimationInterval;
            if (redraw || key != lastKey || (interval > 0 && e.IsMultipleOf((uint)interval)) || e.IsMultipleOf(600))
            {
                if (RenderMobile(scope)) { lastKey = key; lastScope = scope; }
            }
        }
        catch (Exception ex)
        {
            toolbarPolicy.Unavailable(); host?.RenderFailed();
            InvalidateInput();
            Monitor.LogOnce($"Mobile companion update failed: {ex}", LogLevel.Error);
        }
    }
    bool OverlayEligible() => showItemSwitchOverlay && hideToolbar && toolbar?.IsHidden == true
        && host?.Ready == true && Context.IsWorldReady && Game1.IsHudDrawn
        && Game1.activeClickableMenu == null && !Game1.dialogueUp && !Game1.eventUp
        && !Game1.fadeToBlack && !Game1.globalFade && !panels.IsBagShowing;
    void ManualItemSelected(int before) => itemSwitchOverlay?.ManualSwitch(before);
    void DrawItemSwitchOverlay(object sender, RenderedHudEventArgs e) => itemSwitchOverlay?.Draw(e.SpriteBatch);

    bool RenderMobile(CompanionScope scope)
    {
        var frame = frames.AcquireWriter();
        if (frame == null) return false;
        bool published = false;
        try
        {
            var gd = Game1.graphics.GraphicsDevice;
            batch ??= new SpriteBatch(gd);
            target ??= new RenderTarget2D(gd, W, H);
            var oldTargets = gd.GetRenderTargets();
            var oldViewport = gd.Viewport;
            var oldScissor = gd.ScissorRectangle;
            var oldBlend = gd.BlendState;
            var oldDepth = gd.DepthStencilState;
            var oldRaster = gd.RasterizerState;
            var oldSampler = gd.SamplerStates[0];
            try
            {
                panels.PrepareOffscreen(gd, batch);
                gd.SetRenderTarget(target);
                gd.Clear(XColor.Black);
                batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
                try { panels.Draw(batch); }
                finally { batch.End(); }
            }
            finally
            {
                gd.SetRenderTargets(oldTargets);
                gd.Viewport = oldViewport;
                gd.ScissorRectangle = oldScissor;
                gd.BlendState = oldBlend;
                gd.DepthStencilState = oldDepth;
                gd.RasterizerState = oldRaster;
                gd.SamplerStates[0] = oldSampler;
            }
            target.GetData(frame.Bytes);
            frame.BagShowing = panels.IsBagShowing;
            host.Submit(frame, scope);
            published = true;
            return true;
        }
        finally { if (!published) frames.Abandon(frame); }
    }
    void Stop()
    {
        if (stopped) return;
        stopped = true;
        itemSwitchOverlay?.Dispose();
        toolbar?.Dispose();
        state.Dispose(); host?.Dispose(); frames.Dispose();
        panels.ManualItemSelected -= ManualItemSelected;
        Helper.Events.Display.RenderedHud -= DrawItemSwitchOverlay;
        Helper.Events.GameLoop.GameLaunched -= Launched;
        Helper.Events.GameLoop.UpdateTicked -= UpdateMobile;
        Helper.Events.GameLoop.SaveLoaded -= SaveLoaded;
        Helper.Events.GameLoop.ReturnedToTitle -= ReturnedToTitle;
        Helper.Events.GameLoop.DayStarted -= DayStarted;
        Helper.Events.Display.MenuChanged -= MenuChanged;
        Helper.Events.Player.InventoryChanged -= InventoryChanged;
        Helper.Events.Player.Warped -= Warped;
        panels?.DisposeOwnedTargets();
        batch?.Dispose(); target?.Dispose();
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing) Stop();
        base.Dispose(disposing);
    }
}
#endif

/// <summary>Saved to config.json: tab order, hidden tabs and the tab last open.</summary>
public class ModConfig
{
    public List<string> TabOrder { get; set; } = new() { "Today", "Gifts", "People", "Bag", "Craft", "Aim" };
    public List<string> HiddenTabs { get; set; } = new();
    public string LastTab { get; set; } = "Today";
    /// <summary>Write performance numbers to the SMAPI log every 10 s (for troubleshooting stutter).</summary>
    public bool Diagnostics { get; set; } = false;
    public bool HideToolbarWithCompanion { get; set; } = true;
    public bool ShowItemSwitchOverlay { get; set; } = true;
}
