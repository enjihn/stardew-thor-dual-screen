using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Android.Graphics;
using Path = System.IO.Path;
using StardewModdingAPI;
using StardewValley;

namespace ThorFixture;

// Guarded by IsTestFarm in the request dispatcher. Never packaged with the release.
public sealed partial class FixtureEntry
{
    const BindingFlags ProbeFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    static FieldInfo ProbeField(object target, string name) => target.GetType().GetField(name, ProbeFlags)
        ?? throw new MissingFieldException(target.GetType().FullName, name);
    Mod ToolbarMod()
    {
        var info = Helper.ModRegistry.Get("Hung.ThorDualScreen") ?? throw new InvalidOperationException("Companion is not loaded.");
        return (Mod)(info.GetType().GetProperty("Mod", ProbeFlags)?.GetValue(info)
            ?? throw new InvalidOperationException("Mod instance unavailable."));
    }
    object ToolbarState()
    {
        var mod = ToolbarMod();
        var policy = ProbeField(mod, "toolbarPolicy").GetValue(mod);
        var type = typeof(Game1).Assembly.GetType("StardewValley.Menus.Toolbar", true);
        var bar = type.GetField("_instance", ProbeFlags).GetValue(null);
        return new {
            visible = type.GetProperty("visible", ProbeFlags).GetValue(null),
            pressed = type.GetField("toolbarPressed", ProbeFlags).GetValue(null),
            nextTool = type.GetField("_nextToolIndex", ProbeFlags).GetValue(bar),
            formerStripHit = type.GetMethod("isWithinBounds", ProbeFlags).Invoke(bar, new object[] { 50, 500 }),
            enabled = ProbeField(mod, "hideToolbar").GetValue(mod),
            available = ProbeField(policy, "available").GetValue(policy),
            hidden = ProbeField(policy, "hidden").GetValue(policy),
            epoch = ProbeField(policy, "epoch").GetValue(policy),
            viewport = new { Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height },
            game = Snapshot()
        };
    }
    object ItemSwitchState()
    {
        var mod = ToolbarMod();
        var overlay = ProbeField(mod, "itemSwitchOverlay").GetValue(mod);
        var policy = ProbeField(mod, "toolbarPolicy").GetValue(mod);
        object notice = overlay == null ? null : ProbeField(overlay, "notice").GetValue(overlay);
        double now = System.Diagnostics.Stopwatch.GetTimestamp() * 1000.0 / System.Diagnostics.Stopwatch.Frequency;
        return new {
            overlayEnabled = ProbeField(mod, "showItemSwitchOverlay").GetValue(mod),
            initialized = overlay != null,
            failed = overlay == null ? null : ProbeField(overlay, "failed").GetValue(overlay),
            noticeSlot = notice == null ? null : ProbeField(notice, "selected").GetValue(notice),
            remainingMs = notice == null ? 0 : Math.Max(0, (double)ProbeField(notice, "until").GetValue(notice) - now),
            presentation = policy.GetType().GetProperty("Presentation", ProbeFlags).GetValue(policy),
            toolbar = ToolbarState()
        };
    }
    object SwitchItem(JsonElement command)
    {
        RequireNoMenu();
        if (command.TryGetProperty("automaticIndex", out var automatic))
        {
            int index = automatic.GetInt32();
            if (index < 0 || index >= Game1.player.Items.Count) throw new InvalidOperationException("Invalid fixture slot");
            Game1.player.CurrentToolIndex = index;
        }
        else
        {
            var type = typeof(Game1).Assembly.GetType("StardewValley.Menus.Toolbar", true);
            var bar = type.GetField("_instance", ProbeFlags).GetValue(null);
            type.GetMethod("ScrollToolbarOne", ProbeFlags).Invoke(bar, new object[] { command.GetProperty("forward").GetBoolean() });
        }
        return ItemSwitchState();
    }
    object ToolbarConfig(JsonElement command)
    {
        var mod = ToolbarMod();
        bool overlaySetting = command.TryGetProperty("overlayEnabled", out var overlayValue);
        bool enabled = overlaySetting ? overlayValue.GetBoolean() : command.GetProperty("enabled").GetBoolean();
        string path = Path.Combine(mod.Helper.DirectoryPath, "config.json");
        var config = File.Exists(path) ? JsonNode.Parse(File.ReadAllText(path)) : new JsonObject();
        config[overlaySetting ? "ShowItemSwitchOverlay" : "HideToolbarWithCompanion"] = enabled;
        File.WriteAllText(path, config.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        ProbeField(mod, overlaySetting ? "showItemSwitchOverlay" : "hideToolbar").SetValue(mod, enabled);
        return new { enabled, scope = "disposable test group configuration" };
    }
    object ToolbarUploadFault()
    {
        var mod = ToolbarMod();
        var host = ProbeField(mod, "host").GetValue(mod);
        // Replace only the upload destinations with fresh, already recycled test bitmaps.
        // Existing displayed bitmaps are neither recycled nor disposed.
        latencyUi.Post(() => {
            var slots = (Bitmap[])ProbeField(host, "bitmaps").GetValue(host);
            var originals = slots.ToArray();
            var broken = new Bitmap[slots.Length];
            for (int i = 0; i < slots.Length; i++)
            {
                broken[i] = Bitmap.CreateBitmap(620, 540, Bitmap.Config.Argb8888);
                broken[i].Recycle(); slots[i] = broken[i];
            }
            latencyUi.PostDelayed(() => {
                for (int i = 0; i < slots.Length; i++) { slots[i] = originals[i]; broken[i].Dispose(); }
                Monitor.Log("Toolbar upload fault removed; original upload bitmaps restored.", LogLevel.Info);
            }, 12000);
        });
        return new { armed = true, durationMs = 12000, trigger = "touch a lower tab to request a frame" };
    }
}
