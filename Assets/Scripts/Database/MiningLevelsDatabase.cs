using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

namespace MiningTown.IO
{
    public static class MiningLevelsDatabase
    {
        private static Dictionary<int, MiningLevel> miningLevels = new Dictionary<int, MiningLevel>();
        private const string fileName = "MiningLevels";
        private static int[] mineralsDictList = new int[] { 6, 9, 12, 15, 18, 21, 24 };
        private static int[] monsterssDictList = new int[] { 28, 31, 34, 37, 40, 43, 46 };

        public static void LoadItemDatabase()
        {
            List<string> linesList = fileName.ToCsvLines();
            for (int i = 0; i < linesList.Count; i++)
            {
                string[] chars = Regex.Split(linesList[i], ",");
                if (!miningLevels.ContainsKey(chars[0].ToInt()))
                {
                    miningLevels.Add(chars[0].ToInt(),
                        new MiningLevel(chars[0].ToInt(), chars[1], chars[2].ToFloat(), chars[3].ToFloat(),
                        chars[4].ToFloat(), GetMineralSpawnDictByChar(chars, mineralsDictList),
                        chars[26].ToFloat(), GetMineralSpawnDictByChar(chars, monsterssDictList)));
                }
            }
            //foreach (KeyValuePair<int, MiningLevel> item in miningLevels)
            //{
            //    Debug.Log(item.Value.monsterSpawnDict.Count);
            //}
        }

        private static Dictionary<int, float> GetMineralSpawnDictByChar(string[] chars, int[] indexes)
        {
            Dictionary<int, float> tempSpawnDict = new Dictionary<int, float>();
            for (int i = 0; i < indexes.Length; i++)
            {
                if (chars[indexes[i]].ToInt() != 0)
                {
                    tempSpawnDict.Add(chars[indexes[i]].ToInt(), chars[indexes[i] + 1].ToFloat());
                }
                else
                {
                    break;
                }
            }
            return tempSpawnDict;
        }

        public static MiningLevel GetMiningLevelById(int id)
        {
            return miningLevels[id];
        }
    }
}

[System.Serializable]
public class MiningLevel
{
    // 0        1           2           3    
    //levelId,levelMapName,laderMin,laderMax,
    //  4         5    6   7     8     9   10
    //totalProb,name1,id1,prob1,name2,id2,prob2,name3,id3,prob3,name4,id4,prob4,name5,id5,prob5,name6,id6,prob6,name7,id7,prob7,
    //  26              27    28   29   30    31
    //totalMonsterProb,name1,id1,prob1,name2,id2,prob2,name3,id3,prob3,name4,id4,prob4,name5,id5,prob5,name6,id6,prob6,name7,id7,prob7

    public int levelId;
    public string levelMapName;
    public float ladderSpawnMin;
    public float ladderSpawnMax;
    public float totalMineralSpawnProbability;
    public Dictionary<int, float> mineralSpawnDict;
    public float totalMonsterSpawnProbability;
    public Dictionary<int, float> monsterSpawnDict;

    public MiningLevel(int levelId, string levelMapName, float ladderSpawnMin, float ladderSpawnMax,
        float totalMineralSpawnProbability, Dictionary<int, float> mineralSpawnDict,
        float totalMonsterSpawnProbability, Dictionary<int, float> monsterSpawnDict)
    {
        this.levelId = levelId;
        this.levelMapName = levelMapName;
        this.ladderSpawnMin = ladderSpawnMin;
        this.ladderSpawnMax = ladderSpawnMax;

        this.totalMineralSpawnProbability = totalMineralSpawnProbability;
        this.mineralSpawnDict = mineralSpawnDict;

        this.totalMonsterSpawnProbability = totalMonsterSpawnProbability;
        this.monsterSpawnDict = monsterSpawnDict;
    }
}