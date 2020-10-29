using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MiningTown.IO
{
    public static class CropsDatabase
    {
        private static Dictionary<int, Crops> crops = new Dictionary<int, Crops>();
        private const string fileName = "Crops";

        public static void LoadDatabase()
        {
            List<string> linesList = DatabaseBase.GetAllLinesFromCSV(fileName);
            for (int i = 0; i < linesList.Count; i++)
            {
                string[] chars = Regex.Split(linesList[i], ",");
                if (!crops.ContainsKey(chars[1].ToInt()))
                {
                    crops.Add(chars[1].ToInt(), new Crops(chars[1].ToInt(), chars[2].ToInt()));
                }
            }
        }

        public static Crops GetCropById(int cropId)
        {
            if (crops.ContainsKey(cropId))
            {
                return crops[cropId];
            }
            return null;
        }

        public static int GetCoinCostById(int itemId)
        {
            if (crops.ContainsKey(itemId))
            {
                return crops[itemId].coinCost;
            }
            return 0;
        }

        public static List<Crops> GetAllCrops()
        {
            List<Crops> tempCrops = new List<Crops>();
            foreach (KeyValuePair<int, Crops> item in crops)
            {
                tempCrops.Add(item.Value);
            }
            return tempCrops;
        }
    }
}

[System.Serializable]
public class Crops
{
    //     0       1           2 
    //outputName,outputId,coinCost

    public int outputItemId;
    public int coinCost;

    public Crops(int outputItemId, int coinCost)
    {
        this.outputItemId = outputItemId;
        this.coinCost = coinCost;
    }
}