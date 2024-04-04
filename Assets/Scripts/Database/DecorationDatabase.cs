using System.Text.RegularExpressions;
using System.Collections.Generic;
//using UnityEngine;

namespace MiningTown.IO
{
    public class DecorationDatabase
    {
        private static Dictionary<int, DecorationInfo> decoration = new Dictionary<int, DecorationInfo>();
        private const string fileName = "Decoration";

        public static void LoadDatabase()
        {
            List<string> linesList = fileName.ToCsvLines();
            for (int i = 0; i < linesList.Count; i++)
            {
                string[] chars = Regex.Split(linesList[i], ",");
                if (!decoration.ContainsKey(chars[0].ToInt()))
                {
                    decoration.Add(chars[0].ToInt(),
                        new DecorationInfo(chars[0].ToInt(), chars[1], chars[2].ToInt(), chars[3].ToInt(), chars[4]));
                }
            }
            //Debug.Log("Decoration loaded " + decoration.Count);
        }

        public static DecorationInfo GetDecorationInfoById(int itemId)
        {
            return decoration[itemId];
        }

        public static Dictionary<int, DecorationInfo> GetAllDecorations()
        {
            return decoration;
        }

        public static string GetDecorationNameById(int itemId)
        {
            return decoration[itemId].name;
        }

        public static string GetDecorationSlugById(int itemId)
        {
            return decoration[itemId].slug;
        }

        public static int GetDecorationInfolength()
        {
            return decoration.Count;
        }
    }
}

[System.Serializable]
public class DecorationInfo
{
    //id	name	buildCostGems	buildCostCoins	slug
    //0      1       2                  3           4 
    public int id;
    public string name;
    public int buildCostGems;
    public int buildCostCoins;
    public string slug;

    public DecorationInfo(int id, string name, int buildCostGems, int buildCostCoins, string slug)
    {
        this.id = id;
        this.name = name;
        this.buildCostGems = buildCostGems;
        this.buildCostCoins = buildCostCoins;
        this.slug = slug;
    }
}