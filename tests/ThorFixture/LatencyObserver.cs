using System.Text.Json;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Views.Inspectors;
using Android.Widget;

namespace ThorFixture;

// Separate device instrumentation. It observes native View input/drawing and never calls the mod.
public sealed partial class FixtureEntry
{
    readonly Handler latencyUi = new(Looper.MainLooper);
    object latencyResult = new { status = "idle" };
    ImageView latencyView;
    ViewTreeObserver latencyTree;
    Java.Lang.Runnable latencyCommit;
    int[] latencyExpected;
    long latencyInput;

    object ReadLatency() => Volatile.Read(ref latencyResult);
    void LatencyResult(object value) => Volatile.Write(ref latencyResult, value);

    static IEnumerable<ImageView> Images(View root)
    {
        if (root is ImageView image) yield return image;
        if (root is ViewGroup group)
            for (int i = 0; i < group.ChildCount; i++)
                foreach (var child in Images(group.GetChildAt(i))) yield return child;
    }
    static ImageView CompanionImage() => WindowInspector.GlobalWindowViews
        .Where(root => root.Display?.DisplayId == 4)
        .SelectMany(Images).Single(view => view.Drawable is BitmapDrawable);

    static int[] Signature(ImageView view)
    {
        var bitmap = ((BitmapDrawable)view.Drawable).Bitmap;
        var pixels = new List<int>();
        for (int y = 20; y < 180; y += 20)
            for (int x = 20; x < bitmap.Width - 20; x += 20)
                pixels.Add(bitmap.GetPixel(x, y));
        return pixels.ToArray();
    }
    object QueueLatencySnapshot()
    {
        LatencyResult(new { status = "pending" });
        latencyUi.Post(() =>
        {
            try
            {
                var view = CompanionImage();
                LatencyResult(new { status = "snapshot", signature = Signature(view), hardwareAccelerated = view.IsHardwareAccelerated,
                    refreshRate = view.Display.RefreshRate });
            }
            catch (Exception ex) { LatencyResult(new { status = "error", error = ex.Message }); }
        });
        return new { queued = true };
    }
    object StopLatency() { latencyUi.Post(DetachLatency); return new { queued = true }; }
    object ArmLatency(JsonElement command)
    {
        var expected = command.GetProperty("signature").EnumerateArray().Select(x => x.GetInt32()).ToArray();
        LatencyResult(new { status = "pending" });
        latencyUi.Post(() =>
        {
            try
            {
                DetachLatency();
                latencyView = CompanionImage();
                if (!latencyView.IsHardwareAccelerated) throw new InvalidOperationException("Frame commit requires hardware acceleration");
                if (Signature(latencyView).SequenceEqual(expected)) throw new InvalidOperationException("Target image is already displayed");
                latencyExpected = expected;
                latencyTree = latencyView.ViewTreeObserver;
                latencyView.Touch += ObserveLatencyTouch;
                latencyTree.PreDraw += ObserveLatencyPreDraw;
                LatencyResult(new { status = "armed" });
            }
            catch (Exception ex) { DetachLatency(); LatencyResult(new { status = "error", error = ex.Message }); }
        });
        return new { queued = true };
    }
    void ObserveLatencyTouch(object sender, View.TouchEventArgs e)
    {
        if (e.Event.ActionMasked == MotionEventActions.Up && latencyInput == 0)
        {
            latencyInput = e.Event.EventTime;
            LatencyResult(new { status = "input", inputEventMs = latencyInput });
        }
    }
    void ObserveLatencyPreDraw(object sender, ViewTreeObserver.PreDrawEventArgs e)
    {
        e.Handled = true;
        if (latencyInput == 0 || latencyCommit != null || !Signature(latencyView).SequenceEqual(latencyExpected)) return;
        long input = latencyInput;
        float refresh = latencyView.Display.RefreshRate;
        LatencyResult(new { status = "matching-draw", inputEventMs = input });
        latencyCommit = new Java.Lang.Runnable(() =>
        {
            long committed = SystemClock.UptimeMillis();
            LatencyResult(new { status = "committed", inputEventMs = input, commitMs = committed,
                eventToCommitMs = committed - input, refreshRate = refresh,
                withOneRefreshMs = committed - input + 1000d / refresh,
                method = "Native lower touch Up to first matching bitmap frame commit, registered in PreDraw, plus one refresh estimate; not optical latency" });
        });
        latencyTree.RegisterFrameCommitCallback(latencyCommit);
        // Android13 captures commit callbacks at performDraw entry, after PreDraw.
        // Register here to measure the first matching frame without a synthetic redraw.
    }
    void DetachLatency()
    {
        if (latencyView != null) latencyView.Touch -= ObserveLatencyTouch;
        if (latencyTree?.IsAlive == true)
        {
            latencyTree.PreDraw -= ObserveLatencyPreDraw;
            if (latencyCommit != null) latencyTree.UnregisterFrameCommitCallback(latencyCommit);
        }
        latencyCommit?.Dispose(); latencyCommit = null;
        latencyView = null; latencyTree = null; latencyExpected = null; latencyInput = 0;
    }
}
