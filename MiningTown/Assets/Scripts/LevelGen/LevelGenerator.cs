using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    public static Action OnBakeNavMesh;
    public static Action OnLevelGenerated;
    public static Action<int, int> OnLevelSizeLoaded;
    [SerializeField] private int levelId;
    [SerializeField] private Transform[] boundries; // ||_- left,right,bottom,top
    [SerializeField] private Transform ground;
    [SerializeField] private Transform obstaclesParent;
    [SerializeField] private BreakableManager breakableManager;
    [SerializeField] private MonsterManager monsterManager;
    private TextAsset csv;
    private string[,] levelData;
    private int width, height;
    private NavMeshSurface navMeshSurface;
    private const string csvPath = "LevelsCsv/";
    private const string obstaclesPath = "LevelObjects/Obstacles/";
    private const string monstersPath = "LevelObjects/Monsters/";
    private const string breakablesPath = "LevelObjects/Breakables/";

    private void Start()
    {
        OnBakeNavMesh += BakeNavMesh;
        navMeshSurface = GetComponent<NavMeshSurface>();
        LoadCsvLevelData();
        CreateBoundries();
        CreateLevelObjects();
        BakeNavMesh();
        OnLevelGenerated?.Invoke();
    }

    private void OnDestroy()
    {
        OnBakeNavMesh -= BakeNavMesh;
    }

    private void LoadCsvLevelData()
    {
        string path = Path.Combine(csvPath, levelId.ToString());
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

    private void CreateBoundries()
    {
        float zPos = (height / 2) - 0.5f;
        float xPos = (width / 2) - 0.5f;

        //left
        boundries[0].transform.localPosition = new Vector3(-1, 0, zPos);
        boundries[0].transform.localScale = new Vector3(1, 1, height);

        //right
        boundries[1].transform.localPosition = new Vector3(width, 0, zPos);
        boundries[1].transform.localScale = new Vector3(1, 1, height);

        //bottom
        boundries[2].transform.localPosition = new Vector3(xPos, 0, -1);
        boundries[2].transform.localScale = new Vector3(width + 2, 1, 1);

        //top      
        boundries[3].transform.localPosition = new Vector3(xPos, 0, height);
        boundries[3].transform.localScale = new Vector3(width + 2, 1, 1);

        //ground
        ground.transform.localPosition = new Vector3(xPos, -1, zPos);
        ground.transform.localScale = new Vector3(width + 2, 1, height + 2);

    }

    private void CreateLevelObjects()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                InitObstacles(x, y);
            }
        }
    }

    private void InitObstacles(int x, int y)
    {
        string blockName = levelData[x, y];
        switch (blockName[0])
        {
            case 'O':
                GameObject obs = Instantiate(Resources.Load(obstaclesPath + blockName) as GameObject, obstaclesParent);
                obs.transform.localPosition = new Vector3(x, 0, y);
                break;
            case 'M':
                MonsterBase monster = Instantiate(Resources.Load<MonsterBase>(monstersPath + blockName), monsterManager.transform);
                monsterManager.AddMonster(monster, x, y);
                break;
            case 'B':
                BreakableBase breakable = Instantiate(Resources.Load<BreakableBase>(breakablesPath + blockName), breakableManager.transform);
                breakableManager.AddBreakable(breakable, x, y);
                break;
            default:
                break;
        }
    }

    private void BakeNavMesh()
    {
        StartCoroutine(BakeNavMeshAtEnd());
    }

    private IEnumerator BakeNavMeshAtEnd()
    {
        yield return new WaitForEndOfFrame();
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }
}