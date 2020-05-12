using System.Collections;
using UnityEngine;

public class SlimeMonster : MonsterBase
{
    [SerializeField] private float attackChargeDuration = 1.5f;
    [SerializeField] private float attackChargeDistance = 3f;
    [SerializeField] private float relaxDuration = 1f;

    protected override void Update()
    {
        base.Update();
        if (isPlayerDetected && !isPreparingToAttack)
        {
            if (IsInAttackZone())
            {
                isPreparingToAttack = true;
                StartCoroutine(AttackPlayer());
                navMeshAgent.SetDestination(transform.position);
            }
            else
            {
                navMeshAgent.SetDestination(playerTransform.position);
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
        float tempKnockBackDistance = knockBackDistance;
        if (IsInAttackZone()) //Check if still player in attack zone
        {
            knockBackDistance = 0;
            Vector3 lastPlayerPosition = playerTransform.position;
            KnockFront(attackChargeDistance, lastPlayerPosition, 0.5f);
        }
        yield return new WaitForSeconds(relaxDuration);
        knockBackDistance = tempKnockBackDistance;
        isPreparingToAttack = false;
    }
}