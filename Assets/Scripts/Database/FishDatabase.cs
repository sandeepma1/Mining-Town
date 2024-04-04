using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

namespace MiningTown.IO
{
    public static class FishDatabase
    {
        private static Dictionary<int, FishObject> fishObjects = new Dictionary<int, FishObject>();
        private const string fileName = "Fishing";

        public static void LoadItemDatabase()
        {
            List<string> linesList = fileName.ToCsvLines();
            for (int i = 0; i < linesList.Count; i++)
            {
                string[] chars = Regex.Split(linesList[i], ",");
                if (!fishObjects.ContainsKey(chars[0].ToInt()))
                {
                    fishObjects.Add(chars[0].ToInt(),
                        new FishObject(
                        chars[0].ToInt(),
                        chars[1],
                        chars[2].ToInt(),
                        chars[3].ToInt(),
                        chars[4].ToInt(),
                        chars[5].ToInt(),
                        chars[6].ToInt(),
                        chars[7].ToFloat(),
                        chars[8].ToFloat(),
                        chars[9]));
                }
            }
            //Debug.Log(fishObjects.Count);
        }

        public static List<FishObject> GetFishesByLocationId(int locationId)
        {
            List<FishObject> fishes = new List<FishObject>();
            foreach (KeyValuePair<int, FishObject> fish in fishObjects)
            {
                if (fish.Value.locationId == locationId)
                {
                    fishes.Add(fish.Value);
                }
            }
            return fishes;
        }

        public static FishObject GetFishObjectById(int id)
        {
            return fishObjects[id];
        }

        public static string GetFishObjectSlugById(int id)
        {
            return fishObjects[id].slug;
        }
    }
}

[System.Serializable]
public class FishObject
{
    // 0     1          2             3         4           5         6             7      8   9           
    //fishId,name,rodLevel,minLengthInCm,maxLengthInCm,locationId,strugglePower,minWait,maxWait,slug

    public int fishId;
    public string name;
    public int rodLevel;
    public int minLengthInCm;
    public int maxLengthInCm;
    public int locationId;
    public int strugglePower;
    public float minBaitWait;
    public float maxBaitWait;
    public string slug;

    public FishObject(int fishId, string name, int rodLevel, int minLengthInCm, int maxLengthInCm,
        int locationId, int strugglePower, float minBaitWait, float maxBaitWait, string slug)
    {
        this.fishId = fishId;
        this.name = name;
        this.rodLevel = rodLevel;
        this.minLengthInCm = minLengthInCm;
        this.maxLengthInCm = maxLengthInCm;
        this.locationId = locationId;
        this.strugglePower = strugglePower;
        this.minBaitWait = minBaitWait;
        this.maxBaitWait = maxBaitWait;
        this.slug = slug;
    }
}