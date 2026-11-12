#if THOR_ANDROID
#nullable enable
using Android.App;
using Android.Content;
using Android.Hardware.Display;
using Android.Views;
using System.Reflection;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Widget;
using StardewModdingAPI;
using System.Runtime.InteropServices;

namespace DualScreen;

internal sealed class MobileHost : IDisposable
{
    readonly Activity activity;
    readonly IMonitor monitor;
    readonly CompanionState state;
    readonly LatestFrameQueue frames;
    readonly ToolbarVisibilityPolicy toolbar;
    long presentedEpoch;
    readonly FrameCommitSlot<Java.Lang.Runnable> commits = new();
    ViewTreeObserver? commitTree;
    readonly Handler ui = new(Looper.MainLooper!);
    readonly Java.Lang.Runnable pulse, push;
    readonly Bitmap?[] bitmaps = new Bitmap[2];
    readonly BitmapDrawable?[] drawables = new BitmapDrawable[2];
    Java.Nio.ByteBuffer? direct;
    Presentation? window;
    ImageView? view;
    CompanionScope presented, gesture;
    bool hasImage, disposed, paused, hidden;
    int pointer = -1, bitmapIndex;
    public bool Ready { get; private set; }

    public MobileHost(Activity activity, IMonitor monitor, CompanionState state, LatestFrameQueue frames, ToolbarVisibilityPolicy toolbar)
    {
        this.activity = activity; this.monitor = monitor; this.state = state; this.frames = frames; this.toolbar = toolbar;
        pulse = new Java.Lang.Runnable(Pulse);
        push = new Java.Lang.Runnable(PushFrame);
        Microsoft.Xna.Framework.AndroidGameActivity.Paused += OnPaused;
        Microsoft.Xna.Framework.AndroidGameActivity.Resumed += OnResumed;
        ui.Post(pulse);
    }

    public void Submit(LatestFrameQueue.Frame frame, CompanionScope scope)
    {
        frame.Scope = scope;
        frame.PresentationEpoch = toolbar.CurrentEpoch;
        if (frames.Publish(frame, scope.Generation)) ui.Post(push);
    }

    void Pulse()
    {
        if (disposed || paused) return;
        if (activity.IsFinishing || activity.IsDestroyed) { Dispose(); return; }
        try
        {
            var display = SelectCompanionDisplay(activity);
            if (display == null) CloseWindow();
            else if (window == null || window.Display?.DisplayId != display.DisplayId || (!window.IsShowing && !hidden))
            {
                CloseWindow();
                window = new Presentation(activity, display);
                window.Window!.AddFlags(WindowManagerFlags.NotFocusable);
                view = new ImageView(window.Context);
                view.SetScaleType(ImageView.ScaleType.FitXy);
                view.SetBackgroundColor(Color.Black);
                view.Touch += OnTouch;
                view.LayoutChange += OnLayout;
                window.SetContentView(view);
                window.Show();
                commitTree = view.ViewTreeObserver ?? throw new InvalidOperationException("Companion view observer unavailable.");
                commitTree.PreDraw += OnPreDraw;
                toolbar.BeginWindow();
                hidden = false;
                Ready = true;
                monitor.Log($"Companion opened on logical display {display.DisplayId}.", LogLevel.Info);
            }
            else if (!Ready)
            {
                window.Show();
                toolbar.BeginWindow();
                hidden = false;
                Ready = true;
            }
        }
        catch (Exception ex) { CloseWindow(); monitor.LogOnce($"Companion window unavailable: {ex}", LogLevel.Error); }
        if (!disposed && !paused) ui.PostDelayed(pulse, 500);
    }

    void OnLayout(object? sender, View.LayoutChangeEventArgs e)
    {
        if (e.Right - e.Left == e.OldRight - e.OldLeft && e.Bottom - e.Top == e.OldBottom - e.OldTop) return;
        Invalidate();
    }

