using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using MiningTown.IO;

public class MiningLevelGenerator : MonoBehaviour
{
    public static Action OnBakeNavMesh;
    public static Action OnLevelGenerated;
    public static Action<int, int> OnLevelSizeLoaded;
    [SerializeField] private bool forceLevelJump;
    [SerializeField] private int levelId;
    [SerializeField] private Transform obstaclesParent;
    [SerializeField] private Transform groundTilesParent;
    [SerializeField] private TrapsManager trapsManager;
    [SerializeField] private BreakableManager breakableManager;
    [SerializeField] private MonsterManager monsterManager;
    private TextAsset csv;
    private string[,] levelData;
    private int width, height;
    private NavMeshSurface navMeshSurface;
    private List<Vector2Int> randomBreakablesSpace = new List<Vector2Int>();
    public MiningLevel miningLevel;

    private void Start()
    {
        OnBakeNavMesh += BakeNavMesh;
        navMeshSurface = GetComponent<NavMeshSurface>();
        LoadLevel();
        LoadCsvLevelMapData();
        CreateLevelObjects();
        CreateMineralItems();
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
            miningLevel = MiningLevelsDatabase.GetMiningLevelById(levelId);
        }
        else
        {
            miningLevel = MiningLevelsDatabase.GetMiningLevelById(SaveLoadManager.saveData.playerStats.currentMinesLevel);
        }
    }

    private void LoadCsvLevelMapData()
    {
        string path = Path.Combine(GameVariables.path_minesLevelsCsv, miningLevel.levelMapName);
        csv = (TextAsset)Resources.Load(path, typeof(TextAsset));
        levelData = CSVReader.SplitCsvGrid(csv.text);
        width = levelData.GetLength(0);
        height = levelData.GetLength(1);
        OnLevelSizeLoaded?.Invoke(width, height);
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
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                InitLevelObjects(x, y);
            }
        }
    }

    private void InitLevelObjects(int x, int y)
    {

        string blockName = levelData[x, y];
        if (String.IsNullOrEmpty(blockName))
        {
            return;
        }
        switch (blockName[0])
        {
            case '-':
                CreateGroundTile(x, y);
                break;
            case '*':
                PlayerMovement.Instance.transform.position = new Vector3(x, 0, y);
                CreateGroundTile(x, y);
                break;
            case 'O':
                CreateObstacle(x, y);
                break;
            case 'B':
                randomBreakablesSpace.Add(new Vector2Int(x, y));
                CreateGroundTile(x, y);
                break;
            default:
                break;
        }

    }

    private void CreateObstacle(int x, int y)
    {
        GameObject obs = Instantiate(Resources.Load(GameVariables.path_minesObstacles + "O1") as GameObject, obstaclesParent);
        obs.transform.localPosition = new Vector3(x, 0, y);
        obs.transform.GetChild(0).localEulerAngles = new Vector3(0, UnityEngine.Random.Range(0, 360), 0);
        CreateGroundTile(x, y);
    }

    private void CreateGroundTile(int x, int y)
    {
        int random = UnityEngine.Random.Range(1, 7);
        int randomRot = UnityEngine.Random.Range(1, 5);
        GameObject ground = Instantiate(Resources.Load(GameVariables.path_minesGroundTiles + "G" + random.ToString()) as GameObject, groundTilesParent);
        ground.transform.localPosition = new Vector3(x, 0, y);
        ground.transform.localEulerAngles = new Vector3(90, 0, randomRot * 90);
    }

    /// <summary>
    /// This function created breakables, monsters, etc randomly in the given B1 position
    /// </summary>
    private void CreateMineralItems()
    {
        List<int> mineralIds = new List<int>();
        List<int> mineralCount = new List<int>();
        int totalBreakables = 0;
        foreach (KeyValuePair<int, float> mineral in miningLevel.mineralSpawnDict)
        {
            mineralIds.Add(mineral.Key);
            int totalNos = (int)(randomBreakablesSpace.Count * mineral.Value);
            mineralCount.Add(totalNos);
            totalBreakables += totalNos;
        }
        List<Vector2Int> ranList = randomBreakablesSpace.GetRandomElements(totalBreakables);
        ranList.RandomShuffle();


        List<int> mineralIdList = new List<int>();
        for (int i = 0; i < mineralIds.Count; i++)
        {
            for (int j = 0; j < mineralCount[i]; j++)
            {
                mineralIdList.Add(mineralIds[i]);
            }
        }
        mineralIdList.RandomShuffle();


        for (int i = 0; i < mineralIdList.Count; i++)
        {
            BreakableBase breakable;
            string prefabName = MineralsDatabase.GetMineralSlugById(mineralIdList[i]);
            breakable = Instantiate(Resources.Load<BreakableBase>(GameVariables.path_minesBreakables + prefabName), breakableManager.transform);
            breakable.Init(MineralsDatabase.GetMineralById(mineralIdList[i]), ranList[i]);
            breakableManager.AddBreakable(breakable);
        }
        breakableManager.CalculaterLadderSpawn(miningLevel.ladderSpawnMin, miningLevel.ladderSpawnMax);
    }

    private void CreateMonstersItems()
    {
        List<int> monsterIds = new List<int>();
        List<int> monsterCount = new List<int>();
        int totalMonsterBreakables = 0;

        foreach (KeyValuePair<int, float> monsters in miningLevel.monsterSpawnDict)
        {
            monsterIds.Add(monsters.Key);
            int totalNos = (int)(randomBreakablesSpace.Count * monsters.Value);
            monsterCount.Add(totalNos);
            totalMonsterBreakables += totalNos;
        }
        List<Vector2Int> ranList = randomBreakablesSpace.GetRandomElements(totalMonsterBreakables);
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
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }
    #endregion
}