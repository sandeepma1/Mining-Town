using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MiningTown.IO
{
    public class LevelUpDatabase : DatabaseBaseSingleton<LevelUpDatabase>
    {
        private static Level[] gameLevels;
        private const string fileName = "LevelUp";

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void Initialize()
        {
            List<string> linesList = GetAllLinesFromCSV(fileName);
            gameLevels = new Level[linesList.Count];
            //for (int i = 0; i < gameLevels.Length; i++)
            //{
            //    string[] chars = Regex.Split(linesList[i], ",");
            //    gameLevels[i] = new Level(IntParse(chars[0]), IntParse(chars[1]), IntParse(chars[2]),
            //        IntParse(chars[3]), IntParse(chars[4]), IntParse(chars[5]), IntParse(chars[6]));
            //}
        }

        public static Level GetLevelById(int Id)
        {
            return gameLevels[Id];
        }
    }
}

[System.Serializable]
public class Level
{
    public int levelID;
    public int XPforNextLevel;
    public int fieldRewardCount;
    public int itemUnlockID;
    public int itemRewardCount;
    public int buildingUnlockID;
    public int gemsRewardCount;
    /*public int coinsRewardCount;
    public int animalsUnlockID;
    public int animalsRewardCount;
    public int productsUnlockID;
    public int productsRewardCount;
    public int productionBuildingUnlockID;
    public int productionBuildingRewardCount;
    public int decorID;*/

    public Level(int id, int xp, int field, int itemID, int buildingID, int itemCount, int gem)
    {
        levelID = id;
        XPforNextLevel = xp;
        fieldRewardCount = field;
        itemUnlockID = itemID;
        itemRewardCount = itemCount;
        buildingUnlockID = buildingID;
        gemsRewardCount = gem;
    }
}
