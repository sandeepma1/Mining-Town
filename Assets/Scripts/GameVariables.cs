using UnityEngine;

public static class GameVariables
{
    //public static string pp_currentLevelNumber = "currentLevelNumber";
    //public static string pp_currentPlayerHealth = "currentPlayerHealth";

    public static Vector3 housePosition = new Vector3(-30, 0, 5);
    public static string tag_ladder = "Ladder";
    public static string tag_forestExit = "ForestExit";
    public static string tag_water = "Water";
    public static string tag_player = "Player";

    #region Resources Path
    public static string path_cropPrefabPath = "FarmHome/Crop";
    public static string path_sourcePrefabFolder = "FarmHome/Source/";
    public static string path_decorationPrefabFolder = "FarmHome/Decoration/";
    //All Forest path   
    public static string path_forestLevelsCsv = "Forest/LevelsCsv/";
    public static string path_forestObstacles = "Forest/LevelObjects/Obstacles/";
    public static string path_forestGroundTiles = "Forest/LevelObjects/GroundTiles/";
    public static string path_forestChopable = "Forest/LevelObjects/Chopable/";
    //All Mines path   
    public static string path_minesLevelsCsv = "Mines/LevelsCsv/";
    public static string path_minesObstacles = "Mines/LevelObjects/Obstacles/";
    public static string path_minesGroundTiles = "Mines/LevelObjects/GroundTiles/";
    public static string path_minesBreakables = "Mines/LevelObjects/Breakables/";
    //Common Monsters, traps, grass path
    public static string path_traps = "Common/Traps/";
    public static string path_monsters = "Common/Monsters/";
    public static string path_grassBlocks = "Common/GrassBlocks/";
    public static string path_grass = "Common/Grass/";
    #endregion


    //TMP sprite icons
    public static string tmp_coinIcon = "<sprite name=\"coin\">";
    public static string tmp_invIcon = "<sprite name=\"inv\">";
    public static string tmp_gemIcon = "<sprite name=\"gem\">";
    public static string tmp_xpIcon = "<sprite name=\"xp\">";
    public static string tmp_energyIcon = "<sprite name=\"energy\">";
    public static string tmp_timeIcon = "<sprite name=\"time\">";
    public static string tmp_heartIcon = "<sprite name=\"heart\">";

    //UI Messages
    public static string msg_lowOnCoins = "You don't have enough coins, buy some coins to proceed";
    public static string msg_lowOnGems = "You don't have enough gems, buy some gems to proceed";
    public static string msg_lowOnEnergy = "You don't have enough energy, buy some energy to proceed";
    public static string msg_lowOnBackpackSpace = "You backpack is full, buy new slots or empty some items";
    public static string msg_gotoNextMinesLevel = "Decent to next level?";
    public static string msg_gotoNextForestLevel = "Do you want to go deep inside the forest?";
    public static string msg_gotoHomeLevel = "Do you want to go home?";
    public static string msg_resetGame = "This will reset all the game progress, are you sure you want to reset the game data?\n\n You cannot undo this action";

    //Ui Buttons and small text
    public static string smsg_cannotConsume = "Cannot consume";
    public static string smsg_addToBackpack = "Add to backpack";
    public static string smsg_addToBarn = "Add to barn";
    public static string smsg_destroyItems = "Destroy items";

    //UI variables timers, etc
    public static float touchHoldDescDuration = 0.2f;
    public static int maxQueueBox = 8;
    public static int maxLivestock = 5;
    public static float droppedItemEnableTimer = 0.5f;

    //Edit mode Screen    
    public static float edgeThreshold = 100;
    public static Rect edgeScreenRect = new Rect(edgeThreshold, edgeThreshold, Screen.width - edgeThreshold * 2, Screen.height - edgeThreshold * 2);

    public static LayerMask groundLayerMask = LayerMask.NameToLayer("Ground");
    public static float rayCastDistance = 500;

    //Weapons & Tools
    public static float axeHitRate = 0.6f;
    public static float pickaxeHitRate = 0.6f;
    public static float swordHitRate = 0.5f;
    public static float transistionDuration = 1f;

    //Gameplay variables
    public const float energyNewAddSeconds = 60;
    public const int bp_SlotsColumnCount = 6; //6 columns
    public const int bp_MaxSlots = 8 * bp_SlotsColumnCount; //6 columns * 8 rows
    public const int bp_newSlotRateMultiplier = 5;

    //Energy reductions
    public const int energy_raisedBed = 1;
    public const int energy_fishing = 5;
    public const int energy_pickaxe = 1;
    public const int energy_axe = 1;
}
public enum Scenes
{
    Loading,
    FarmHome,
    Mines,
    Forest,
    Town
}