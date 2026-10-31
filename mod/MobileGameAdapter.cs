#if THOR_ANDROID
using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;

namespace DualScreen;

/// <summary>Narrow bindings verified against Play 1.6.15.3. Fail before enabling input if they drift.</summary>
internal static class MobileGameAdapter
{
    const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    static readonly List<string> bindingErrors = new();
    static readonly FieldInfo bundlePage = Field<JunimoNoteMenu, bool>("specificBundlePage");
    static readonly FieldInfo bundle = Field<JunimoNoteMenu, StardewValley.Menus.Bundle>("currentPageBundle");
    static readonly FieldInfo held = Field<JunimoNoteMenu, Item>("heldItem");
    static readonly FieldInfo source = Field<ItemGrabMenu, Item>("sourceItem");
    static readonly FieldInfo loadIndex = Field<LoadGameMenu, int>("currentItemIndex");
    static readonly FieldInfo clip = Field<LoadGameMenu, Rectangle>("clipBox");
    static readonly FieldInfo itemHeight = Field<LoadGameMenu, int>("itemHeight");
    static readonly FieldInfo confirm = Field<LoadGameMenu, ConfirmationDialog>("confirmBox");
    static readonly FieldInfo backup = Field<LoadGameMenu, ConfirmationDialog>("backupBox");
    static readonly MethodInfo recalculate = Method<LoadGameMenu>("recalculateSlots", typeof(void));
    static readonly MethodInfo purchase = Method<ShopMenu>("tryToPurchaseItem", typeof(bool),
        typeof(ISalable), typeof(ISalable), typeof(int), typeof(int), typeof(int));
    static readonly FieldInfo selling = Field<ShopMenu, Item>("itemPlayerIsSelling");
    static readonly FieldInfo storageShop = Field<ShopMenu, bool>("_isStorageShop");
    // The released host rewrites direct access to this native field into its desktop facade property.
    // That facade has a different instance layout. Bind the actual Android field instead.
    static readonly FieldInfo inventoryHeld = Field<MenuWithInventory, Item>("heldItem");

