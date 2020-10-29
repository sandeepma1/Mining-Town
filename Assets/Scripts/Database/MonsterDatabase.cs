using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MiningTown.IO
{
    public static class MonsterDatabase
    {
        private static Dictionary<int, Monster> monsters = new Dictionary<int, Monster>();
        private const string fileName = "Monsters";

        public static void LoadItemDatabase()
        {
            List<string> linesList = fileName.ToCsvLines();
            for (int i = 0; i < linesList.Count; i++)
            {
                string[] chars = Regex.Split(linesList[i], ",");
                if (!monsters.ContainsKey(chars[0].ToInt()))
                {
                    // YourEnumType parsed_enum = (YourEnumType)System.Enum.Parse( typeof(YourEnumType), your_string );

                    monsters.Add(chars[0].ToInt(),
                        new Monster(chars[0].ToInt(),
                        chars[1],
                        chars[3].ToInt(),
                        chars[4].ToInt(),
                        chars[5].ToInt(),
                        (MonsterType)System.Enum.Parse(typeof(MonsterType), chars[6]),
                        (MonsterIdleState)System.Enum.Parse(typeof(MonsterIdleState), chars[7]),
                        chars[8].ToFloat(),
                        chars[9].ToFloat(),
                        chars[10].ToFloat(),
                        chars[11].ToFloat(),
                        chars[12].ToFloat(),
                        chars[13].ToFloat(),
                        chars[14].ToFloat(),
                        chars[15].ToFloat(),
                        chars[16].ToInt(),
                        chars[17].ToInt(),
                        chars[18]));
                }
            }
        }

        public static Monster GetMonsterById(int id)
        {
            return monsters[id];
        }

        public static string GetMonsterSlugById(int id)
        {
            return monsters[id].slug;
        }
    }
}

[System.Serializable]
public class Monster
{
    // 0   1        2             3       4        5      6       7      8            
    //id,name,outputOnKill,outputItemId,minDrop,maxDrop,type,idleState,wanderRepeatRate,
    //      9       10          11              12          13          14          15      16      17     19
    //wanderRadius,damage,knockBackDistance,viewRadius,attackRadius,alertSpeed,baseSpeed,health,xpOnKill,slug

    public int id;
    public string name;
    public int outputOnKill;
    public int minDrop;
    public int maxDrop;
    public MonsterType type;
    public MonsterIdleState idleState;
    public float wanderRepeatRate;
    public float wanderRadius;
    public float damage;
    public float knockBackDistance;
    public float viewRadius;
    public float attackRadius;
    public float alertSpeed;
    public float baseSpeed;
    public int health;
    public int xpOnKill;
    public string slug;

    public Monster(int id, string name, int outputOnKill, int minDrop, int maxDrop,
        MonsterType type, MonsterIdleState idleState, float wanderRepeatRate,
        float wanderRadius, float damage, float knockBackDistance, float viewRadius,
        float attackRadius, float alertSpeed, float baseSpeed, int health, int xpOnKill, string slug)
    {
        this.id = id;
        this.name = name;
        this.outputOnKill = outputOnKill;
        this.minDrop = minDrop;
        this.maxDrop = maxDrop;
        this.type = type;
        this.idleState = idleState;
        this.wanderRepeatRate = wanderRepeatRate;
        this.wanderRadius = wanderRadius;
        this.damage = damage;
        this.knockBackDistance = knockBackDistance;
        this.viewRadius = viewRadius;
        this.attackRadius = attackRadius;
        this.alertSpeed = alertSpeed;
        this.baseSpeed = baseSpeed;
        this.health = health;
        this.xpOnKill = xpOnKill;
        this.slug = slug;
    }
}


public enum MonsterType
{
    FollowAndKill,
    Shooting,
    Patrol,
    TouchAttack,
    ChargeAttack
}

public enum MonsterIdleState
{
    Still,
    Wander
}