#if THOR_ANDROID
using Android.App;
using Android.Content;
using Android.Hardware.Display;
using Android.Views;
using System.Reflection;

namespace DualScreen;

internal static class MobileHost
{
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
        var manager = (DisplayManager?)activity.GetSystemService(Context.DisplayService)
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
