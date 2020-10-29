using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

namespace MiningTown.IO
{
    public static class ItemDatabase
    {
        private static Dictionary<int, Item> items = new Dictionary<int, Item>();
        private const string fileName = "AllItems";

        public static void LoadItemDatabase()
        {
            List<string> linesList = fileName.ToCsvLines();
            for (int i = 0; i < linesList.Count; i++)
            {
                string[] chars = Regex.Split(linesList[i], ",");
                if (!items.ContainsKey(chars[0].ToInt()))
                {
                    items.Add(chars[0].ToInt(),
                        new Item(chars[0].ToInt(),
                        chars[1],
                        chars[2],
                        chars[3],
                        chars[4].ToInt(),
                        chars[5].ToInt(),
                        chars[6].ToInt(),
                        chars[7].ToInt(),
                        chars[8].ToInt(),
                        chars[9].ToInt(),
                        chars[10].ToInt(),
                        chars[11].ToInt(),
                        chars[12],
                        chars[13].ToInt()));
                }
            }
        }
        public static int GetEnergyRestoreItemId(int itemId)
        {
            return items[itemId].energyRestore;
        }
        public static int GetHealthRestoreItemId(int itemId)
        {
            return items[itemId].healthRestore;
        }
        public static int GetCategoryIdByItemId(int itemId)
        {
            return items[itemId].categoryId;
        }

        public static List<int> GetAllItemsByCategoryId(int categoryId)
        {
            List<int> itemIds = new List<int>();
            foreach (KeyValuePair<int, Item> item in items)
            {
                if (item.Value.categoryId == categoryId)
                {
                    itemIds.Add(item.Key);
                }
            }
            return itemIds;
        }

        public static int GetRemoveByCoinByItemId(int itemId)
        {
            return items[itemId].removeByCoins;
        }

        public static int GetYieldDurationInMinsById(int itemId)
        {
            return items[itemId].yieldDurationInMins;
        }

        public static Item GetItemById(int itemId)
        {
            return items[itemId];
        }

        public static string GetItemNameById(int itemId)
        {
            return items[itemId].name;
        }

        public static string GetItemSlugById(int itemId)
        {
            if (items.ContainsKey(itemId))
            {
                return items[itemId].slug;
            }
            return null;
        }

        public static int GetItemXpOnHarvestById(int itemId)
        {
            if (items.ContainsKey(itemId))
            {
                return items[itemId].xpOnHarvest;
            }
            return 0;
        }

        public static int GetBuyValueInGemsByItemId(int itemId)
        {
            return items[itemId].buyValueInGems;
        }

        public static int GetItemslength()
        {
            return items.Count;
        }

        public static Dictionary<int, Item> GetAllItems()
        {
            return items;
        }
    }
}

[System.Serializable]
public class Item
{   // 0    1    2           3            4               5            6               7            8           9               10              11      12      13
    //id,name,category,description,energyRestore,healthRestore,buyValueInGems,sellValueInCoins,xpOnSell,yieldDurationInMins,xpOnHarvest,removeByCoins,slug,categoryId

    public int itemId;
    public string name;
    public string category;
    public string description;
    public int energyRestore;
    public int healthRestore;
    public int buyValueInGems;
    public int sellValueInCoins;
    public int xpOnSell;
    public int yieldDurationInMins;
    public int xpOnHarvest;
    public int removeByCoins;
    public string slug;
    public int categoryId;

    public Item() { itemId = -1; }

    public Item(int itemId, string name, string category, string description, int energyRestore, int healthRestore,
        int buyValueInGems, int sellValueInCoins, int xpOnSell, int yieldDurationInMins, int xpOnHarvest,
        int removeByCoins, string slug, int categoryId)
    {
        this.itemId = itemId;
        this.name = name;
        this.category = category;
        this.description = description;
        this.energyRestore = energyRestore;
        this.healthRestore = healthRestore;
        this.buyValueInGems = buyValueInGems;
        this.sellValueInCoins = sellValueInCoins;
        this.xpOnSell = xpOnSell;
        this.yieldDurationInMins = yieldDurationInMins;
        this.xpOnHarvest = xpOnHarvest;
        this.removeByCoins = removeByCoins;
        this.slug = slug;
        this.categoryId = categoryId;
    }
}