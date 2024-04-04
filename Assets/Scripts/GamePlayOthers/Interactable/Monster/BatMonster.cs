using System.Collections;
using UnityEngine;

public class BatMonster : MonsterBase
{
    [SerializeField] private float attackChargeDuration = 1.5f;
    [SerializeField] private float knockFrontDistance = 3;
    [SerializeField] private float relaxDuration = 1f;

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (isPlayerDetected && !isPreparingToAttack)
        {
            if (IsInAttackZone())
            {
                isPreparingToAttack = true;
                StartCoroutine(AttackPlayer());
            }
        }
        if (isPreparingToAttack)
        {
            LookAtPlayer();
        }
    }

    private IEnumerator AttackPlayer()
    {
        //Play charge animations
        yield return new WaitForSeconds(attackChargeDuration);
        print("AttackPlayer");
        //Play Attack animations
        float tempKnockBackDistance = monster.knockBackDistance;
        if (IsInAttackZone()) //Check if still player in attack zone
        {
            monster.knockBackDistance = 0;
            Vector3 lastPlayerPosition = playerTransform.position;
            KnockFront(knockFrontDistance, lastPlayerPosition, 0.5f);
        }
        yield return new WaitForSeconds(relaxDuration);
        monster.knockBackDistance = tempKnockBackDistance;
        isPreparingToAttack = false;
    }
}