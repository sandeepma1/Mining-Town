using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

namespace MiningTown.IO
{
    public static class ForestLevelsDatabase
    {
        private static Dictionary<int, ForestLevel> forestLevels = new Dictionary<int, ForestLevel>();
        private const string fileName = "ForestLevels";
        private static int[] forestDictList = new int[] { 4, 7, 10, 13, 16, 19, 22 };
        private static int[] monsterssDictList = new int[] { 26, 29, 32, 35, 38, 41, 44 };

        public static void LoadItemDatabase()
        {
            List<string> linesList = fileName.ToCsvLines();
            for (int i = 0; i < linesList.Count; i++)
            {
                string[] chars = Regex.Split(linesList[i], ",");
                if (!forestLevels.ContainsKey(chars[0].ToInt()))
                {
                    forestLevels.Add(chars[0].ToInt(),
                        new ForestLevel(chars[0].ToInt(), chars[1],
                        chars[2].ToFloat(), GetForestSpawnDictByChar(chars, forestDictList),
                        chars[24].ToFloat(), GetForestSpawnDictByChar(chars, monsterssDictList)));
                }
            }
            //foreach (KeyValuePair<int, MiningLevel> item in miningLevels)
            //{
            //    Debug.Log(item.Value.monsterSpawnDict.Count);
            //}
        }

        private static Dictionary<int, float> GetForestSpawnDictByChar(string[] chars, int[] indexes)
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

        public static ForestLevel GetForestLevelById(int id)
        {
            return forestLevels[id];
        }
    }
}

[System.Serializable]
public class ForestLevel
{
    // 0        1           
    //levelId,levelMapName,
    //  2         3    4   5     6    7   8
    //totalProb,name1,id1,prob1,name2,id2,prob2,name3,id3,prob3,name4,id4,prob4,name5,id5,prob5,name6,id6,prob6,name7,id7,prob7,
    //  24              25   26   27   
    //totalMonsterProb,name1,id1,prob1,name2,id2,prob2,name3,id3,prob3,name4,id4,prob4,name5,id5,prob5,name6,id6,prob6,name7,id7,prob7

    public int levelId;
    public string levelMapName;
    public float totalForestObjectsSpawnProbability;
    public Dictionary<int, float> forestObjectsSpawnDict;
    public float totalMonsterSpawnProbability;
    public Dictionary<int, float> monsterSpawnDict;

    public ForestLevel(int levelId, string levelMapName,
        float totalForestObjectsSpawnProbability, Dictionary<int, float> forestObjectsSpawnDict,
        float totalMonsterSpawnProbability, Dictionary<int, float> monsterSpawnDict)
    {
        this.levelId = levelId;
        this.levelMapName = levelMapName;

        this.totalForestObjectsSpawnProbability = totalForestObjectsSpawnProbability;
        this.forestObjectsSpawnDict = forestObjectsSpawnDict;

        this.totalMonsterSpawnProbability = totalMonsterSpawnProbability;
        this.monsterSpawnDict = monsterSpawnDict;
    }
}