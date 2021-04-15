using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using MiningTown.IO;
using DG.Tweening;

public class ForestLevelGenerator : MonoBehaviour
{
    public static Action OnBakeNavMesh;
    public static Action OnLevelGenerated;
    public static Action<int, int> OnLevelSizeLoaded;
    [SerializeField] private bool forceLevelJump;
    [SerializeField] private int levelId;
    [SerializeField] private Transform obstaclesParent;
    [SerializeField] private Transform grassBlocksParent;
    //[SerializeField] private TrapsManager trapsManager;
    //[SerializeField] private BreakableManager breakableManager;
    [SerializeField] private ChopableManager chopableManager;
    [SerializeField] private MonsterManager monsterManager;
    private TextAsset csv;
    private string[,] levelData;
    private int width, height;
    private int spaces = 5;
    private int totalWidth;
    private int totalHeight;
    private NavMeshSurface navMeshSurface;
    private List<Vector2Int> chopablesSpaces = new List<Vector2Int>();
    public ForestLevel forestLevel;
    public Transform[,] grasses;// = new Transform[]

    private void Start()
    {
        OnBakeNavMesh += BakeNavMesh;
        navMeshSurface = GetComponent<NavMeshSurface>();
        LoadLevel();
        ReadCsvMapData();
        CreateLevelObjects();
        CreateForestObjects();
        CreateMonstersItems();
        BakeNavMesh();
        OnLevelGenerated?.Invoke();
        //GameEvents.ResumeGame();
    }

    private void OnDestroy()
    {
        OnBakeNavMesh -= BakeNavMesh;
    }

    private void LoadLevel()
    {
        if (forceLevelJump && Application.isEditor)
        {
            forestLevel = ForestLevelsDatabase.GetForestLevelById(levelId);
        }
        else
        {
            forestLevel = ForestLevelsDatabase.GetForestLevelById(SaveLoadManager.saveData.playerStats.currentForestLevel);
        }
    }

    private void ReadCsvMapData()
    {
        string path = Path.Combine(GameVariables.path_forestLevelsCsv, forestLevel.levelMapName);
        csv = (TextAsset)Resources.Load(path, typeof(TextAsset));
        levelData = CSVReader.ReadCsv(csv.text);
        width = levelData.GetLength(0);
        height = levelData.GetLength(1);

        totalWidth = width * spaces;
        totalHeight = height * spaces;
        grasses = new Transform[totalWidth, totalHeight];
        // OnLevelSizeLoaded?.Invoke(totalWidth, totalHeight);
        FlipLevelVertically();
    }

