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
    object ToolbarConfig(JsonElement command)
    {
        var mod = ToolbarMod();
        bool enabled = command.GetProperty("enabled").GetBoolean();
        string path = Path.Combine(mod.Helper.DirectoryPath, "config.json");
        var config = File.Exists(path) ? JsonNode.Parse(File.ReadAllText(path)) : new JsonObject();
        config["HideToolbarWithCompanion"] = enabled;
        File.WriteAllText(path, config.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        ProbeField(mod, "hideToolbar").SetValue(mod, enabled);
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