    void OnTouch(object? sender, View.TouchEventArgs e)
    {
        e.Handled = true;
        if (!Ready || !hasImage || view == null || e.Event is not { } motion) return;
        var action = motion.ActionMasked;
        if (action == MotionEventActions.Down)
        { pointer = motion.GetPointerId(0); gesture = presented; }
        if (pointer < 0) return;
        int index = motion.FindPointerIndex(pointer);
        var transform = new DrawableTransform(view.PaddingLeft, view.PaddingTop,
            view.Width - view.PaddingLeft - view.PaddingRight, view.Height - view.PaddingTop - view.PaddingBottom,
            ModEntry.W, ModEntry.H);
        TouchAction mapped = action switch
        {
            MotionEventActions.Down => TouchAction.Down,
            MotionEventActions.Move => TouchAction.Move,
            MotionEventActions.Up => TouchAction.Up,
            _ => TouchAction.Cancel
        };
        int x = 0, y = 0;
        if (motion.PointerCount != 1 || index < 0 || !transform.TryMap(motion.GetX(index), motion.GetY(index), out x, out y))
            mapped = TouchAction.Cancel;
        state.Enqueue(new CompanionTouch(gesture, mapped, x, y));
        if (mapped is TouchAction.Up or TouchAction.Cancel) pointer = -1;
    }

    void PushFrame()
    {
        var frame = frames.TakeLatest();
        if (frame == null) return;
        try
        {
            if (disposed || paused || !Ready || view == null || !state.IsCurrent(frame.Scope)) return;
            direct ??= Java.Nio.ByteBuffer.AllocateDirect(frame.Bytes.Length)!;
            Marshal.Copy(frame.Bytes, 0, Android.Runtime.JNIEnv.GetDirectBufferAddress(direct.Handle), frame.Bytes.Length);
            direct.Rewind();
            int index = bitmapIndex ^= 1;
            var bitmap = bitmaps[index] ??= Bitmap.CreateBitmap(ModEntry.W, ModEntry.H, Bitmap.Config.Argb8888!)!;
            bitmap.CopyPixelsFromBuffer(direct);
            var drawable = drawables[index];
            if (drawable == null)
            {
                drawables[index] = drawable = new BitmapDrawable(window!.Context.Resources, bitmap);
                drawable.SetFilterBitmap(false);
            }
            view.SetImageDrawable(drawable);
            presented = frame.Scope;
            presentedEpoch = frame.PresentationEpoch;
            hasImage = true;
        }
        catch (Exception ex) { monitor.LogOnce($"Companion frame upload failed: {ex}", LogLevel.Error); CloseWindow(); }
        finally { if (frames.Complete(frame)) ui.Post(push); }
    }

    // PreDraw registers the callback before Android submits the current frame.
    void OnPreDraw(object? sender, ViewTreeObserver.PreDrawEventArgs e)
    {
        e.Handled = true;
        if (disposed || paused || !Ready || !hasImage || view == null || commitTree == null
            || commits.Current != null || toolbar.IsAvailable(presentedEpoch) || !OperatingSystem.IsAndroidVersionAtLeast(29) || !view.IsHardwareAccelerated) return;
        var targetView = view;
        var targetDrawable = view.Drawable;
        var scope = presented;
        long epoch = presentedEpoch;
        Java.Lang.Runnable? callback = null;
        callback = new Java.Lang.Runnable(() =>
        {
            try
            {
                if (!disposed && !paused && Ready && hasImage && ReferenceEquals(view, targetView)
                    && ReferenceEquals(view.Drawable, targetDrawable) && state.IsCurrent(scope))
                    toolbar.Present(epoch);
                else if (!disposed && Ready && view != null) view.Invalidate();
            }
            finally { commits.Complete(callback!); }
        });
        try
        {
            var liveTree = view.ViewTreeObserver;
            if (liveTree == null || !liveTree.IsAlive) { callback.Dispose(); return; }
            commitTree = liveTree;
            commits.Set(callback);
            liveTree.RegisterFrameCommitCallback(callback);
        }
        catch (Exception ex)
        {
            commits.Cancel(_ => true); Invalidate();
            monitor.LogOnce($"Companion frame commit unavailable: {ex}", LogLevel.Warn);
        }
    }