    private void FlipLevelVertically()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height / 2; y++)
            {
                string tmp = levelData[x, height - y - 1];
                levelData[x, height - y - 1] = (levelData[x, y]);
                levelData[x, y] = tmp;
            }
        }
    }

    private void CreateLevelObjects()
    {
        for (int x = 0; x < totalWidth; x += spaces)
        {
            for (int y = 0; y < totalHeight; y += spaces)
            {
                InitLevelObjects(x, y);
            }
        }
    }

    private void InitLevelObjects(int x, int y)
    {
        string blockName = levelData[x / spaces, y / spaces];
        if (String.IsNullOrEmpty(blockName))
        {
            return;
        }
        switch (blockName[0])
        {
            case '-':
                break;
            case '*':
                PlayerMovement.Instance.transform.position = new Vector3(x, 0, y);
                CreateGrassNavTile(x, y);
                break;
            case 'O':
                CreateObstacle(x, y);
                CreateGrassNonNavTile(x, y);
                break;
            case 'T':
                CreateBackgroundTrees(x, y);
                break;
            case 'C':
                chopablesSpaces.Add(new Vector2Int(x, y));
                break;
            case 'E': //Forest Exit
                CreateExitBlock(x, y);
                CreateGrassNonNavTile(x, y);
                break;
            case 'N': //Nothing, but create ground
                CreateGrassNavTile(x, y);
                break;
            default:
                break;
        }

    }

    private void CreateGrassNavTile(int x, int y)
    {
        GameObject ground = Instantiate(Resources.Load(GameVariables.path_grassBlocks + "GrassBlockNav") as GameObject, grassBlocksParent);
        ground.transform.localPosition = new Vector3(x, 0, y);
    }

    private void CreateGrassNavTreeBaseTile(int x, int y)
    {
        GameObject ground = Instantiate(Resources.Load(GameVariables.path_grassBlocks + "GrassBlockNavTree") as GameObject, grassBlocksParent);
        ground.transform.localPosition = new Vector3(x, 0, y);
    }

    private void CreateGrassNonNavTile(int x, int y)
    {
        GameObject ground = Instantiate(Resources.Load(GameVariables.path_grassBlocks + "GrassBlockNonNav") as GameObject, grassBlocksParent);
        ground.transform.localPosition = new Vector3(x, 0, y);
    }

    private void CreateObstacle(int x, int y)
    {
        GameObject obs = Instantiate(Resources.Load(GameVariables.path_forestObstacles + "O1") as GameObject, obstaclesParent);
        obs.transform.localPosition = new Vector3(x, 0, y);
        obs.transform.GetChild(0).localEulerAngles = new Vector3(0, UnityEngine.Random.Range(-60, 60), 0);
    }

    private void CreateExitBlock(int x, int y)
    {
        GameObject forestExitBlock = Instantiate(Resources.Load(GameVariables.path_forestObstacles + "ForestExit") as GameObject, obstaclesParent);
        forestExitBlock.transform.localPosition = new Vector3(x, 0, y);
    }

    private void CreateBackgroundTrees(int x, int y)
    {
        GameObject obs = Instantiate(Resources.Load(GameVariables.path_forestObstacles + "T1") as GameObject, obstaclesParent);
        obs.transform.localPosition = new Vector3(x, 0, y);
        obs.transform.GetChild(0).localEulerAngles = new Vector3(0, UnityEngine.Random.Range(-60, 60), 0);
    }

    /// <summary>
    /// This function created chopables randomly in the given "C" position
    /// </summary>
    private void CreateForestObjects()
    {
        List<int> forestObjectIds = new List<int>();
        List<int> forestObjectsCount = new List<int>();
        int totalChopables = 0;
        foreach (KeyValuePair<int, float> forestObject in forestLevel.forestObjectsSpawnDict)
        {
            forestObjectIds.Add(forestObject.Key);
            int totalNos = (int)(chopablesSpaces.Count * forestObject.Value);
            forestObjectsCount.Add(totalNos);
            totalChopables += totalNos;
        }

        List<Vector2Int> randomChoppablesPositions = chopablesSpaces.GetRandomElements(totalChopables);

        for (int i = 0; i < chopablesSpaces.Count; i++)
        {
            if (!randomChoppablesPositions.Contains(chopablesSpaces[i]))
            {
                CreateGrassNavTile(chopablesSpaces[i].x, chopablesSpaces[i].y);
            }
        }

        randomChoppablesPositions.RandomShuffle();


        List<int> forestObjectsIdList = new List<int>();
        for (int i = 0; i < forestObjectIds.Count; i++)
        {
            for (int j = 0; j < forestObjectsCount[i]; j++)
            {
                forestObjectsIdList.Add(forestObjectIds[i]);
            }
        }
        forestObjectsIdList.RandomShuffle();

        for (int i = 0; i < forestObjectsIdList.Count; i++)
        {
            ChopableBase chopableBase;
            string prefabName = ForestObjectsDatabase.GetForestObjectSlugById(forestObjectsIdList[i]);
            chopableBase = Instantiate(Resources.Load<ChopableBase>(GameVariables.path_forestChopable + prefabName), chopableManager.transform);
            chopableBase.Init(ForestObjectsDatabase.GetForestObjectById(forestObjectsIdList[i]), randomChoppablesPositions[i]);
            CreateGrassNavTreeBaseTile(randomChoppablesPositions[i].x, randomChoppablesPositions[i].y);
            chopableManager.AddChopable(chopableBase);
        }
    }

    private void CreateMonstersItems()
    {
        List<int> monsterIds = new List<int>();
        List<int> monsterCount = new List<int>();
        int totalMonsterBreakables = 0;

        foreach (KeyValuePair<int, float> monsters in forestLevel.monsterSpawnDict)
        {
            monsterIds.Add(monsters.Key);
            int totalNos = (int)(chopablesSpaces.Count * monsters.Value);
            monsterCount.Add(totalNos);
            totalMonsterBreakables += totalNos;
        }
        List<Vector2Int> ranList = chopablesSpaces.GetRandomElements(totalMonsterBreakables);
        ranList.RandomShuffle();


        List<int> monsterIdList = new List<int>();
        for (int i = 0; i < monsterIds.Count; i++)
        {
            for (int j = 0; j < monsterCount[i]; j++)
            {
                monsterIdList.Add(monsterIds[i]);
            }
        }
        monsterIdList.RandomShuffle();


        for (int i = 0; i < monsterIdList.Count; i++)
        {
            MonsterBase monsterBase;
            Monster monster = MonsterDatabase.GetMonsterById(monsterIdList[i]);
            monsterBase = Instantiate(Resources.Load<MonsterBase>(GameVariables.path_monsters + monster.slug), monsterManager.transform);
            monsterManager.AddMonster(monster, monsterBase, ranList[i].x, ranList[i].y);
        }
        //monsterManager.OnAllBreakableDone();
    }


    #region NavMesh stuff
    private void BakeNavMesh()
    {
        StartCoroutine(BakeNavMeshAtEnd());
    }

    private IEnumerator BakeNavMeshAtEnd()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }
    #endregion
}