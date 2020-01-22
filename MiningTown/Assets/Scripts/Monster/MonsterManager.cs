using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager Instance = null;
    [SerializeField] private MonsterBase smallMonsterPrefab;
    [SerializeField] private Vector3[] spawnPositios;
    private List<MonsterBase> smallMonsters = new List<MonsterBase>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < spawnPositios.Length; i++)
        {
            MonsterBase smallMonster = Instantiate(smallMonsterPrefab, spawnPositios[i], Quaternion.identity, transform);
            smallMonsters.Add(smallMonster);
        }
    }

    public List<MonsterBase> GetAllMonstersList()
    {
        return smallMonsters;
    }

    public void MonsterDied(MonsterBase smallMonster)
    {
        smallMonsters.Remove(smallMonster);
    }
}