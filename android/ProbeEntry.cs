using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Reflection;
using System.Runtime.InteropServices;
using XColor = Microsoft.Xna.Framework.Color;

namespace DualScreen;

// Temporary Task 3 probe. No gameplay mutations, custom Java subclasses or fixture state.
public sealed class ProbeEntry : Mod
{
    const int Width = 620, Height = 540;
    Activity? activity;
    Presentation? window;
    ImageView? view;
    RenderTarget2D? target;
    SpriteBatch? batch;
    Bitmap? bitmap;
    Java.Nio.ByteBuffer? buffer;
    readonly byte[] pixels = new byte[Width * Height * 4];
    int busy, touches, drawnTouches = -1;
    volatile bool ready;

    public override void Entry(IModHelper helper)
    {
        var id = GetType().Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .Single(a => a.Key == "ThorBuildId").Value;
        Monitor.Log($"THOR_PROBE build={id}; game={typeof(Game1).Assembly.GetName().Version}; host={typeof(Mod).Assembly.GetName().Version}", LogLevel.Info);
        helper.Events.GameLoop.GameLaunched += Open;
        helper.Events.GameLoop.UpdateTicked += Render;
        helper.Events.Input.ButtonPressed += (_, e) => Monitor.Log($"THOR_PROBE input={e.Button}", LogLevel.Info);
        Microsoft.Xna.Framework.AndroidGameActivity.Paused += Paused;
        Microsoft.Xna.Framework.AndroidGameActivity.Resumed += Resumed;
    }

    void Open(object? sender, GameLaunchedEventArgs e)
    {
        try
        {
            activity = MobileHost.ResolveGameActivity();
            Monitor.Log($"THOR_PROBE activity={activity.GetType().FullName}", LogLevel.Info);
            activity.RunOnUiThread(() =>
            {
                try
                {
                    var display = MobileHost.SelectCompanionDisplay(activity);
                    if (display is null) throw new InvalidOperationException("No companion display.");
                    window = new Presentation(activity, display);
                    window.Window!.AddFlags(WindowManagerFlags.NotFocusable);
                    view = new ImageView(window.Context);
                    view.SetScaleType(ImageView.ScaleType.FitXy);
                    view.Touch += Touch;
                    window.SetContentView(view);
                    window.Show();
                    ready = true;
                    Monitor.Log($"THOR_PROBE presentation display={display.DisplayId}; framework Touch adapter created", LogLevel.Info);
                }
                catch (Exception ex) { Monitor.Log($"THOR_PROBE window failed: {ex}", LogLevel.Error); }
            });
        }
        catch (Exception ex) { Monitor.Log($"THOR_PROBE activity failed: {ex}", LogLevel.Error); }
    }

    void Touch(object? sender, Android.Views.View.TouchEventArgs e)
    {
        e.Handled = true;
        if (e.Event is not { } motion) return;
        if (motion.ActionMasked == MotionEventActions.Up) Interlocked.Increment(ref touches);
        Monitor.Log($"THOR_PROBE touch={motion.ActionMasked} x={motion.GetX():0} y={motion.GetY():0} view={view?.Width}x{view?.Height}", LogLevel.Info);
    }

    void Render(object? sender, UpdateTickedEventArgs e)
    {
        if (!ready || view is null || Game1.smallFont is null || Game1.mouseCursors is null
            || drawnTouches == Volatile.Read(ref touches) || Interlocked.CompareExchange(ref busy, 1, 0) != 0) return;
        try
        {
            var gd = Game1.graphics.GraphicsDevice;
            target ??= new RenderTarget2D(gd, Width, Height);
            batch ??= new SpriteBatch(gd);
            var oldTargets = gd.GetRenderTargets();
            var oldViewport = gd.Viewport;
            int count = Volatile.Read(ref touches);
            try
            {
                gd.SetRenderTarget(target);
                gd.Clear(new XColor(35, 60, 45));
                batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
                batch.Draw(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(240, 150, 128, 128),
                    new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16), XColor.White);
                batch.DrawString(Game1.smallFont, "Thor Android Release probe", new Vector2(55, 50), XColor.White);
                batch.DrawString(Game1.smallFont, "Real game texture + font", new Vector2(55, 320), XColor.White);
                batch.DrawString(Game1.smallFont, $"Lower screen touches: {count}", new Vector2(55, 380), XColor.White);
                batch.End();
            }
            finally { gd.SetRenderTargets(oldTargets); gd.Viewport = oldViewport; }
            target.GetData(pixels);
            drawnTouches = count;
            activity!.RunOnUiThread(() =>
            {
                try
                {
                    buffer ??= Java.Nio.ByteBuffer.AllocateDirect(pixels.Length)!;
                    Marshal.Copy(pixels, 0, Android.Runtime.JNIEnv.GetDirectBufferAddress(buffer.Handle), pixels.Length);
                    buffer.Rewind();
                    bitmap ??= Bitmap.CreateBitmap(Width, Height, Bitmap.Config.Argb8888!)!;
                    bitmap.CopyPixelsFromBuffer(buffer);
                    view.SetImageBitmap(bitmap);
                    Monitor.Log($"THOR_PROBE frame touches={count}; drawable={view.Width}x{view.Height}; focus={activity.HasWindowFocus}", LogLevel.Info);
                }
                catch (Exception ex) { Monitor.Log($"THOR_PROBE upload failed: {ex}", LogLevel.Error); }
                finally { Volatile.Write(ref busy, 0); }
            });
        }
        catch (Exception ex)
        {
            ready = false;
            Volatile.Write(ref busy, 0);
            Monitor.Log($"THOR_PROBE render failed: {ex}", LogLevel.Error);
        }
    }

    void Paused(object? sender, EventArgs e) => activity?.RunOnUiThread(() =>
    { window?.Hide(); Monitor.Log("THOR_PROBE paused", LogLevel.Info); });
    void Resumed(object? sender, EventArgs e) => activity?.RunOnUiThread(() =>
    { window?.Show(); Monitor.Log("THOR_PROBE resumed", LogLevel.Info); });
}
