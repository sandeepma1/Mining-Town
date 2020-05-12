using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    //public static Action<Vector3, bool> OnPlayerMoved;
    [SerializeField] private LayerMask searchLayer;
    [SerializeField] private float searchRadius = 5;
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private BasicSword basicSwordPrefab;
    [SerializeField] private BasicPickaxe basicPickaxePrefab;
    [SerializeField] private Transform playerMeshRotation;
    [SerializeField] private Transform playerCrosshairDot;
    [SerializeField] private Transform playerRangeCircle;
    [SerializeField] private float m_moveSpeed = 5;
    [SerializeField] private Animator m_animator;
    [SerializeField] private BreakableManager breakableManager;
    [SerializeField] private MonsterManager monsterManager;
    private Transform mainCamera;
    private Vector3 m_currentDirection = Vector3.zero;
    private float currentV = 0;
    private float currentH = 0;
    private Vector2 inputAxis = Vector2.zero;
    private bool isPlayerMoving = false;
    private BasicSword basicSword; //Change to weapon base later
    private BasicPickaxe basicPickaxe; //Change to weapon base later
    private IInteractable closestInteractable;
    private const float rotationSpeed = 10;
    private const float playerAttackRange = 2f;
    private const float maxDotDistance = 2f;
    private const float joystickInterpolation = 50;
    private const float maxMonsterInteractionDistance = 4; //if this distance, then hit monster only not breakables

    private void Awake()
    {
        Instance = this;
    }

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
        basicPickaxe = Instantiate(basicPickaxePrefab, weaponHolder);
        playerRangeCircle.localScale = new Vector3(playerAttackRange, playerAttackRange, 0);
        //OnPlayerMoved?.Invoke(transform.position, false);
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
        currentV = Mathf.Lerp(currentV, inputAxis.y, Time.deltaTime * joystickInterpolation);
        currentH = Mathf.Lerp(currentH, inputAxis.x, Time.deltaTime * joystickInterpolation);
        Vector3 direction = mainCamera.forward * currentV + mainCamera.right * currentH;
        direction.y = 0;
        direction = direction.normalized;
        if (direction != Vector3.zero) //Player Moving
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * joystickInterpolation);
            playerMeshRotation.rotation = Quaternion.LookRotation(m_currentDirection);
            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;
            //OnPlayerMoved?.Invoke(transform.position, true);
            m_animator.SetFloat("MoveSpeed", direction.magnitude);
            isPlayerMoving = true;
        }
        else //Stopped
        {
            //Note: Make changes here if the last selected monster should be selected or always 
            //attack nearest monster
            //OnPlayerMoved?.Invoke(transform.position, false);
            isPlayerMoving = false;
        }
    }

    private void Update()
    {
        MonsterBase nearestMonster = monsterManager.GetNearestMonsterFromPosition(transform.position);
        BreakableBase nearestBreakable = breakableManager.GetNearestBreakableFromPosition(transform.position);
        if (nearestMonster != null)
        {
            Vector3 pos = nearestMonster.transform.position;
            if (IsClosedToMonster(pos)) //Priority given to monster to hit first even if breakble is close
            {
                LookTowardsObject(pos);
                SelectionCircle.SetToThisParent?.Invoke(nearestMonster.transform);
                if (IsClosedToObject(pos) && !isPlayerMoving)
                {
                    basicSword.Attack();
                }
                return;
            }
        }

        if (nearestBreakable != null)
        {
            Vector3 pos = nearestBreakable.transform.position;
            SelectionCircle.SetToThisParent?.Invoke(nearestBreakable.transform);
            if (IsClosedToObject(pos) && !isPlayerMoving)
            {
                LookTowardsObject(pos);
                basicPickaxe.Attack();
            }
        }
    }

    private IInteractable GetClosestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius, searchLayer);
        Collider nearestCollider = null;
        float minSqrDistance = Mathf.Infinity;
        for (int i = 0; i < colliders.Length; i++)
        {
            float sqrDistanceToCenter = (transform.position - colliders[i].transform.position).sqrMagnitude;
            if (sqrDistanceToCenter < minSqrDistance)
            {
                minSqrDistance = sqrDistanceToCenter;
                nearestCollider = colliders[i];
            }
        }
        if (nearestCollider == null)
        {
            return null;
        }
        else
        {
            return nearestCollider.GetComponent<IInteractable>();
        }
    }

    private void LookTowardsObject(Vector3 position)
    {
        playerMeshRotation.rotation = Quaternion.Slerp(playerMeshRotation.rotation,
           Quaternion.LookRotation((position - transform.position).normalized),
           Time.deltaTime * rotationSpeed);
    }

    private bool IsClosedToObject(Vector3 objectPosition)
    {
        float distance = Vector3.Distance(transform.position, objectPosition);
        return (distance <= playerAttackRange);
    }

    private bool IsClosedToMonster(Vector3 objectPosition)
    {
        float distance = Vector3.Distance(transform.position, objectPosition);
        return (distance <= maxMonsterInteractionDistance);
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public Transform GetPlayerTransform()
    {
        return transform;
    }
}