using System.Collections;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CapsuleCollider))]
public class SlimeMonster : MonsterBase
{
    [SerializeField] private Renderer slimeRenderer;
    [SerializeField] private float attackChargeDuration = 1.5f;
    [SerializeField] private float attackAnimationDuration = 0.45f;

    protected override void LateUpdate()
    {
        if (GameEvents.IsGamePaused() || isDead)
        {
            return;
        }
        base.LateUpdate();

        if (isPlayerDetected && IsInViewZone())
        {
            AttackPlayerAfterCharge();
        }
        else
        {
            //slimeRenderer.material.color = Color.white;
        }
    }

    private void AttackPlayerAfterCharge()
    {
        //Play charge animations
        LookAtPlayer();
        //slimeRenderer.material.color = Color.red;
        navMeshAgent.SetDestination(playerTransform.position);
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackChargeDuration) //Charge done
        {
            attackTimer = 0;
            if (IsInAttackZone()) //Check if still player in attack zone
            {
                navMeshAgent.SetDestination(transform.position);
                PlayAttackAnimation();
                Invoke("HitPlayerAfterAnimation", attackAnimationDuration);
            }
        }
    }

    //Used by Invoke
    private void HitPlayerAfterAnimation()
    {
        print("HitPlayerAfterAnimation");
        PlayerCurrencyManager.ReduceHealth((int)monster.damage);
    }
}