using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Locations;
using Microsoft.Xna.Framework;

namespace ThorFixture;

/// <summary>Test-state setup/observation only. No dependency on, or invocation of, production mod handlers.</summary>
public sealed class FixtureEntry : Mod
{
    const ulong TestId = 453080632;
    const string TestName = "ThorTest", TestSlot = "ThorTest_453080632";
    string directory, lastRequest;
    readonly List<double> frames = new();
    long lastFrame;
    bool measuring;
    Chest testChest;
    string lastAnswer;
    static readonly System.Reflection.FieldInfo nativeHeld = typeof(MenuWithInventory).GetField("heldItem");

    public override void Entry(IModHelper helper)
    {
        directory = Path.Combine(Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath, "ThorFixture");
        Directory.CreateDirectory(directory);
        string ack = Path.Combine(directory, "last-request.txt");
        lastRequest = File.Exists(ack) ? File.ReadAllText(ack) : "";
        helper.Events.GameLoop.UpdateTicking += Update;
        helper.Events.Display.Rendered += Rendered;
        Monitor.Log("Disposable fixture ready. Only ThorTest test identity is permitted. Not a release component.", LogLevel.Info);
    }
    static bool IsTestFarm => Context.IsWorldReady && Game1.uniqueIDForThisGame == TestId
        && Game1.player.Name == TestName && Game1.player.farmName.Value == TestName;

