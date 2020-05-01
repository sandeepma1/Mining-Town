using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager Instance = null;
    [SerializeField] private MonsterBase smallMonsterPrefab;
    [SerializeField] private Transform monsterSelectionRing;
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
            InstantiateMonster(spawnPositios[i]);
        }
    }

    //Temp Function
    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            InstantiateMonster(spawnPositios[0]);
        }
    }

    private void InstantiateMonster(Vector3 position)
    {
        MonsterBase smallMonster = Instantiate(smallMonsterPrefab, position, Quaternion.identity, transform);
        smallMonsters.Add(smallMonster);
    }

    public void MonsterDied(MonsterBase smallMonster)
    {
        SetMonsterSelectionRing(null);
        smallMonsters.Remove(smallMonster);
    }

    #region Helper Functions
    public MonsterBase GetNearestMonsterFromPosition(Vector3 currentPos)
    {
        MonsterBase nearestMonster = null;
        float minDist = Mathf.Infinity;
        for (int i = 0; i < smallMonsters.Count; i++)
        {
            float dist = Vector3.Distance(smallMonsters[i].transform.position, currentPos);
            if (dist < minDist)
            {
                nearestMonster = smallMonsters[i];
                minDist = dist;
            }
        }
        SetMonsterSelectionRing(nearestMonster);
        return nearestMonster;
    }

    private void SetMonsterSelectionRing(MonsterBase nearestMonster)
    {
        if (nearestMonster == null)
        {
            monsterSelectionRing.SetParent(null);
            monsterSelectionRing.gameObject.SetActive(false);
        }
        else
        {
            monsterSelectionRing.SetParent(nearestMonster.transform);
            monsterSelectionRing.gameObject.SetActive(true);
            monsterSelectionRing.localPosition = Vector3.zero;
        }
    }
    #endregion
}