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
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;

namespace ThorFixture;

/// <summary>Test-state setup/observation only. No dependency on, or invocation of, production mod handlers.</summary>
public sealed partial class FixtureEntry : Mod
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
                "chest-setup" => ChestSetup(command),
                "crops" => Crops(command),
                "event" => PrepareEvent(command),
                "fridge" => PrepareFridge(command),
                "craft-quest" => PrepareCraftQuest(),
                "ui-stall" => StallUi(),
                "timing-start" => StartTiming(),
                "timing-stop" => StopTiming(),
                "latency-snapshot" => QueueLatencySnapshot(),
                "latency-arm" => ArmLatency(command),
                "latency-read" => ReadLatency(),
                "latency-stop" => StopLatency(),
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
        clubCoins = Game1.player.clubCoins, qiGems = Game1.player.QiGems, festivalScore = Game1.player.festivalScore,
        controller = new { optionEnabled = Game1.options.snappyMenus, Game1.options.gamepadControls, active = Game1.options.SnappyMenus },
        luck = Game1.player.DailyLuck, tomorrow = Game1.weatherForTomorrow,
        raining = Game1.currentLocation.IsRainingHere(),
        health = Game1.player.maxHealth,
        canUnderstandDwarves = Game1.player.canUnderstandDwarves,
        itemsCrafted = Game1.stats.ItemsCrafted,
        quests = Game1.player.questLog.Select(quest => new { id = quest.id.Value, type = quest.GetType().Name, complete = quest.completed.Value }).ToArray(),
        friendship = Game1.player.friendshipData.Pairs.ToDictionary(pair => pair.Key,
            pair => new { pair.Value.Points, pair.Value.GiftsToday, pair.Value.GiftsThisWeek }),
        nearby = Game1.currentLocation.characters.Where(npc => npc.IsVillager).Select(npc => new
        {
            npc.Name, tile = new[] { npc.Tile.X, npc.Tile.Y }, npc.Birthday_Day, npc.Birthday_Season,
            taste = Game1.player.ActiveObject is { } held ? npc.getGiftTasteForThisItem(held) : -1
        }).ToArray(),
        crops = Game1.getFarm().terrainFeatures.Pairs.Where(pair => pair.Value is HoeDirt { crop: not null }).Select(pair =>
            new { tile = pair.Key.ToString(), ready = ((HoeDirt)pair.Value).readyForHarvest(), water = ((HoeDirt)pair.Value).state.Value,
                dead = ((HoeDirt)pair.Value).crop.dead.Value, harvest = ((HoeDirt)pair.Value).crop.indexOfHarvest.Value }).ToArray(),
        menu = Game1.activeClickableMenu?.GetType().FullName, location = Game1.currentLocation.NameOrUniqueName,
        facing = Game1.player.FacingDirection, usingTool = Game1.player.UsingTool,
        canMove = Game1.player.CanMove, standingPixel = Game1.player.StandingPixel.ToString(),
        lastClick = Game1.player.lastClick.ToString(), toolLocation = Game1.player.GetToolLocation().ToString(),
        waterLeft = (Game1.player.CurrentTool as StardewValley.Tools.WateringCan)?.WaterLeft,
        eventUp = Game1.eventUp, currentEvent = Game1.CurrentEvent is { } ev ? new { ev.skippable, ev.skipped, ev.isFestival } : null,
        zoom = Game1.options.zoomLevel, baseZoom = Game1.options.desiredBaseZoomLevel,
        fridge = (Game1.currentLocation as FarmHouse)?.fridge.Value?.GetItemsForPlayer().Select(Describe).ToArray(),
        objects = Game1.currentLocation.objects.Pairs.Where(pair => Vector2.Distance(pair.Key, Game1.player.Tile) <= 8)
            .Select(pair => new { tile = pair.Key.ToString(), item = Describe(pair.Value) }).ToArray(),
        furniture = Game1.currentLocation.furniture.Select(item => new { tile = item.TileLocation.ToString(), item = Describe(item), rotation = item.currentRotation.Value }).ToArray(),
        dirt = Game1.currentLocation.terrainFeatures.Pairs.Where(pair => pair.Value is HoeDirt && Vector2.Distance(pair.Key, Game1.player.Tile) <= 8)
            .Select(pair => new { tile = pair.Key.ToString(), water = ((HoeDirt)pair.Value).state.Value, crop = ((HoeDirt)pair.Value).crop?.indexOfHarvest.Value }).ToArray(),
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
        trashValue = Utility.getTrashReclamationPrice(item, Game1.player),
        clothing = item is Clothing clothing ? new { dyeable = clothing.dyeable.Value, color = clothing.clothesColor.Value.PackedValue } : null,
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
        if (command.TryGetProperty("items", out var suppliedItems))
        {
            var items = suppliedItems.EnumerateArray().Select(CreateItem).ToArray();
            if (items.Length > 36) throw new InvalidOperationException("Fixture inventory has more than 36 entries.");
            Game1.player.MaxItems = 36;
            Game1.player.Items.Clear();
            for (int i = 0; i < 36; i++) Game1.player.Items.Add(i < items.Length ? items[i] : null);
            Game1.player.CurrentToolIndex = 0;
        }
        if (command.TryGetProperty("money", out var money)) Game1.player.Money = money.GetInt32();
        if (command.TryGetProperty("clubCoins", out var coins)) Game1.player.clubCoins = coins.GetInt32();
        if (command.TryGetProperty("controllerMode", out var controller))
        {
            Game1.options.snappyMenus = true;
            Game1.options.gamepadControls = controller.GetBoolean();
        }
        if (command.TryGetProperty("time", out var time)) Game1.timeOfDay = time.GetInt32();
        if (command.TryGetProperty("trashLevel", out var trash)) Game1.player.trashCanLevel = trash.GetInt32();
        if (command.TryGetProperty("day", out var day)) Game1.dayOfMonth = Math.Clamp(day.GetInt32(), 1, 28);
        if (command.TryGetProperty("luck", out var luck)) Game1.player.team.sharedDailyLuck.Value = luck.GetDouble();
        if (command.TryGetProperty("tomorrow", out var tomorrow)) Game1.weatherForTomorrow = tomorrow.GetString();
        if (command.TryGetProperty("zoom", out var zoom))
            StardewValley.Mobile.PinchZoom.Instance.ZoomLevel = Math.Clamp(zoom.GetSingle(), 0.75f, 2f);
        if (command.TryGetProperty("rain", out var rain))
        {
            Game1.isRaining = rain.GetBoolean();
            Game1.netWorldState.Value.GetWeatherForLocation("Default").IsRaining = rain.GetBoolean();
        }
        if (command.TryGetProperty("recipes", out var recipes))
            foreach (var entry in recipes.EnumerateArray())
            {
                string name = entry.GetString();
                if (!CraftingRecipe.craftingRecipes.ContainsKey(name)) throw new InvalidOperationException("Unknown native recipe");
                if (!Game1.player.craftingRecipes.ContainsKey(name)) Game1.player.craftingRecipes.Add(name, 0);
            }
        if (command.TryGetProperty("friends", out var friends))
            foreach (var entry in friends.EnumerateArray())
            {
                string name = entry.GetProperty("name").GetString();
                if (Game1.getCharacterFromName(name) == null) throw new InvalidOperationException("Unknown native villager");
                Game1.player.friendshipData[name] = new Friendship(entry.GetProperty("points").GetInt32());
            }
        if (command.TryGetProperty("readJunimo", out var read) && read.GetBoolean()
            && !Game1.player.mailReceived.Contains("canReadJunimoText")) Game1.player.mailReceived.Add("canReadJunimoText");
        return Snapshot();
    }
    static Item CreateItem(JsonElement entry)
    {
        var item = ItemRegistry.Create(entry.GetProperty("item").GetString(),
            entry.GetProperty("count").GetInt32(), entry.TryGetProperty("quality", out var q) ? q.GetInt32() : 0);
        if (entry.TryGetProperty("marker", out var marker)) item.modData["ThorFixture/marker"] = marker.GetString();
        return item;
    }
    object ChestSetup(JsonElement command)
    {
        RequireNoMenu();
        var tile = new Vector2(command.GetProperty("x").GetInt32(), command.GetProperty("y").GetInt32());
        if (Game1.currentLocation.objects.ContainsKey(tile)) throw new InvalidOperationException("Fixture chest tile occupied");
        var chest = new Chest(true, tile, command.TryGetProperty("item", out var kind) ? kind.GetString() : "130");
        var items = command.GetProperty("items").EnumerateArray().Select(CreateItem).ToArray();
        if (items.Length > chest.GetActualCapacity()) throw new InvalidOperationException("Fixture chest capacity exceeded");
        chest.GetItemsForPlayer().Clear();
        foreach (var item in items) chest.GetItemsForPlayer().Add(item);
        chest.modData["ThorFixture/marker"] = "chest";
        Game1.currentLocation.objects.Add(tile, chest);
        testChest = chest;
        return Snapshot();
    }
    object Crops(JsonElement command)
    {
        RequireNoMenu();
        var farm = Game1.getFarm();
        foreach (var entry in command.GetProperty("tiles").EnumerateArray())
        {
            int x = entry.GetProperty("x").GetInt32(), y = entry.GetProperty("y").GetInt32();
            var tile = new Vector2(x, y);
            if (farm.objects.ContainsKey(tile)) throw new InvalidOperationException("Fixture crop tile has an object");
            var crop = entry.TryGetProperty("seed", out var seed) ? new Crop(seed.GetString(), x, y, farm) : null;
            if (crop != null && entry.TryGetProperty("ready", out var ready) && ready.GetBoolean()) crop.growCompletely();
            farm.terrainFeatures[tile] = new HoeDirt(entry.GetProperty("water").GetInt32(), farm) { crop = crop };
        }
        return Snapshot();
    }
    object StallUi()
    {
        new Android.OS.Handler(Android.OS.Looper.MainLooper).Post(() => Thread.Sleep(1200));
        return new { requestedMilliseconds = 1200, target = "Android UI thread" };
    }
    object PrepareEvent(JsonElement command)
    {
        RequireNoMenu();
        if (Game1.currentLocation is not FarmHouse) throw new InvalidOperationException("Event fixture requires FarmHouse.");
        bool skippable = command.GetProperty("skippable").GetBoolean();
        string flag = skippable ? "skippable" : "unskippable";
        Game1.currentLocation.startEvent(new Event($"none/follow/farmer 5 6 2/{flag}/precisePause 15000/end"));
        return Snapshot();
    }
    object PrepareFridge(JsonElement command)
    {
        RequireNoMenu();
        if (Game1.currentLocation is not FarmHouse house) throw new InvalidOperationException("Fridge fixture requires FarmHouse.");
        if (command.TryGetProperty("upgrade", out var upgrade) && upgrade.GetBoolean() && house.upgradeLevel == 0)
        {
            house.moveObjectsForHouseUpgrade(1);
            house.setMapForUpgradeLevel(1);
        }
        var items = command.GetProperty("items").EnumerateArray().Select(CreateItem).ToArray();
        if (items.Length > house.fridge.Value.GetActualCapacity()) throw new InvalidOperationException("Fridge fixture capacity exceeded");
        house.fridge.Value.GetItemsForPlayer().Clear();
        foreach (var item in items) house.fridge.Value.GetItemsForPlayer().Add(item);
        return Snapshot();
    }
    object PrepareCraftQuest()
    {
        RequireNoMenu();
        string output = new CraftingRecipe("Scarecrow").createItem().QualifiedItemId;
        var quest = DataLoader.Quests(Game1.content)
            .Where(pair => pair.Value.StartsWith("Crafting/", StringComparison.Ordinal))
            .Select(pair => StardewValley.Quests.Quest.getQuestFromId(pair.Key))
            .OfType<StardewValley.Quests.CraftingQuest>()
            .First(candidate => candidate.ItemId.Value == output);
        if (Game1.player.questLog.Any(existing => existing.id.Value == quest.id.Value))
            throw new InvalidOperationException("Crafting test quest already present; preserve its progression.");
        Game1.player.addQuest(quest.id.Value);
        return Snapshot();
    }
    object Warp(JsonElement command)
    {
        RequireNoMenu();
        string location = command.GetProperty("location").GetString();
        if (location == "fixture-mine")
        {
            Game1.enterMine(1);
            return new { requested = "UndergroundMine1" };
        }
        if (Game1.getLocationFromName(location) == null) throw new InvalidOperationException("Unknown native location.");
        Game1.warpFarmer(location, command.GetProperty("x").GetInt32(), command.GetProperty("y").GetInt32(), false);
        return new { requested = location };
    }
    object Menu(JsonElement command)
    {
        RequireNoMenu();
        string kind = command.GetProperty("kind").GetString();
        int skill = command.TryGetProperty("skill", out var requestedSkill) ? requestedSkill.GetInt32() : 0;
        int level = command.TryGetProperty("level", out var requestedLevel) ? requestedLevel.GetInt32() : 5;
        if (kind == "level")
        {
            if (skill < 0 || skill > 4 || level < 1 || level > 10) throw new InvalidOperationException("Invalid test skill/level");
            if (command.TryGetProperty("professions", out var professions))
            {
                Game1.player.professions.Clear();
                foreach (var entry in professions.EnumerateArray()) Game1.player.professions.Add(entry.GetInt32());
            }
            var field = new[] { Game1.player.farmingLevel, Game1.player.fishingLevel, Game1.player.foragingLevel, Game1.player.miningLevel, Game1.player.combatLevel }[skill];
            field.Value = level;
            Game1.player.newLevels.Clear(); Game1.player.newLevels.Add(new Point(skill, level));
        }
        if (kind == "dialogue") Game1.currentLocation.afterQuestion = (farmer, answer) => lastAnswer = answer;
        if (kind == "no-room") Game1.player.addItemByMenuIfNecessaryElseHoldUp(ItemRegistry.Create("(O)24", 3, 2));
        else if (kind == "museum-rewards" && Game1.currentLocation is LibraryMuseum museumRewards) museumRewards.OpenRewardMenu();
        else if (kind == "shipping")
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
            }, currency: command.TryGetProperty("currency", out var currency) ? currency.GetInt32() : 0, who: "Pierre"),
            "level" => new LevelUpMenu(skill, level),
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
            latencyUi.Post(DetachLatency);
        }
        base.Dispose(disposing);
    }
}