    void Rendered(object sender, RenderedEventArgs e)
    {
        if (!measuring) return;
        long now = Stopwatch.GetTimestamp();
        if (lastFrame != 0 && frames.Count < 40000) frames.Add((now - lastFrame) * 1000d / Stopwatch.Frequency);
        lastFrame = now;
    }
    void Update(object sender, UpdateTickingEventArgs e)
    {
        if (!e.IsMultipleOf(6)) return;
        string request = Path.Combine(directory, "request.json");
        if (!File.Exists(request)) return;
        string id = null;
        try
        {
            using var json = JsonDocument.Parse(File.ReadAllText(request));
            var command = json.RootElement;
            id = command.GetProperty("id").GetString();
            if (!Regex.IsMatch(id ?? "", "^[A-Za-z0-9_-]{1,60}$") || id == lastRequest) return;
            lastRequest = id;
            File.WriteAllText(Path.Combine(directory, "last-request.txt"), id);
            if (!IsTestFarm) throw new InvalidOperationException("Refused: active farm is not the designated disposable test identity.");
            string operation = command.GetProperty("operation").GetString();
            object result = operation switch
            {
                "state" => Snapshot(),
                "export" => ExportSave(id),
                "seed" => Seed(command),
                "warp" => Warp(command),
                "menu" => Menu(command),
                "timing-start" => StartTiming(),
                "timing-stop" => StopTiming(),
                _ => throw new InvalidOperationException("Unknown fixture operation")
            };
            Write(id, new { ok = true, operation, result });
        }
        catch (Exception ex)
        {
            Monitor.Log($"Fixture request failed: {ex.Message}", LogLevel.Warn);
            if (id != null && Regex.IsMatch(id, "^[A-Za-z0-9_-]{1,60}$")) Write(id, new { ok = false, error = ex.ToString() });
        }
    }
    void Write(string id, object value) => File.WriteAllText(Path.Combine(directory, id + ".json"),
        JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true }));
    object Snapshot() => new
    {
        farmId = Game1.uniqueIDForThisGame, day = Game1.dayOfMonth, season = Game1.currentSeason,
        year = Game1.year, time = Game1.timeOfDay, money = Game1.player.Money,
        menu = Game1.activeClickableMenu?.GetType().FullName, location = Game1.currentLocation.NameOrUniqueName,
        tile = new[] { Game1.player.Tile.X, Game1.player.Tile.Y }, held = Game1.player.CurrentToolIndex,
        items = Game1.player.Items.Select((item, index) => new { index, item = Describe(item) }).ToArray(),
        recipes = Game1.player.craftingRecipes.Pairs.ToDictionary(pair => pair.Key, pair => pair.Value),
        professions = Game1.player.professions.ToArray(),
        pendingLevels = Game1.player.newLevels.Select(level => new[] { level.X, level.Y }).ToArray(),
        geodesCracked = Game1.stats.GeodesCracked, lastAnswer,
        bundles = (Game1.getLocationFromName("CommunityCenter") as CommunityCenter)?.bundles.Pairs.Select(pair => new { id = pair.Key, given = pair.Value.ToArray() }).ToArray(),
        bundleRewards = (Game1.getLocationFromName("CommunityCenter") as CommunityCenter)?.bundleRewards.Pairs.ToDictionary(pair => pair.Key, pair => pair.Value),
        bundlePage = Game1.activeClickableMenu is JunimoNoteMenu note ? new { note.scrambledText, specific = typeof(JunimoNoteMenu).GetField("specificBundlePage", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(note) } : null,
        shipping = Game1.getFarm().getShippingBin(Game1.player).Select(Describe).ToArray(),
        menuHeld = Game1.activeClickableMenu is MenuWithInventory menu ? Describe((Item)nativeHeld.GetValue(menu)) : null
        , storage = Game1.activeClickableMenu is ItemGrabMenu grab ? grab.ItemsToGrabMenu.actualInventory.Select(Describe).ToArray() : null
        , tailoring = Game1.activeClickableMenu is TailoringMenu tailor ? new { left = Describe(tailor.leftIngredientSpot.item), right = Describe(tailor.rightIngredientSpot.item) } : null
        , forge = Game1.activeClickableMenu is ForgeMenu forge ? new { left = Describe(forge.leftIngredientSpot.item), right = Describe(forge.rightIngredientSpot.item) } : null
        , donations = (Game1.getLocationFromName("ArchaeologyHouse") as LibraryMuseum)?.museumPieces.Pairs.Select(pair => new { tile = pair.Key.ToString(), id = pair.Value }).ToArray()
        , chest = testChest?.GetItemsForPlayer().Select(Describe).ToArray()
        , shop = Game1.activeClickableMenu is ShopMenu shop ? new
        {
            shop.currency, shop.inventoryVisible, shop.quantityToSell,
            held = Describe(shop.heldItem as Item),
            stock = shop.itemPriceAndStock.Select(pair => new { id = (pair.Key as Item)?.QualifiedItemId, pair.Value.Price, pair.Value.Stock }).ToArray()
        } : null
    };
    static object Describe(Item item) => item == null ? null : new
    {
        id = item.QualifiedItemId, item.Stack, item.Quality,
        enchantments = item is Tool tool ? tool.enchantments.Select(enchantment => new { type = enchantment.GetType().Name, level = enchantment.GetLevel() }).ToArray() : null,
        data = item.modData.Pairs.ToDictionary(pair => pair.Key, pair => pair.Value)
    };
    object ExportSave(string id)
    {
        string source = Path.Combine(Path.GetFullPath(Game1.savesPath), TestSlot);
        string main = Path.Combine(source, TestSlot), info = Path.Combine(source, "SaveGameInfo");
        if (!File.Exists(main) || !File.Exists(info)) throw new FileNotFoundException("Saved disposable farm pair is missing.");
        string target = Path.Combine(directory, id + ".zip");
        using (var archive = ZipFile.Open(target, ZipArchiveMode.Create))
        {
            archive.CreateEntryFromFile(main, TestSlot + "/" + TestSlot);
            archive.CreateEntryFromFile(info, TestSlot + "/SaveGameInfo");
        }
        return new { file = Path.GetFileName(target), bytes = new FileInfo(target).Length };
    }
    static void RequireNoMenu()
    {
        if (Game1.activeClickableMenu != null || Game1.eventUp || Game1.player.UsingTool)
            throw new InvalidOperationException("Close the test menu/event and stop using tools before setup.");
    }
    object Seed(JsonElement command)
    {
        RequireNoMenu();
        var items = command.GetProperty("items").EnumerateArray().Select(entry =>
        {
            var item = ItemRegistry.Create(entry.GetProperty("item").GetString(),
                entry.GetProperty("count").GetInt32(), entry.TryGetProperty("quality", out var q) ? q.GetInt32() : 0);
            if (entry.TryGetProperty("marker", out var marker)) item.modData["ThorFixture/marker"] = marker.GetString();
            return item;
        }).ToArray();
        if (items.Length > 36) throw new InvalidOperationException("Fixture inventory has more than 36 entries.");
        Game1.player.MaxItems = 36;
        Game1.player.Items.Clear();
        for (int i = 0; i < 36; i++) Game1.player.Items.Add(i < items.Length ? items[i] : null);
        Game1.player.CurrentToolIndex = 0;
        if (command.TryGetProperty("money", out var money)) Game1.player.Money = money.GetInt32();
        if (command.TryGetProperty("time", out var time)) Game1.timeOfDay = time.GetInt32();
        if (command.TryGetProperty("trashLevel", out var trash)) Game1.player.trashCanLevel = trash.GetInt32();
        if (command.TryGetProperty("readJunimo", out var read) && read.GetBoolean()
            && !Game1.player.mailReceived.Contains("canReadJunimoText")) Game1.player.mailReceived.Add("canReadJunimoText");
        return Snapshot();
    }
    object Warp(JsonElement command)
    {
        RequireNoMenu();
        string location = command.GetProperty("location").GetString();
        if (Game1.getLocationFromName(location) == null) throw new InvalidOperationException("Unknown native location.");
        Game1.warpFarmer(location, command.GetProperty("x").GetInt32(), command.GetProperty("y").GetInt32(), false);
        return new { requested = location };
    }
    object Menu(JsonElement command)
    {
        RequireNoMenu();
        string kind = command.GetProperty("kind").GetString();
        if (kind == "level")
        {
            Game1.player.farmingLevel.Value = 5;
            if (!Game1.player.newLevels.Contains(new Point(0, 5))) Game1.player.newLevels.Add(new Point(0, 5));
        }
        if (kind == "dialogue") Game1.currentLocation.afterQuestion = (farmer, answer) => lastAnswer = answer;
        if (kind == "shipping")
        {
            var menu = new ItemGrabMenu(null, reverseGrab: true, showReceivingMenu: false,
                highlightFunction: Utility.highlightShippableObjects, behaviorOnItemSelectFunction: Game1.getFarm().shipItem, message: "",
                snapToBottom: true, canBeExitedWithKey: true, showOrganizeButton: false, context: Game1.getFarm());
            menu.initializeUpperRightCloseButton();
            menu.setBackgroundTransparency(false);
            menu.setDestroyItemOnClick(true);
            menu.initializeShippingBin();
            Game1.activeClickableMenu = menu;
        }
        else if (kind == "chest")
        {
            if (testChest == null)
            {
                testChest = new Chest(true);
                testChest.addItem(ItemRegistry.Create("(O)388", 990));
                testChest.addItem(ItemRegistry.Create("(O)390", 20));
                testChest.modData["ThorFixture/marker"] = "chest";
                var tile = Game1.player.Tile + new Vector2(-2, 0);
                if (Game1.currentLocation.objects.ContainsKey(tile)) throw new InvalidOperationException("Fixture chest tile occupied");
                testChest.TileLocation = tile;
                Game1.currentLocation.objects.Add(tile, testChest);
            }
            testChest.ShowMenu();
        }
        else Game1.activeClickableMenu = kind switch
        {
            "inventory" => new GameMenu(),
            "geode" => new GeodeMenu(),
            "tailoring" => new TailoringMenu(),
            "forge" => new ForgeMenu(),
            "museum" when Game1.currentLocation is LibraryMuseum museum => new MuseumMenu(museum.isItemSuitableForDonation),
            "bundle" when Game1.currentLocation is CommunityCenter center => new JunimoNoteMenu(0, center.bundlesDict()),
            "dialogue" => new DialogueBox("Choose a disposable test answer.", new[] { new Response("TestYes", "Yes"), new Response("TestNo", "No") }),
            "shop" => new ShopMenu("SeedShop", new Dictionary<ISalable, ItemStockInformation>
            {
                [ItemRegistry.Create("(O)472")] = new ItemStockInformation(20, 7),
                [ItemRegistry.Create("(O)24")] = new ItemStockInformation(35, 20)
            }, who: "Pierre"),
            "level" => new LevelUpMenu(0, 5),
            _ => throw new InvalidOperationException("Unsupported native fixture menu")
        };
        return Snapshot();
    }
    object StartTiming() { frames.Clear(); lastFrame = 0; measuring = true; return new { started = true }; }
    object StopTiming()
    {
        measuring = false;
        return new { method = "SMAPI Rendered callback intervals measured by Stopwatch", milliseconds = frames.ToArray() };
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Helper.Events.GameLoop.UpdateTicking -= Update;
            Helper.Events.Display.Rendered -= Rendered;
        }
        base.Dispose(disposing);
    }
}
