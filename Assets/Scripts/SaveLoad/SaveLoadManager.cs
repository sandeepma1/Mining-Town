using System;
using System.Collections.Generic;
using System.IO;
using MiningTown.IO;
using UnityEngine;

public static class SaveLoadManager
{
    public static SaveData saveData;
    private static string fileFullPath;

    private const int totalSaveEntries = 2; //responsible to track all types of save triggers
    private static int currentSaveEntries = 0;
    private static bool isSaveDataLoaded;
    private static Scenes loadedScene;

    public static void LoadOrCreateNewGame()
    {
        SceneLoader.OnSceneChanged += OnLevelLoading;
        if (isSaveDataLoaded)
        {
            return;
        }
        fileFullPath = Application.persistentDataPath + "/SaveData.json";
        if (!File.Exists(fileFullPath))
        {
            NewGameSaveData();
        }
        else
        {
            LoadGame();
        }
        OnLoadFarmItemsData();
        OnLoadBackpackItemsData();
    }

    private static void OnLevelLoading(Scenes scene)
    {
        loadedScene = scene;
    }

    public static void RemoveFruitTree(FruitTreeData fruitTreeData)
    {
        saveData.fruitTreeDatas.Remove(fruitTreeData);
    }


    #region RaisedBeds
    public static List<RaisedBedData> GetAllRaisedBeds()
    {
        return saveData.raisedBedDatas;
    }

    public static void AddNewRaisedBeds(RaisedBedData raisedBedData)
    {
        saveData.raisedBedDatas.Add(raisedBedData);
    }

    public static int GetRaisedBedsCount()
    {
        return saveData.raisedBedDatas.Count;
    }
    #endregion


    #region ProdBuildings
    public static List<ProdBuildingData> GetAllProdBuildings()
    {
        return saveData.prodBuildingDatas;
    }

    public static void AddNewProdBuilding(ProdBuildingData prodBuildingData)
    {
        saveData.prodBuildingDatas.Add(prodBuildingData);
    }

    public static int GetProdBuildingsCount()
    {
        return saveData.prodBuildingDatas.Count;
    }
    #endregion


    #region Livestock
    public static List<LivestockData> GetAllLivestocks()
    {
        return saveData.livestockDatas;
    }

    public static void AddNewLivestock(LivestockData livestockData)
    {
        saveData.livestockDatas.Add(livestockData);
    }

    public static int GetLivestocksCount()
    {
        return saveData.livestockDatas.Count;
    }
    #endregion


    #region FruitTrees
    public static List<FruitTreeData> GetAllFruitTrees()
    {
        return saveData.fruitTreeDatas;
    }

    public static void AddNewFruitTree(FruitTreeData fruitTreeData)
    {
        saveData.fruitTreeDatas.Add(fruitTreeData);
    }

    public static int GetFruitTreesCount()
    {
        return saveData.fruitTreeDatas.Count;
    }
    #endregion


    #region Decorations
    public static List<DecorationData> GetAllDecorations()
    {
        return saveData.decorationDatas;
    }

    public static void AddDecoration(DecorationData decorationData)
    {
        saveData.decorationDatas.Add(decorationData);
    }

    public static int GetDecorationsCount()
    {
        return saveData.decorationDatas.Count;
    }
    #endregion


    //This is common fuction between Barn Item and Backpack
    public static void AddUpdateItem(int itemId, int count)
    {
        switch (loadedScene)
        {
            case Scenes.Loading:
                break;
            case Scenes.FarmHome:
                AddUpdateItemInBarn(itemId, count);
                break;
            case Scenes.Mines:
                AddUpdateItemInBackpack(itemId, count);
                break;
            case Scenes.Forest:
                AddUpdateItemInBackpack(itemId, count);
                break;
            case Scenes.Town:
                AddUpdateItemInBackpack(itemId, count);
                break;
            default:
                break;
        }
    }