    void Invalidate() { toolbar.Unavailable(); hasImage = false; pointer = -1; state.Invalidate(); frames.Invalidate(); }
    void CancelCommit() => commits.Cancel(callback => commitTree?.IsAlive == true
        && OperatingSystem.IsAndroidVersionAtLeast(29) && commitTree.UnregisterFrameCommitCallback(callback));
    void CloseWindow()
    {
        Ready = false;
        hidden = false;
        Invalidate();
        if (commitTree?.IsAlive == true)
        {
            commitTree.PreDraw -= OnPreDraw;
        }
        CancelCommit(); commitTree = null;
        if (view != null)
        {
            view.Touch -= OnTouch; view.LayoutChange -= OnLayout;
            view.SetImageDrawable(null);
        }
        window?.Dismiss();
        view?.Dispose(); view = null;
        window?.Dispose(); window = null;
    }
    void OnPaused(object? sender, EventArgs e) => ui.Post(() =>
    {
        if (disposed) return;
        paused = true;
        ui.RemoveCallbacks(pulse);
        if (activity.IsFinishing || activity.IsDestroyed) { Dispose(); return; }
        // A pause does not remove the physical display. Reuse this Presentation's
        // window context; repeated construction retained Binder resources on API33.
        Ready = false;
        Invalidate();
        CancelCommit();
        if (window != null) { window.Hide(); hidden = true; }
    });
    void OnResumed(object? sender, EventArgs e) => ui.Post(() =>
    {
        if (disposed) return;
        paused = false;
        ui.RemoveCallbacks(pulse);
        Pulse();
    });
    public void RenderFailed() => ui.Post(() => { if (!disposed) { Invalidate(); } });
    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        toolbar.Unavailable();
        Ready = false;
        state.Invalidate(); frames.Dispose();
        Microsoft.Xna.Framework.AndroidGameActivity.Paused -= OnPaused;
        Microsoft.Xna.Framework.AndroidGameActivity.Resumed -= OnResumed;
        ui.Post(() =>
        {
            ui.RemoveCallbacks(pulse); ui.RemoveCallbacks(push);
            CloseWindow();
            foreach (var drawable in drawables) drawable?.Dispose();
            // Do not Recycle: Android may still retain its own drawable/render references.
            foreach (var bitmap in bitmaps) bitmap?.Dispose();
            direct?.Dispose();
            pulse.Dispose(); push.Dispose(); ui.Dispose();
        });
    }

    public static Activity ResolveGameActivity()
    {
        var type = Type.GetType("JunimoGate.GameHost.GameHostBridge, JunimoGate.GameHost", throwOnError: true)!;
        var method = type.GetMethod("GetActivity", BindingFlags.Public | BindingFlags.Static,
            binder: null, types: Type.EmptyTypes, modifiers: null)
            ?? throw new MissingMethodException(type.FullName, "GetActivity");
        return method.Invoke(null, null) as Activity
            ?? throw new InvalidOperationException("JunimoGate did not return its active game Activity.");
    }

    public static Display? SelectCompanionDisplay(Activity activity)
    {
        var manager = (DisplayManager?)activity.GetSystemService(Android.Content.Context.DisplayService)
            ?? throw new InvalidOperationException("Android DisplayManager is unavailable.");
#pragma warning disable CS0618
        int primary = activity.WindowManager!.DefaultDisplay!.DisplayId;
#pragma warning restore CS0618
        var candidates = (manager.GetDisplays(DisplayManager.DisplayCategoryPresentation) ?? [])
            .Where(display => display.IsValid && display.DisplayId != primary).ToArray();
        if (candidates.Length > 1)
            throw new InvalidOperationException("More than one companion display is available; selection is ambiguous.");
        return candidates.SingleOrDefault();
    }
}
#endif
