using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private List<MonsterBase> monsters = new List<MonsterBase>();

    public void AddMonster(Monster monster, MonsterBase monsterBase, int xPos, int yPos)
    {
        monsterBase.SetMonsterWarpPosition(new Vector3(xPos, 0, yPos));
        monsterBase.InitMonster(monster);
        monsters.Add(monsterBase);
        monsterBase.OnMonsterDied += MonsterDied;
    }

    public void MonsterDied(MonsterBase monsterBase)
    {
        SelectionCircle.OnSetToThisParent?.Invoke(null);
        monsterBase.OnMonsterDied -= MonsterDied;
        monsters.Remove(monsterBase);
    }
}