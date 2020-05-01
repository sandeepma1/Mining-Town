using UnityEngine;
using UnityEngine.AI;

public class MonsterBase : MonoBehaviour
{
    [SerializeField] private Transform uiHealthBarPosition;
    [SerializeField] private float maxHealth = 5;
    private float health;
    private UiHealthBar uiHealthBar;
    private NavMeshAgent navMeshAgent;
    private Camera mainCamera;
    private const float rotationSpeed = 10;
    private bool followPlayerTrigger = false;

    private void Start()
    {
        health = maxHealth;
        PlayerMovement.OnPlayerMoved += OnPlayerMoved;
        mainCamera = Camera.main;
        navMeshAgent = GetComponent<NavMeshAgent>();
        uiHealthBar = Instantiate(UiHealthBarCanvas.Instance.GetUiHealthBarPrefab());
        uiHealthBar.Init(uiHealthBarPosition);
        FollowPlayer();
    }

    private void OnDestroy()
    {
        PlayerMovement.OnPlayerMoved -= OnPlayerMoved;
    }

    private void OnPlayerMoved(Vector3 playerPosition, bool isPlayerMoving)
    {
        if (isPlayerMoving)
        {
            navMeshAgent.SetDestination(playerPosition);
            RotateTowards(playerPosition);
            followPlayerTrigger = true;
        }
        else
        {
            if (followPlayerTrigger)
            {
                followPlayerTrigger = false;
                FollowPlayer();
            }
        }
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation((targetPosition - transform.position).normalized),
            Time.deltaTime * rotationSpeed);
    }

    private void FollowPlayer()
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    // monster stopped
                }
            }//else monster running
        }
    }

    #region GetComponent Collision in other scripts TakeHit()
    public void TakeHit()
    {
        health--;
        if (health <= 0)
        {
            MonsterManager.Instance.MonsterDied(this);
            Destroy(uiHealthBar.gameObject);
            Destroy(this.gameObject);
        }
        else
        {
            uiHealthBar.OnHealthChanged(health / maxHealth);
        }
    }
    #endregion
}