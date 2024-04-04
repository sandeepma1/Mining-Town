using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

namespace MiningTown.IO
{
    public static class MineralsDatabase
    {
        private static Dictionary<int, Mineral> minerals = new Dictionary<int, Mineral>();
        private const string fileName = "Minerals";

        public static void LoadItemDatabase()
        {
            List<string> linesList = fileName.ToCsvLines();
            for (int i = 0; i < linesList.Count; i++)
            {
                string[] chars = Regex.Split(linesList[i], ",");
                if (!minerals.ContainsKey(chars[0].ToInt()))
                {
                    minerals.Add(chars[0].ToInt(),
                        new Mineral(chars[0].ToInt(),
                        chars[1],
                        chars[3].ToInt(),
                        chars[4].ToInt(),
                        chars[5].ToInt(),
                        chars[6].ToInt(),
                        chars[7].ToInt(),
                        chars[8]));
                }
            }
        }

        public static Mineral GetMineralById(int id)
        {
            return minerals[id];
        }

        public static string GetMineralSlugById(int id)
        {
            return minerals[id].slug;
        }
    }
}

[System.Serializable]
public class Mineral
{
    // 0     1          2             3                 4       5         6       7          8               
    // id	name	outputItemName	outputItemId	minDrop	maxDrop	hitPoints	xpOnDrop	slug

    public int id;
    public string name;
    public int outputItemId;
    public int minDrop;
    public int maxDrop;
    public int hitPoints;
    public int xpOnDrop;
    public string slug;

    public Mineral(int id, string name, int outputItemId, int minDrop, int maxDrop, int hitPoints, int xpOnDrop, string slug)
    {
        this.id = id;
        this.name = name;
        this.outputItemId = outputItemId;
        this.minDrop = minDrop;
        this.maxDrop = maxDrop;
        this.hitPoints = hitPoints;
        this.xpOnDrop = xpOnDrop;
        this.slug = slug;
    }
}