    #region Farm Barn Items
    public static Dictionary<int, int> uiBarnItemsDict = new Dictionary<int, int>(); //itemID, count
    public static List<ItemIdWithCount> GetAllFarmItems()
    {
        return saveData.barnItems;
    }

    private static void OnLoadFarmItemsData()
    {
        for (int i = 0; i < saveData.barnItems.Count; i++)
        {
            uiBarnItemsDict.Add(saveData.barnItems[i].itemId, saveData.barnItems[i].itemCount);
        }
    }

    public static void AddUpdateItemInBarn(int itemId, int count)
    {
        if (uiBarnItemsDict.ContainsKey(itemId)) //Update count
        {
            uiBarnItemsDict[itemId] += count;
        }
        else //Create new item
        {
            uiBarnItemsDict.Add(itemId, count);
        }
        UiFarmBarnInventory.OnAddUpdateItemToBarn?.Invoke(itemId, count);
    }

    public static bool RemoveFarmBarnItem(int itemId, int count)
    {
        if (!uiBarnItemsDict.ContainsKey(itemId))
        {
            return false;
        }

        if (count <= uiBarnItemsDict[itemId])
        {
            uiBarnItemsDict[itemId] -= count;
            UiFarmBarnInventory.OnReduceItemInInventory?.Invoke(itemId, count);
            if (uiBarnItemsDict[itemId] <= 0)
            {
                uiBarnItemsDict.Remove(itemId);
                UiFarmBarnInventory.OnRemoveItemFromInventory?.Invoke(itemId);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public static int DoesItemExistsReturnCount(int itemId)
    {
        if (uiBarnItemsDict.ContainsKey(itemId))
        {
            return uiBarnItemsDict[itemId];
        }
        else
        {
            return 0;
        }
    }
    #endregion


    #region Backpack Items
    public static Dictionary<int, int> backpackItemsDict = new Dictionary<int, int>(); //itemID, count

    private static void OnLoadBackpackItemsData()
    {
        for (int i = 0; i < saveData.backpackData.items.Count; i++)
        {
            backpackItemsDict.Add(saveData.backpackData.items[i].itemId, saveData.backpackData.items[i].itemCount);
        }
    }

    public static void AddUpdateItemInBackpack(int itemId, int count)
    {
        UiPlayerBackpackCanvas.OnAddItemToBackpack?.Invoke(itemId, count);
    }

    public static int DoesItemExistsInBackpackReturnCount(int itemId)
    {
        if (uiBarnItemsDict.ContainsKey(itemId))
        {
            return uiBarnItemsDict[itemId];
        }
        else
        {
            return 0;
        }
    }
    #endregion


    #region Save all triggers coming from various parts of game
    //General Save Data
    public static void SavePlayerEnergy(Vector3 position)
    {
        saveData.playerStats.playerPosition = position;
    }

    //Trigger 1
    public static void SaveFarmBarnItems(List<ItemIdWithCount> farmItems)
    {
        if (saveData == null)
            return;

        saveData.barnItems = new List<ItemIdWithCount>();
        saveData.barnItems = farmItems;

        //Energy
        if (saveData.playerStats.nextEnergyDateTime != null)
        {
            saveData.playerStats.nextEnergyDateTimeJson = JsonUtility.ToJson((JsonDateTime)saveData.playerStats.nextEnergyDateTime);
        }
        currentSaveEntries++;
        //Debug.Log("SaveFarmBarnItems");
        CheckIfAllEntriesDone();
    }

    public static void SavePlayerBackpackItems(List<ItemIdWithCount> backpackItems)
    {
        if (saveData == null)
            return;

        saveData.backpackData.items = new List<ItemIdWithCount>();
        saveData.backpackData.items = backpackItems;
        currentSaveEntries++;
        //Debug.Log("Saving backpack");
        CheckIfAllEntriesDone();
    }

    private static void CheckIfAllEntriesDone()
    {
        if (currentSaveEntries == totalSaveEntries)
        {
            currentSaveEntries = 0;
            SaveGame();
        }
    }
    #endregion


    #region Main Save Load Functions
    private static void NewGameSaveData()
    {
        Debug.Log("NewGameSaveData");
        saveData = new SaveData();
        saveData.NewSaveData();
        SaveGame();
    }

    private static void LoadGame()
    {
        saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(fileFullPath));
        saveData.playerStats.playerPosition = GameVariables.playerHomePosition;
        isSaveDataLoaded = true;
    }

    private static void SaveGame()
    {
        string save = JsonUtility.ToJson(saveData);
        File.WriteAllText(fileFullPath, save);
        Debug.Log("All Data saved");
    }

    public static void ResetGameData()
    {
        if (File.Exists(fileFullPath))
        {
            saveData = null;
            File.Delete(fileFullPath);
        }
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }
    #endregion
}


[System.Serializable]
public class SaveData
{
    //All player releated stats
    public PlayerStats playerStats;
    public PlayerBackpackData backpackData;

    public List<ItemIdWithCount> barnItems = new List<ItemIdWithCount>(); //itemID, count
    public List<RaisedBedData> raisedBedDatas = new List<RaisedBedData>();
    public List<ProdBuildingData> prodBuildingDatas = new List<ProdBuildingData>();
    public List<LivestockData> livestockDatas = new List<LivestockData>();
    public List<FruitTreeData> fruitTreeDatas = new List<FruitTreeData>();
    public List<DecorationData> decorationDatas = new List<DecorationData>();

    public SaveData()
    {

    }

    public void NewSaveData()
    {
        //All player releated stats, edit this constructor
        playerStats = new PlayerStats();
        backpackData = new PlayerBackpackData();

        barnItems.Add(new ItemIdWithCount(1, 5));
        barnItems.Add(new ItemIdWithCount(2, 5));
        //barnItems.Add(new ItemIdWithCount(3, 50));

        raisedBedDatas.Add(new RaisedBedData(new Vector3(2f, 0, -2)));//Raised Bed 1
        raisedBedDatas.Add(new RaisedBedData(new Vector3(6f, 0, -2)));//Raised Bed 2
        raisedBedDatas.Add(new RaisedBedData(new Vector3(10f, 0, -2f)));//Raised Bed 3

        prodBuildingDatas.Add(new ProdBuildingData(new Vector3(16f, 0, -2f), 2, 2));//Bakery
        //prodBuildingDatas.Add(new ProdBuildingData(new Vector3(4.5f, 0, -9.5f), 7, 2));//Juice Maker

        //livestockDatas.Add(new LivestockData(new Vector3(8, 0, -10), 3, 2));//Chicken Coop

        fruitTreeDatas.Add(new FruitTreeData(new Vector3(21, 0, -2f), 5));//Apple Tree
        fruitTreeDatas.Add(new FruitTreeData(new Vector3(24, 0, -2f), 6));//Apple Tree

        //fruitTreeDatas.Add(new FruitTreeData(new Vector3(-22, 0, -2), 6));//Orange Tree
        //fruitTreeDatas.Add(new FruitTreeData(new Vector3(-19, 0, -2), 6));//Orange Tree

        // decorationDatas.Add(new DecorationData(1, new Vector3(-6.5f, 0, -4)));//Decoration item
    }
}

[System.Serializable]
public class PlayerStats
{
    public int playerLevel;
    public int currentEnergy;
    public int maxEnergy;
    public DateTime nextEnergyDateTime;
    public string nextEnergyDateTimeJson;
    public int currentHealth;
    public int maxHealth;
    public int playerXp;
    public int coins;
    public int gems;
    public Vector3 playerPosition;
    public int currentForestLevel;
    public int currentMinesLevel;
    public bool isInGodMode;//Temp remove this;
    public PlayerStats()
    {
        playerPosition = new Vector3(-14, 0, -15);
        playerLevel = 1;
        playerXp = 0;
        coins = 200;
        gems = 20;
        maxEnergy = 250;
        currentEnergy = maxEnergy;
        currentForestLevel = 1;
        currentMinesLevel = 1;
        maxHealth = 50;
        currentHealth = maxHealth;
        nextEnergyDateTimeJson = JsonUtility.ToJson((JsonDateTime)DateTime.UtcNow);
    }
}

[System.Serializable]
public class PlayerBackpackData
{
    public int backpackSlotsCount;
    public int maxSlotSize;
    public List<ItemIdWithCount> items;
    public PlayerBackpackData()
    {
        backpackSlotsCount = GameVariables.bp_SlotsColumnCount;
        maxSlotSize = 9;
        items = new List<ItemIdWithCount>();
    }
}

[System.Serializable]
public class DecorationData
{
    public int decoId;
    public Vector3 pos;

    public DecorationData(int decoId, Vector3 pos) //New game default constructor
    {
        this.decoId = decoId;
        this.pos = pos;
    }
}

[System.Serializable]
public class FruitTreeData
{
    public int sourceId;
    public int itemId;
    public int maxHarvestTimes;
    public Vector3 pos;
    public FruitTreeState fruitTreeState;
    public DateTime yieldDateTime;
    public string yieldDateTimeJson;

    public FruitTreeData(Vector3 pos, int sourceId) //New game default constructor
    {
        Receipe receipe = ItemReceipesDatabase.GetFruitTreeReceipeBySourceId(sourceId);
        if (receipe == null)
        {
            Debug.LogError("ItemReceipesDatabase did not loaded yet");
        }
        this.sourceId = sourceId;
        itemId = receipe.outputItemId;
        maxHarvestTimes = receipe.coinCost; //This is also max harves times in csv file
        this.pos = pos;
        fruitTreeState = FruitTreeState.Growing;
        Item currentFruidTreeItem = ItemDatabase.GetItemById(ItemReceipesDatabase.GetFruitTreeReceipeBySourceId(sourceId).outputItemId);
        DateTime time = DateTime.UtcNow.AddMinutes(currentFruidTreeItem.yieldDurationInMins);
        yieldDateTimeJson = JsonUtility.ToJson((JsonDateTime)time);
    }
}

[System.Serializable]
public class ProdBuildingData
{
    public int sourceId;
    public Vector3 pos;
    public ProdBuildingState prodBuildingState;
    public int unlockedQueueCount;
    public List<int> queuedItemIds = new List<int>();
    public List<int> doneItemIds = new List<int>();
    public List<DateTime> yieldDateTimes = new List<DateTime>();
    public List<string> yieldDateTimeJsons = new List<string>();

    public ProdBuildingData(Vector3 pos, int sourceId, int startUnlockCount) //New game default constructor
    {
        unlockedQueueCount = startUnlockCount;
        this.sourceId = sourceId;
        this.pos = pos;
        prodBuildingState = ProdBuildingState.Idle;
    }
}

[System.Serializable]
public class LivestockData
{
    public int sourceId;
    public Vector3 pos;
    public int unlockedQueueCount;
    public int outputItemId;

    public LivestockState[] livestockStates = new LivestockState[GameVariables.maxLivestock];
    public DateTime[] yieldDateTimes = new DateTime[GameVariables.maxLivestock];
    public string[] yieldDateTimeJsons = new string[GameVariables.maxLivestock];

    public LivestockData(Vector3 pos, int sourceId, int startUnlockCount) //New game default constructor
    {
        unlockedQueueCount = startUnlockCount;
        this.sourceId = sourceId;
        this.pos = pos;
        outputItemId = ItemReceipesDatabase.GetAllReceipeBySourceId(sourceId)[0].outputItemId;
        for (int i = 0; i < GameVariables.maxLivestock; i++)
        {
            livestockStates[i] = LivestockState.Hungry;
        }
    }
}
[System.Serializable]
public class SingleLivestock
{
    public LivestockState livestockState;
    public DateTime yieldDateTime;
    public TimeSpan timeSpan;
    public string yieldDateTimeJson;
    public SingleLivestock()
    {
        livestockState = LivestockState.Hungry;
        DateTime time = DateTime.UtcNow;
        yieldDateTimeJson = JsonUtility.ToJson((JsonDateTime)time);
    }
}

[System.Serializable]
public class RaisedBedData
{
    public Vector3 pos;
    public int currentCropId;
    public RaisedBedState raisedBedState;
    public DateTime yieldDateTime;
    public string yieldDateTimeJson;

    public RaisedBedData(Vector3 pos) //New game default constructor
    {
        this.pos = pos;
        raisedBedState = RaisedBedState.Idle;

        DateTime time = DateTime.UtcNow;
        yieldDateTimeJson = JsonUtility.ToJson((JsonDateTime)time);
    }
}

[System.Serializable]
public class JsonDateTime
{
    public long value;

    public static implicit operator DateTime(JsonDateTime jdt)
    {
        return DateTime.FromFileTimeUtc(jdt.value);
    }
    public static implicit operator JsonDateTime(DateTime dt)
    {
        JsonDateTime jdt = new JsonDateTime();
        if (dt.Ticks > 0)
        {
            jdt.value = dt.ToFileTimeUtc();
            return jdt;
        }
        //else
        //{
        //    jdt.value = new DateTime().AddSeconds(1).ToFileTimeUtc();
        //    return jdt;
        //}
        return null;
    }

    //Example
    //DateTime time = DateTime.UtcNow;
    //print(time);
    //string json = JsonUtility.ToJson((JsonDateTime)time);
    //print(json);
    //DateTime timeFromJson = JsonUtility.FromJson<JsonDateTime>(json);
    //print(timeFromJson);
}

[System.Serializable]
public class ItemIdWithCount
{
    public int itemId;
    public int itemCount;
    public ItemIdWithCount()
    {
        itemId = 0;
        itemCount = 0;
    }
    public ItemIdWithCount(int id, int count)
    {
        itemId = id;
        itemCount = count;
    }
}

public class RaisedBedCropLayout
{
    private static Vector3[] spaces = new Vector3[] {
        new Vector3(-1, 0, 1),
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, -1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, -1)
    };
    public static Vector3[] layout2x2 = new[] { new Vector3(-0.75f, 0f, 0.25f), new Vector3(0.75f, 0f, 0.25f),
                                       new Vector3(-0.75f,0f, -1.25f), new Vector3(0.75f, 0f, -1.25f) };

    //0     1       2
    //3     4       5
    //6     7       8
    public static Dictionary<int, List<Vector3>> layouts = new Dictionary<int, List<Vector3>> {
        {1, new List<Vector3>(new Vector3[] { spaces[4] })},
        {2, new List<Vector3>(new Vector3[] { spaces[3], spaces[5] })},
        {3, new List<Vector3>(new Vector3[] { spaces[3], spaces[4], spaces[5] })},
        {4, new List<Vector3>(new Vector3[] { spaces[0], spaces[2], spaces[6], spaces[8] })},
        {5, new List<Vector3>(new Vector3[] { spaces[0], spaces[2], spaces[4], spaces[6], spaces[8] })},
        {6, new List<Vector3>(new Vector3[] { spaces[0], spaces[1], spaces[2], spaces[6], spaces[7], spaces[8] })},
        {7, new List<Vector3>(new Vector3[] { spaces[0], spaces[1], spaces[2], spaces[4], spaces[6], spaces[7], spaces[8] })},
        {8, new List<Vector3>(new Vector3[] { spaces[0], spaces[1], spaces[2], spaces[3], spaces[5], spaces[6], spaces[7], spaces[8] })},
        {9, new List<Vector3>(new Vector3[] { spaces[0], spaces[1], spaces[2], spaces[3], spaces[4], spaces[5], spaces[6], spaces[7], spaces[8] })},
    };
}