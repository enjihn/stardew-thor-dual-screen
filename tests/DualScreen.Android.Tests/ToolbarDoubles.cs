// Desktop contract doubles for the Android-only reflection and installation boundary.
// These do not simulate Harmony interception; that is verified on the Thor.
using System.Reflection;
namespace StardewModdingAPI
{
    public enum LogLevel { Warn }
    public interface IMonitor { void Log(string message, LogLevel level); }
    public sealed class TestMonitor : IMonitor
    {
        public string? Warning;
        public void Log(string message, LogLevel level) => Warning = message;
    }
}
namespace StardewValley
{
    public class Item { }
    public class Game1 { }
}
namespace StardewValley.Menus
{
    public class Toolbar
    {
        public static Toolbar _instance = new();
        public static bool toolbarPressed;
        public int _startTapPositionX, _startTapPositionY, _startIndex, _drawStartIndex, _shoulderButtonDownCount, _nextToolIndex;
        public bool _ignoreRelease, _showTooltip;
        public StardewValley.Item? hoverItem, lastHoverItem;
        public string hoverTitle = "";
        public static bool visible => true;
        public void receiveLeftClick(int x, int y, bool playSound) { }
        public void releaseLeftClick(int x, int y) { }
        public void performHoverAction(int x, int y) { }
        public bool isWithinBounds(int x, int y) => true;
    }
}
namespace HarmonyLib
{
    public sealed class HarmonyMethod(Type type, string name) { }
    public sealed class Harmony(string owner)
    {
        public static int Attempts, FailAt;
        public static readonly List<MethodInfo> Installed = new();
        public static string? RemovedOwner;
        public void Patch(MethodInfo method, HarmonyMethod prefix)
        {
            if (++Attempts == FailAt) throw new InvalidOperationException("Controlled patch installation failure");
            Installed.Add(method);
        }
        public void UnpatchAll(string id) { RemovedOwner = id; Installed.Clear(); }
    }
}
