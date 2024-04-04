using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;
using System.Collections;
using MiningTown.IO;

public class MonsterBase : MonoBehaviour, IMonster
{
    public Action<MonsterBase> OnMonsterDied;
    [Space(10)]
    [SerializeField] private Transform uiHealthBarPosition;
    [SerializeField] private bool hasRunAnimation;
    [Space(30)]
    private float health;
    private UiHealthBar uiHealthBar;
    protected NavMeshAgent navMeshAgent;
    protected const float knockBackDuration = 0.1f;
    protected bool isPlayerDetected;
    protected bool isPreparingToAttack;
    protected Transform playerTransform;
    private const float lookRotationSpeed = 10;
    protected Monster monster;
    private Animator animator;
    protected bool isDead;
    protected Vector3Int lastMonsterPosInt;
    protected float attackTimer = 0;
    private float wanderTimer = 0;

    internal void InitMonster(Monster monster)
    {
        //Start stuff
        GameEvents.OnPauseGame += OnPauseGame;
        GameEvents.OnResumeGame += OnResumeGame;
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = PlayerMovement.Instance.GetPlayerTransform();
        uiHealthBar = UiHealthBarCanvas.Instance.CreateHealthBar(uiHealthBarPosition);

        this.monster = monster;
        health = monster.health;
        navMeshAgent.speed = monster.baseSpeed;
    }

    protected virtual void LateUpdate()
    {
        if (GameEvents.IsGamePaused())
        {
            return;
        }
        if (monster.type == MonsterType.FollowAndKill)
        {
            if (IsInViewZone())
            {
                OnPlayerDetected(true);
            }
            else
            {
                OnPlayerDetected(false);
            }
        }
        switch (monster.idleState)
        {
            case MonsterIdleState.Still:
                break;
            case MonsterIdleState.Wander:
                Wander();
                IsMonsterMoving();
                break;
            default:
                break;
        }
        if (lastMonsterPosInt != Vector3Int.FloorToInt(transform.position))
        {
            lastMonsterPosInt = Vector3Int.FloorToInt(transform.position);
            GrassManager.OnStepOnGrass?.Invoke(lastMonsterPosInt);
        }
    }

    private void OnDestroy()
    {
        GameEvents.OnPauseGame -= OnPauseGame;
        GameEvents.OnResumeGame -= OnResumeGame;
    }

    private void OnResumeGame()
    {
        animator.speed = 1;
        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = false;
        }
    }

    private void OnPauseGame()
    {
        animator.speed = 0;
        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
        }
    }

    private void IsMonsterMoving()
    {
        if (!hasRunAnimation)
        {
            return;
        }
        if (!navMeshAgent.pathPending && navMeshAgent.isOnNavMesh)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    //Hud.SetHudText?.Invoke("not moving");
                    IsRunning(false);
                }
            }
            else
            {
                //Hud.SetHudText?.Invoke("moving");
                IsRunning(true);
            }
        }
    }

    public void SetMonsterWarpPosition(Vector3 pos)
    {
        StartCoroutine(WarpMonster(pos));
    }

    private IEnumerator WarpMonster(Vector3 pos)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !GameEvents.IsGamePaused());
        if (!isDead)
        {
            navMeshAgent.Warp(pos);
        }
    }

    private void OnPlayerDetected(bool isDetected)
    {
        isPlayerDetected = isDetected;
        navMeshAgent.speed = monster.baseSpeed * monster.alertSpeed;
    }


    #region Wander stuff
    private void Wander()
    {
        if (GameEvents.IsGamePaused())
        {
            return;
        }
        if (isPlayerDetected && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.SetDestination(transform.position);
            return;
        }
        wanderTimer += Time.deltaTime;
        if (wanderTimer > monster.wanderRepeatRate)
        {
            wanderTimer = 0;
            Vector3 randDirection = UnityEngine.Random.insideUnitSphere * monster.wanderRadius;
            randDirection += transform.position;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randDirection, out navHit, monster.wanderRadius, -1);
            navMeshAgent.SetDestination(new Vector3(navHit.position.x, 0, navHit.position.z));
        }
    }
    #endregion


    #region Helper functions
    protected void LookAtPlayer()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation((playerTransform.position - transform.position).normalized),
                Time.deltaTime * lookRotationSpeed);
    }

    protected bool IsInAttackZone()
    {
        return Vector3.Distance(playerTransform.position, transform.position) <= monster.attackRadius;
    }

    protected bool IsInViewZone()
    {
        return Vector3.Distance(playerTransform.position, transform.position) <= monster.viewRadius;
    }
    #endregion


    #region TakeHit stuff
    public void TakeHit(int hitPoint)
    {
        attackTimer = 0;
        if (isDead)
        {
            return;
        }
        health -= hitPoint;
        if (health <= 0)
        {
            OnMonsterDied?.Invoke(this);
            gameObject.layer = LayerMask.NameToLayer("Default");
            Destroy(uiHealthBar.gameObject);
            PlayDeadAnimation();

            if (monster.outputOnKill > 0)
            {
                int dropCount = UnityEngine.Random.Range(monster.minDrop, monster.maxDrop + 1);
                if (dropCount > 0)
                {
                    DroppedItemManager.OnDropResource(ItemDatabase.GetItemById(monster.outputOnKill), transform, dropCount);
                }
            }
            Destroy(gameObject, 1);
            isDead = true;
        }
        else
        {
            PlayTakeHitAnimation();
            uiHealthBar.OnHealthChanged(hitPoint, health, monster.health);
            Knockback();
        }
    }

    protected void Knockback()
    {
        Vector3 direction = (transform.position - playerTransform.position).normalized;
        transform.DOMove(transform.position + direction * monster.knockBackDistance, knockBackDuration);
    }

    protected void KnockFront(float knockFrontDistance, Vector3 position, float knockBackSpeed)
    {
        Vector3 direction = (position - transform.position).normalized;
        transform.DOMove(position + direction * knockFrontDistance, knockBackSpeed);
    }
    #endregion


    #region IMonster
    public Transform GetTransform()
    {
        return transform;
    }

    public float GetDamageValue()
    {
        return monster.damage;
    }
    #endregion


    #region All Animation stuff
    protected void PlayTakeHitAnimation()
    {
        animator.SetTrigger("Hitted");
    }

    protected void PlayAttackAnimation()
    {
        animator.SetTrigger("Attack");
    }

    protected void PlayDeadAnimation()
    {
        animator.SetTrigger("Dead");
    }

    protected void IsRunning(bool isRunning)
    {
        animator.SetBool("IsRunning", isRunning);
    }
    #endregion
}