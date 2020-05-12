using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private List<MonsterBase> smallMonsters = new List<MonsterBase>();

    public void AddMonster(MonsterBase monster, int xPos, int yPos)
    {
        monster.transform.localPosition = new Vector3(xPos, 0, yPos);
        //ClosestObject.AddToRederer?.Invoke(monster.monsterMesh);
        smallMonsters.Add(monster);
        monster.OnMonsterDied += MonsterDied;
    }

    public void MonsterDied(MonsterBase smallMonster)
    {
        SelectionCircle.SetToThisParent?.Invoke(null);
        smallMonster.OnMonsterDied -= MonsterDied;
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
        return nearestMonster;
    }

    public Vector3? GetNearestMonsterPositionFromPosition(Vector3 currentPos)
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
        if (nearestMonster == null)
        {
            return null;
        }
        else
        {
            return nearestMonster.transform.position;
        }
    }

    private void SetMonsterSelectionRing(MonsterBase nearestMonster)
    {
        //if (nearestMonster == null)
        //{
        //    monsterSelectionRing.SetParent(null);
        //    monsterSelectionRing.gameObject.SetActive(false);
        //}
        //else
        //{
        //    monsterSelectionRing.SetParent(nearestMonster.transform);
        //    monsterSelectionRing.gameObject.SetActive(true);
        //    monsterSelectionRing.localPosition = Vector3.zero;
        //}
    }
    #endregion
}

public enum MonsterType
{
    FollowOnDetect,
    Shooting,
    Patrol
}