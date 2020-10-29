using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

namespace MiningTown.IO
{
    public static class ForestObjectsDatabase
    {
        private static Dictionary<int, ForestObject> forestObjects = new Dictionary<int, ForestObject>();
        private const string fileName = "ForestObjects";

        public static void LoadItemDatabase()
        {
            List<string> linesList = fileName.ToCsvLines();
            for (int i = 0; i < linesList.Count; i++)
            {
                string[] chars = Regex.Split(linesList[i], ",");
                if (!forestObjects.ContainsKey(chars[0].ToInt()))
                {
                    forestObjects.Add(chars[0].ToInt(),
                        new ForestObject(chars[0].ToInt(),
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

        public static ForestObject GetForestObjectById(int id)
        {
            return forestObjects[id];
        }

        public static string GetForestObjectSlugById(int id)
        {
            return forestObjects[id].slug;
        }
    }
}

[System.Serializable]
public class ForestObject
{
    // 0     1          2             3                 4       5         6       7          8               
    //id,name,rodLevel,minLengthInCm,maxLengthInCm,locationId,strugglePower,minWait,maxWait,slug

    public int id;
    public string name;
    public int outputItemId;
    public int minDrop;
    public int maxDrop;
    public int hitPoints;
    public int xpOnDrop;
    public string slug;

    public ForestObject(int id, string name, int outputItemId, int minDrop, int maxDrop, int hitPoints, int xpOnDrop, string slug)
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