using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MiningTown.IO
{
    public static class LivestockDatabase
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
            List<string> linesList = GetAllLinesFromCSV(fileName);
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

        public static int GetRequired3CoinCostById(int itemId)
        {
            if (receipes.ContainsKey(itemId))
            {
                return receipes[itemId].reqCount3;
            }
            return 0;
        }

        public static int GetSourceIdByOutputItemId(int outputItemId)
        {
            return receipes[outputItemId].sourceItemId;
        }

        public static Receipe GetRequiredReceipeByOutputItemId(int outputItemId)
        {
            return receipes[outputItemId];
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

        public static List<string> GetAllLinesFromCSV(string fileName)
        {
            TextAsset itemCSV = Resources.Load("GameCsv/" + fileName) as TextAsset;
            List<string> linesList = Regex.Split(itemCSV.text, "\r\n").ToList<string>();
            linesList.RemoveAt(0); // Remove first item as CSV has column names
            linesList.RemoveAt(linesList.Count - 1); // Warning: Remove last item as CSV one blank line at the end
            return linesList;
        }
    }
}