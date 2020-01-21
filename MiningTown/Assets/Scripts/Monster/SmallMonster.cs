using System;
using UnityEngine;
using UnityEngine.AI;

public class SmallMonster : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    [SerializeField] private float rotationSpeed = 2;

    private void Start()
    {
        PlayerMovement.OnPlayerMoved += OnPlayerMoved;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void OnDestroy()
    {
        PlayerMovement.OnPlayerMoved -= OnPlayerMoved;
    }

    private void OnPlayerMoved(Vector3 playerPosition)
    {
        navMeshAgent.SetDestination(playerPosition);
        RotateTowards(playerPosition);
    }

    private void LateUpdate()
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    DebugText.PrintDebugText("Stopped");
                }
            }//else running
        }
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation((targetPosition - transform.position).normalized),
            Time.deltaTime * rotationSpeed);
    }
}