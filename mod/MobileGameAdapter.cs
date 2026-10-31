#if THOR_ANDROID
using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;

namespace DualScreen;

/// <summary>Narrow bindings verified against Play 1.6.15.3. Fail before enabling input if they drift.</summary>
internal static class MobileGameAdapter
{
    const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    static readonly FieldInfo bundlePage = Field<JunimoNoteMenu, bool>("specificBundlePage");
    static readonly FieldInfo bundle = Field<JunimoNoteMenu, StardewValley.Menus.Bundle>("currentPageBundle");
    static readonly FieldInfo held = Field<JunimoNoteMenu, Item>("heldItem");
    static readonly FieldInfo source = Field<ItemGrabMenu, Item>("sourceItem");
    static readonly FieldInfo loadIndex = Field<LoadGameMenu, int>("currentItemIndex");
    static readonly FieldInfo clip = Field<LoadGameMenu, Rectangle>("clipBox");
    static readonly FieldInfo itemHeight = Field<LoadGameMenu, int>("itemHeight");
    static readonly FieldInfo confirm = Field<LoadGameMenu, ConfirmationDialog>("confirmBox");
    static readonly FieldInfo backup = Field<LoadGameMenu, ConfirmationDialog>("backupBox");
    static readonly MethodInfo recalculate = typeof(LoadGameMenu).GetMethod("recalculateSlots", Instance, Type.EmptyTypes)
        ?? throw new MissingMethodException(typeof(LoadGameMenu).FullName, "recalculateSlots()");

    static FieldInfo Field<T, TValue>(string name)
    {
        var field = typeof(T).GetField(name, Instance);
        if (field?.FieldType != typeof(TValue))
            throw new MissingFieldException($"Expected {typeof(T).FullName}.{name}: {typeof(TValue).FullName}");
        return field;
    }
    public static void ValidateRuntime()
    {
        // Touch a cached binding to force type initialization before the window accepts input.
        _ = bundlePage.Name;
    }
    public static bool SpecificBundlePage(JunimoNoteMenu menu) => (bool)bundlePage.GetValue(menu);
    public static StardewValley.Menus.Bundle CurrentBundle(JunimoNoteMenu menu) => (StardewValley.Menus.Bundle)bundle.GetValue(menu);
    public static Item HeldItem(JunimoNoteMenu menu) => (Item)held.GetValue(menu);
    public static void SetHeldItem(JunimoNoteMenu menu, Item item) => held.SetValue(menu, item);
    public static Item SourceItem(ItemGrabMenu menu) => menu == null ? null : (Item)source.GetValue(menu);

    public static bool SelectSave(LoadGameMenu menu, int index)
    {
        if (!ReferenceEquals(TitleMenu.subMenu, menu) || menu.IsDoingTask() || menu.deleteConfirmationScreen
            || confirm.GetValue(menu) != null || backup.GetValue(menu) != null
            || (int)loadIndex.GetValue(menu) != 0 || index < 0 || index >= menu.MenuSlots.Count)
            return false;
        // The mobile list has one button per save. Scroll the chosen row into its actual native clip.
        var visible = (Rectangle)clip.GetValue(menu);
        menu.scrollArea.setYOffsetForScroll(-index * (int)itemHeight.GetValue(menu));
        recalculate.Invoke(menu, null);
        if (index >= menu.slotButtons.Count) return false;
        var row = Rectangle.Intersect(menu.slotButtons[index].bounds, visible);
        if (row.Width < 96 || row.Height < 2) return false;
        var point = new Point(row.X + 48, row.Center.Y);
        if (menu.deleteButtons.Exists(button => button.containsPoint(point.X, point.Y))) return false;
        menu.receiveLeftClick(point.X, point.Y);
        menu.releaseLeftClick(point.X, point.Y);
        return true;
    }
}
#endif
