using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MiningTown.IO
{
    public static class ItemReceipesDatabase
    {
        private static Dictionary<int, Receipe> receipes = new Dictionary<int, Receipe>();
        private const string fileName = "ItemReceipes";

        public static int AllItemCount
        {
            get
            {
                return receipes.Count;
            }
        }

        public static void LoadDatabase()
        {
            List<string> linesList = DatabaseBase.GetAllLinesFromCSV(fileName);
            for (int i = 0; i < linesList.Count; i++)
            {
                string[] chars = Regex.Split(linesList[i], ",");
                if (!receipes.ContainsKey(chars[1].ToInt()))
                {
                    receipes.Add(chars[1].ToInt(), new Receipe(chars[1].ToInt(), chars[3].ToInt(), chars[5].ToInt(), chars[6].ToInt(),
                    chars[8].ToInt(), chars[9].ToInt(), chars[11].ToInt(), chars[12].ToInt(), chars[13].ToInt()));
                }
            }
        }

        public static Receipe GetReceipeById(int itemId)
        {
            return receipes[itemId];
        }

        public static int GetReceipelength()
        {
            return receipes.Count;
        }

        public static int GetCoinCostById(int itemId)
        {
            if (receipes.ContainsKey(itemId))
            {
                return receipes[itemId].coinCost;
            }
            return 0;
        }

        public static int GetSourceIdByOutputItemId(int outputItemId)
        {
            return receipes[outputItemId].sourceItemId;
        }

        public static bool DoesHasAllItemsInReceipe(int outputItemId, bool remove = false)
        {
            int haveItemCount1 = SaveLoadManager.DoesItemExistsReturnCount(receipes[outputItemId].reqId1);
            int haveItemCount2 = SaveLoadManager.DoesItemExistsReturnCount(receipes[outputItemId].reqId2);
            int haveItemCount3 = SaveLoadManager.DoesItemExistsReturnCount(receipes[outputItemId].reqId3);

            if (receipes[outputItemId].reqCount1 <= haveItemCount1 &&
                receipes[outputItemId].reqCount2 <= haveItemCount2 &&
                receipes[outputItemId].reqCount3 <= haveItemCount3)
            {
                if (remove)
                {
                    SaveLoadManager.RemoveFarmBarnItem(receipes[outputItemId].reqId1, receipes[outputItemId].reqCount1);
                    SaveLoadManager.RemoveFarmBarnItem(receipes[outputItemId].reqId2, receipes[outputItemId].reqCount2);
                    SaveLoadManager.RemoveFarmBarnItem(receipes[outputItemId].reqId3, receipes[outputItemId].reqCount3);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<Receipe> GetAllReceipeBySourceId(int sourceId)
        {
            List<Receipe> tempItems = new List<Receipe>();
            foreach (KeyValuePair<int, Receipe> item in receipes)
            {
                if (item.Value.sourceItemId == sourceId)
                {
                    tempItems.Add(item.Value);
                }
            }
            return tempItems;
        }

        public static Receipe GetFruitTreeReceipeBySourceId(int sourceId)
        {
            List<Receipe> tempItems = new List<Receipe>();
            foreach (KeyValuePair<int, Receipe> item in receipes)
            {
                if (item.Value.sourceItemId == sourceId)
                {
                    tempItems.Add(item.Value);
                }
            }
            if (tempItems.Count > 0)
            {
                if (tempItems.Count >= 2)
                {
                    Debug.LogError("Incorrect receipe processed, check this ");
                    return null;
                }
                else
                {
                    return tempItems[0];
                }
            }
            else
            {
                return null;
            }
        }
    }
}

[System.Serializable]
public class Receipe
{
    //     0       1           2       3       4       5       6           7       8       9       10      11      12          13
    //outputName,outputId,sourceName,sourceId,reqName1,reqId1,reqCount1,reqName2,reqId2,reqCount2,reqName3,reqId3,reqCount3,coinCost

    public int outputItemId;
    public int sourceItemId;
    public int reqId1;
    public int reqCount1;
    public int reqId2;
    public int reqCount2;
    public int reqId3;
    public int reqCount3;
    public int coinCost;

    public Receipe(int outputItemId, int sourceItemId, int reqId1, int reqCount1, int reqId2, int reqCount2,
        int reqId3, int reqCount3, int coinCost)
    {
        this.outputItemId = outputItemId;
        this.sourceItemId = sourceItemId;
        this.reqId1 = reqId1;
        this.reqCount1 = reqCount1;
        this.reqId2 = reqId2;
        this.reqCount2 = reqCount2;
        this.reqId3 = reqId3;
        this.reqCount3 = reqCount3;
        this.coinCost = coinCost;
    }
}