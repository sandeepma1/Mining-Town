using UnityEngine;
using UnityEngine.AI;

public class MonsterBase : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Camera mainCamera;
    private const float rotationSpeed = 10;
    [SerializeField] private Transform uiHealthBarPosition;
    private float health;
    [SerializeField] private float maxHealth = 5;
    private UiHealthBar uiHealthBar;

    private void Start()
    {
        health = maxHealth;
        PlayerMovement.OnPlayerMoved += OnPlayerMoved;
        mainCamera = Camera.main;
        navMeshAgent = GetComponent<NavMeshAgent>();
        uiHealthBar = Instantiate(UiHealthBarCanvas.Instance.GetUiHealthBarPrefab());
        uiHealthBar.Init(uiHealthBarPosition);
    }

    private void OnDestroy()
    {
        PlayerMovement.OnPlayerMoved -= OnPlayerMoved;
    }

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

    private void LateUpdate()
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {

                }
            }//else running
        }
    }

    private void OnPlayerMoved(Vector3 playerPosition)
    {
        navMeshAgent.SetDestination(playerPosition);
        RotateTowards(playerPosition);
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation((targetPosition - transform.position).normalized),
            Time.deltaTime * rotationSpeed);
    }
}