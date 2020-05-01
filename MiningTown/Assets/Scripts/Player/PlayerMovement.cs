using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static Action<Vector3, bool> OnPlayerMoved;
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private BasicSword basicSwordPrefab;
    [SerializeField] private Transform playerMeshRotation;
    [SerializeField] private Transform playerCrosshairDot;
    [SerializeField] private Transform playerRangeCircle;
    [SerializeField] private float m_moveSpeed = 5;
    [SerializeField] private float m_interpolation = 50;
    [SerializeField] private Animator m_animator;
    private Transform mainCamera;
    private Vector3 m_currentDirection = Vector3.zero;
    private float currentV = 0;
    private float currentH = 0;
    private Vector2 inputAxis = Vector2.zero;
    private bool isPlayerMoving = false;
    private const float rotationSpeed = 10;
    private MonsterBase closestMonster = null;
    private const float playerAttackRange = 2f;
    private const float maxDotDistance = 2f;
    private BasicSword basicSword; //Change to weapon base later

    private void Start()
    {
        InitPlayer();
        mainCamera = Camera.main.transform;
        Joystick.OnJoystickMove += OnJoystickMove;
    }

    private void OnDestroy()
    {
        Joystick.OnJoystickMove -= OnJoystickMove;
    }

    private void InitPlayer()
    {
        basicSword = Instantiate(basicSwordPrefab, weaponHolder);
        playerRangeCircle.localScale = new Vector3(playerAttackRange, playerAttackRange, 0);
        OnPlayerMoved?.Invoke(transform.position, false);
    }

    private void OnJoystickMove(Vector2 position, bool isMoving)
    {
        playerCrosshairDot.gameObject.SetActive(isMoving);
        playerCrosshairDot.transform.localPosition = position * maxDotDistance;
        inputAxis = position;
        m_animator.SetBool("Grounded", inputAxis != Vector2.zero);
    }

    private void FixedUpdate()
    {
        currentV = Mathf.Lerp(currentV, inputAxis.y, Time.deltaTime * m_interpolation);
        currentH = Mathf.Lerp(currentH, inputAxis.x, Time.deltaTime * m_interpolation);
        Vector3 direction = mainCamera.forward * currentV + mainCamera.right * currentH;
        direction.y = 0;
        direction = direction.normalized;
        if (direction != Vector3.zero) //Player Moving
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);
            playerMeshRotation.rotation = Quaternion.LookRotation(m_currentDirection);
            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;
            OnPlayerMoved?.Invoke(transform.position, true);
            m_animator.SetFloat("MoveSpeed", direction.magnitude);
            isPlayerMoving = true;
        }
        else //Stopped
        {
            //Note: Make changes here if the last selected monster should be selected or always 
            //attack nearest monster
            OnPlayerMoved?.Invoke(transform.position, false);
            isPlayerMoving = false;
            closestMonster = MonsterManager.Instance.GetNearestMonsterFromPosition(transform.position);
            if (closestMonster != null)
            {
                RotateTowardsClosestEnemy(closestMonster.transform.position);
            }
        }
    }

    private void RotateTowardsClosestEnemy(Vector3 positionToLook)
    {
        Hud.SetHudText?.Invoke("RotateTowardsClosestEnemy " + positionToLook);
        playerMeshRotation.rotation = Quaternion.Slerp(playerMeshRotation.rotation,
            Quaternion.LookRotation((positionToLook - transform.position).normalized),
            Time.deltaTime * rotationSpeed);
        AttackClosestMonster();
    }

    private void AttackClosestMonster()
    {
        if (closestMonster == null)
        {
            return;
        }
        float closestEnemyDistance = Vector3.Distance(transform.position, closestMonster.transform.position);
        Hud.SetHudText?.Invoke(closestEnemyDistance.ToString());
        if (closestEnemyDistance <= playerAttackRange)
        {
            basicSword.Attack();
            Hud.SetHudText?.Invoke("Attacking");
        }
    }
}