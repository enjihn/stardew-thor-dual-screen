#if THOR_ANDROID
using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace DualScreen;

/// <summary>Passive HUD feedback for native manual selection, independent of toolbar suppression.</summary>
internal sealed class MobileItemSwitchOverlay : IDisposable
{
    const string Owner = "Hung.ThorDualScreen.ItemSwitchOverlay";
    static MobileItemSwitchOverlay active;
    readonly Harmony harmony = new(Owner);
    readonly ItemSwitchNotice notice = new();
    readonly ToolbarVisibilityPolicy presentation;
    readonly Func<bool> eligible;
    readonly IMonitor monitor;
    bool failed;
    static double Now => Stopwatch.GetTimestamp() * 1000.0 / Stopwatch.Frequency;

    MobileItemSwitchOverlay(ToolbarVisibilityPolicy presentation, Func<bool> eligible, IMonitor monitor)
    { this.presentation = presentation; this.eligible = eligible; this.monitor = monitor; }

    public static MobileItemSwitchOverlay TryInstall(ToolbarVisibilityPolicy presentation, Func<bool> eligible, IMonitor monitor)
    {
        var overlay = new MobileItemSwitchOverlay(presentation, eligible, monitor);
        try
        {
            var toolbarType = typeof(Game1).Assembly.GetType("StardewValley.Menus.Toolbar", true);
            var scroll = toolbarType.GetMethod("ScrollToolbarOne", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(bool) }, null);
            var cycle = typeof(Game1).GetMethod("pressSwitchToolButton", BindingFlags.Public | BindingFlags.Static, null, Type.EmptyTypes, null);
            if (scroll?.ReturnType != typeof(void) || cycle?.ReturnType != typeof(void))
                throw new MissingMethodException("Native manual-selection bindings unavailable.");
            foreach (var method in new[] { scroll, cycle })
                overlay.harmony.Patch(method, prefix: new HarmonyMethod(typeof(MobileItemSwitchOverlay), nameof(BeforeSwitch)),
                    postfix: new HarmonyMethod(typeof(MobileItemSwitchOverlay), nameof(AfterSwitch)));
            active = overlay;
            return overlay;
        }
        catch (Exception ex)
        {
            overlay.Dispose();
            monitor.Log($"Item-switch overlay unavailable: {ex}", LogLevel.Warn);
            return null;
        }
    }
    static void BeforeSwitch(out int __state) => __state = Game1.player?.CurrentToolIndex ?? -1;
    static void AfterSwitch(int __state) => active?.ManualSwitch(__state);
    public void ManualSwitch(int before)
    {
        if (failed) return;
        try
        {
            var p = presentation.Presentation;
            notice.ManualSwitch(before, Game1.player?.CurrentToolIndex ?? -1, Game1.player?.CurrentItem,
                p.Epoch, Now, p.Available && p.BagShowing == false && eligible());
        }
        catch (Exception ex) { Disable(ex); }
    }
    float Opacity()
    {
        var p = presentation.Presentation;
        return notice.Opacity(Game1.player?.CurrentToolIndex ?? -1, Game1.player?.CurrentItem,
            p.Epoch, Now, !failed && p.Available && p.BagShowing == false && eligible());
    }
    public void Tick()
    {
        if (failed) return;
        try { Opacity(); }
        catch (Exception ex) { Disable(ex); }
    }
    public void Clear() => notice.Clear();
    public void Draw(SpriteBatch batch)
    {
        if (failed) return;
        try
        {
            float alpha = Opacity();
            if (alpha <= 0) return;
            // RenderedHud already uses UI coordinates and an open PointClamp SpriteBatch.
            const int slot = 48, gap = 4, width = 5 * slot + 4 * gap;
            int x = (Game1.uiViewport.Width - width) / 2;
            int y = Game1.uiViewport.Height - 16 - slot;
            if (x < Game1.xEdge || y < 0) return;
            int selected = Game1.player.CurrentToolIndex;
            int first = ItemSwitchNotice.FirstSlot(selected, Game1.player.MaxItems);
            for (int i = 0; i < 5; i++)
            {
                int index = first + i, left = x + i * (slot + gap);
                // Native Android toolbar backgrounds are separate from its transparent slot outlines.
                var bounds = new Rectangle(left, y, slot, slot);
                batch.Draw(Game1.menuTexture, bounds, new Rectangle(12, index % 2 == 0 ? 296 : 280, 4, 4), Color.White * alpha);
                if (index == selected)
                    batch.Draw(Game1.mouseCursors, bounds, new Rectangle(150, 470, 4, 4), Color.White * alpha);
                var source = new Rectangle(index == selected ? 20 : 0, 96, 20, 20);
                batch.Draw(Game1.mobileSpriteSheet, bounds, source, Color.White * alpha);
                if (index < Game1.player.Items.Count && Game1.player.Items[index] is Item item)
                {
                    const float scale = 40f / 64f;
                    item.drawInMenu(batch, new Vector2(left + 4 - 32 * (1 - scale), y + 4 - 32 * (1 - scale)),
                        scale, alpha, .9f, StackDrawType.Hide, Color.White, false);
                }
            }
        }
        catch (Exception ex) { Disable(ex); }
    }
    void Disable(Exception ex)
    {
        failed = true; notice.Clear();
        monitor.Log($"Item-switch overlay disabled: {ex}", LogLevel.Warn);
    }
    public void Dispose()
    {
        failed = true; notice.Clear();
        if (ReferenceEquals(active, this)) active = null;
        harmony.UnpatchAll(Owner);
    }
}
#endif
