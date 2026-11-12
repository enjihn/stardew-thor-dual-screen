#if THOR_ANDROID
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace DualScreen;

/// <summary>Suppresses only the native Android toolbar while the companion is usable.</summary>
internal sealed class MobileToolbar : IDisposable
{
    const string Owner = "Hung.ThorDualScreen.ToolbarVisibility";
    static volatile bool hidden;
    readonly Harmony harmony = new(Owner);
    readonly IMonitor monitor;
    readonly FieldInfo instance, pressed, startX, startY, startIndex, drawIndex, ignoreRelease, hoverItem, lastHoverItem, hoverTitle, showTooltip, shoulderCount;
    bool failed;
    public bool IsHidden => !failed && hidden;

    MobileToolbar(Type type, IMonitor monitor)
    {
        this.monitor = monitor;
        FieldInfo Field(string name, Type fieldType, bool isStatic = false)
        {
            var f = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | (isStatic ? BindingFlags.Static : BindingFlags.Instance));
            if (f == null || f.FieldType != fieldType) throw new MissingFieldException(type.FullName, name);
            return f;
        }
        instance = Field("_instance", type, true); pressed = Field("toolbarPressed", typeof(bool), true);
        startX = Field("_startTapPositionX", typeof(int)); startY = Field("_startTapPositionY", typeof(int));
        startIndex = Field("_startIndex", typeof(int)); drawIndex = Field("_drawStartIndex", typeof(int));
        ignoreRelease = Field("_ignoreRelease", typeof(bool)); hoverItem = Field("hoverItem", typeof(Item));
        lastHoverItem = Field("lastHoverItem", typeof(Item)); hoverTitle = Field("hoverTitle", typeof(string));
        showTooltip = Field("_showTooltip", typeof(bool)); shoulderCount = Field("_shoulderButtonDownCount", typeof(int));
        MethodInfo Method(string name, Type result, bool isStatic, params Type[] args)
        {
            var m = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | (isStatic ? BindingFlags.Static : BindingFlags.Instance), null, args, null);
            if (m == null || m.DeclaringType != type || m.ReturnType != result) throw new MissingMethodException(type.FullName, name);
            return m;
        }
        var patches = new (MethodInfo method, string prefix)[] {
            (Method("get_visible", typeof(bool), true), nameof(VisiblePrefix)),
            (Method("receiveLeftClick", typeof(void), false, typeof(int), typeof(int), typeof(bool)), nameof(InputPrefix)),
            (Method("releaseLeftClick", typeof(void), false, typeof(int), typeof(int)), nameof(InputPrefix)),
            (Method("performHoverAction", typeof(void), false, typeof(int), typeof(int)), nameof(InputPrefix)),
            (Method("isWithinBounds", typeof(bool), false, typeof(int), typeof(int)), nameof(BoundsPrefix))
        };
        try { foreach (var (method, prefix) in patches) harmony.Patch(method, prefix: new HarmonyMethod(typeof(MobileToolbar), prefix)); }
        catch { hidden = false; harmony.UnpatchAll(Owner); throw; }
    }
    public static MobileToolbar TryInstall(IMonitor monitor)
    {
        try { return new MobileToolbar(typeof(Game1).Assembly.GetType("StardewValley.Menus.Toolbar", true), monitor); }
        catch (Exception ex) { hidden = false; monitor.Log($"Toolbar hiding unavailable: {ex}", LogLevel.Warn); return null; }
    }
    public void SetHidden(bool value)
    {
        if (failed || hidden == value) return;
        try
        {
            var bar = instance.GetValue(null);
            if (bar == null) { hidden = false; return; }
            pressed.SetValue(null, false); startX.SetValue(bar, -1); startY.SetValue(bar, -1);
            drawIndex.SetValue(bar, startIndex.GetValue(bar)); ignoreRelease.SetValue(bar, false);
            hoverItem.SetValue(bar, null); lastHoverItem.SetValue(bar, null); hoverTitle.SetValue(bar, "");
            showTooltip.SetValue(bar, true); shoulderCount.SetValue(bar, 0);
            hidden = value;
        }
        catch (Exception ex) { hidden = false; failed = true; monitor.Log($"Toolbar hiding disabled: {ex}", LogLevel.Warn); }
    }
    static bool VisiblePrefix(ref bool __result) { if (!hidden) return true; __result = false; return false; }
    static bool InputPrefix() => !hidden;
    static bool BoundsPrefix(ref bool __result) { if (!hidden) return true; __result = false; return false; }
    public void Dispose() { hidden = false; harmony.UnpatchAll(Owner); }
}
#endif
