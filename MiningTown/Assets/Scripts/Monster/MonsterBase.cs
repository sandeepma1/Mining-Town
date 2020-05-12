using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;
using UnityEditor;

public class MonsterBase : MonoBehaviour, IInteractable
{
    public Action<MonsterBase> OnMonsterDied;
    [SerializeField] protected MonsterType monsterType;
    [SerializeField] private bool doWander;
    [SerializeField] private float wanderRepeatRate = 3;
    [SerializeField] private float wanderRadius = 2;
    [SerializeField] private Transform uiHealthBarPosition;
    [SerializeField] private float maxHealth = 5;
    [SerializeField] protected float damage = 5;
    [SerializeField] protected float knockBackDistance = 1;
    [SerializeField] private float viewZone = 5;
    [Space(30)] [SerializeField] private float attackZone = 5;

    private float health;
    private UiHealthBar uiHealthBar;
    protected NavMeshAgent navMeshAgent;
    protected const float knockBackDuration = 0.1f;
    protected bool isPlayerDetected;
    protected bool isPreparingToAttack;
    protected Transform playerTransform;

    protected virtual void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        uiHealthBar = UiHealthBarCanvas.Instance.CreateHealthBar(uiHealthBarPosition);
        health = maxHealth;
        playerTransform = PlayerMovement.Instance.GetPlayerTransform();
        if (doWander)
        {
            InvokeRepeating("Wander", 1, wanderRepeatRate);
        }
    }

    protected virtual void Update()
    {
        if (monsterType == MonsterType.FollowOnDetect)
        {
            if (IsInViewZone())
            {
                isPlayerDetected = true;
            }
            else
            {
                isPlayerDetected = false;
            }
        }
    }


    #region Wander stuff
    private void Wander()
    {
        if (isPlayerDetected)
        {
            return;
        }
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * wanderRadius;
        randDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, wanderRadius, -1);
        navMeshAgent.SetDestination(new Vector3(navHit.position.x, 0, navHit.position.z));
    }
    #endregion


    #region Helper functions
    protected void LookAtPlayer()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation,
      Quaternion.LookRotation((playerTransform.position - transform.position).normalized),
      Time.deltaTime * 10);
    }

    protected bool IsInAttackZone()
    {
        return Vector3.Distance(playerTransform.position, transform.position) <= attackZone;
    }

    protected bool IsInViewZone()
    {
        return Vector3.Distance(playerTransform.position, transform.position) <= viewZone;
    }
    #endregion


    #region GetComponent Collision in other scripts TakeHit()
    public void TakeHit()
    {
        health--;
        if (health <= 0)
        {
            OnMonsterDied?.Invoke(this);
            Destroy(uiHealthBar.gameObject);
            Destroy(this.gameObject);
        }
        else
        {
            uiHealthBar.OnHealthChanged(health / maxHealth);
            Knockback();
        }
    }

    protected void Knockback()
    {
        Vector3 direction = (transform.position - playerTransform.position).normalized;
        transform.DOMove(transform.position + direction * knockBackDistance, knockBackDuration);
    }

    protected void KnockFront(float knockFrontDistance, Vector3 position, float knockBackSpeed)
    {
        Vector3 direction = (position - transform.position).normalized;
        transform.DOMove(position + direction * knockFrontDistance, knockBackSpeed);
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public float GetDamageValue()
    {
        return damage;
    }
    #endregion
}