    static FieldInfo Field<T, TValue>(string name)
    {
        var field = typeof(T).GetField(name, Instance);
        if (field?.FieldType != typeof(TValue))
            bindingErrors.Add($"Expected {typeof(T).FullName}.{name}: {typeof(TValue).FullName}");
        return field;
    }
    static MethodInfo Method<T>(string name, Type result, params Type[] arguments)
    {
        var method = typeof(T).GetMethod(name, Instance, arguments);
        if (method?.ReturnType != result) bindingErrors.Add($"Unsupported {typeof(T).FullName}.{name} signature");
        return method;
    }
    static bool inspected;
    public static IReadOnlyList<string> ValidateRuntime()
    {
        if (!inspected)
        {
            inspected = true;
            Field<ShippingMenu, bool>("outro"); Field<ShippingMenu, bool>("newDayPlaque");
            foreach (string name in new[] { "introTimer", "outroFadeTimer", "centerY", "dayPlaqueY", "finalOutroTimer" })
                Field<ShippingMenu, int>(name);
            Field<ShippingMenu, List<MoneyDial>>("categoryDials");
            Field<LevelUpMenu, Rectangle>("sourceRectForLevelIcon"); Field<LevelUpMenu, string>("title");
            Field<LevelUpMenu, List<int>>("professionsToChoose");
            Field<TitleMenu, List<float>>("bigClouds"); Field<TitleMenu, List<float>>("smallClouds");
            Field<ShopMenu, float>("sellPercentage"); Field<Game1, BlendState>("lightingBlend");
        }
        var errors = new List<string>(bindingErrors);
        Region(errors, "Cursors landscape and stars", Game1.mouseCursors, new Rectangle(0, 737, 640, 911));
        Region(errors, "Cursors controls", Game1.mouseCursors, new Rectangle(0, 0, 654, 506));
        Region(errors, "1.6 weather", Game1.mouseCursors_1_6, new Rectangle(243, 293, 12, 8));
        Region(errors, "mobile day gradient", Game1.mobileSpriteSheet, new Rectangle(236, 0, 4, 264));
        Region(errors, "mobile night gradient", Game1.mobileSpriteSheet, new Rectangle(228, 0, 4, 184));
        Region(errors, "inventory slots", Game1.menuTexture, new Rectangle(128, 128, 64, 64));
        if (Game1.smallFont == null) errors.Add("Game small font is unavailable");
        try
        {
            Region(errors, "title art", Game1.content.Load<Texture2D>("Minigames\\TitleButtons"), new Rectangle(0, 0, 398, 245));
            Region(errors, "cloud art", Game1.content.Load<Texture2D>("Minigames\\Clouds"), new Rectangle(0, 0, 512, 696));
        }
        catch (Exception ex) { errors.Add($"Native title assets: {ex.Message}"); }
        return errors;
    }
    static void Region(List<string> errors, string name, Texture2D texture, Rectangle source)
    {
        if (texture == null || texture.IsDisposed || !texture.Bounds.Contains(source)) errors.Add($"Unavailable sprite region: {name} {source}");
    }
    public static bool Purchase(ShopMenu menu, ISalable item, ISalable heldItem, int quantity) =>
        (bool)purchase.Invoke(menu, new object[] { item, heldItem, quantity, 0, 0 });
    public static bool Sell(ShopMenu menu, int index, Item item, int quantity)
    {
        if (!ReferenceEquals(Game1.activeClickableMenu, menu) || menu.readOnly || Game1.options.SnappyMenus
            || quantity <= 0 || quantity > item.Stack || index < 0 || index >= Game1.player.Items.Count
            || !ReferenceEquals(Game1.player.Items[index], item)) return false;
        var slot = menu.inventory.inventory.FirstOrDefault(c => c.name == index.ToString());
        if (slot == null) return false;
        if (!menu.inventoryVisible) Click(menu, menu.inventoryButton.bounds.Center);
        if (!menu.inventoryVisible || !ReferenceEquals(Game1.activeClickableMenu, menu)) return false;
        // Native storage shops transfer the whole item on slot release, including their onSell callback.
        if ((bool)storageShop.GetValue(menu) && quantity != item.Stack) return false;
        Click(menu, slot.bounds.Center);
        if ((bool)storageShop.GetValue(menu)) return true;
        if (!ReferenceEquals(selling.GetValue(menu), item) || !ReferenceEquals(Game1.player.Items[index], item)) return false;
        menu.quantityToSell = quantity;
        Click(menu, menu.sellButton.bounds.Center);
        return selling.GetValue(menu) == null;
    }
    static void Click(IClickableMenu menu, Point point)
    {
        menu.receiveLeftClick(point.X, point.Y);
        if (ReferenceEquals(Game1.activeClickableMenu, menu)) menu.releaseLeftClick(point.X, point.Y);
    }
    public static int MoveStorage(ItemGrabMenu menu, bool fromChest, int index, Item item, int quantity)
    {
        if (!ReferenceEquals(Game1.activeClickableMenu, menu) || quantity <= 0 || quantity > item.Stack) return 0;
        var from = fromChest ? menu.ItemsToGrabMenu : menu.inventory;
        var to = fromChest ? menu.inventory : menu.ItemsToGrabMenu;
        if (index < 0 || index >= from.actualInventory.Count || !ReferenceEquals(from.actualInventory[index], item)
            || !(from.highlightMethod?.Invoke(item) ?? true) || !(to.highlightMethod?.Invoke(item) ?? true)) return 0;
        var sourceSlot = from.inventory.FirstOrDefault(c => c.name == index.ToString());
        // Let the mobile menu own movement, metadata, callbacks, capacity and any remainder.
        var targetSlot = to.inventory.FirstOrDefault(c => int.TryParse(c.name, out int slot)
            && slot < to.actualInventory.Count && to.actualInventory[slot] == null)
            ?? to.inventory.FirstOrDefault(c => int.TryParse(c.name, out int slot) && slot < to.actualInventory.Count
                && to.actualInventory[slot] is Item other && other.canStackWith(item) && other.Stack < other.maximumStackSize());
        if (sourceSlot == null || targetSlot == null) return 0;
        var target = to.actualInventory[int.Parse(targetSlot.name)];
        int room = target == null ? item.maximumStackSize() : target.maximumStackSize() - target.Stack;
        quantity = Math.Min(quantity, Math.Max(0, room));
        if (quantity == 0) return 0;
        var heldItem = InventoryHeldItem(menu);
        if (heldItem != null && !ReferenceEquals(heldItem, item)) return 0;
        from.ClearSelection(); to.ClearSelection(); SetInventoryHeldItem(menu, null);
        Click(menu, sourceSlot.bounds.Center);
        if (!ReferenceEquals(Game1.activeClickableMenu, menu) || !ReferenceEquals(InventoryHeldItem(menu), item)) return 0;
        int before = item.Stack;
        // Always use the native split path with an amount that fits this slot. Its whole-stack
        // remainder callback can re-add the same bag reference to itself and erase the remainder.
        from.currentlyHeldStack = quantity;
        Click(menu, targetSlot.bounds.Center);
        int remaining = index < from.actualInventory.Count && ReferenceEquals(from.actualInventory[index], item) ? item.Stack : 0;
        return Math.Max(0, before - remaining);
    }
    public static bool SpecificBundlePage(JunimoNoteMenu menu) => (bool)bundlePage.GetValue(menu);
    public static StardewValley.Menus.Bundle CurrentBundle(JunimoNoteMenu menu) => (StardewValley.Menus.Bundle)bundle.GetValue(menu);
    public static Item HeldItem(JunimoNoteMenu menu) => (Item)held.GetValue(menu);
    public static void SetHeldItem(JunimoNoteMenu menu, Item item) => held.SetValue(menu, item);
    public static Item SourceItem(ItemGrabMenu menu) => menu == null ? null : (Item)source.GetValue(menu);
    public static Item InventoryHeldItem(MenuWithInventory menu) => (Item)inventoryHeld.GetValue(menu);
    public static void SetInventoryHeldItem(MenuWithInventory menu, Item item) => inventoryHeld.SetValue(menu, item);

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
