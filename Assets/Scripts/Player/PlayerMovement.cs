using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    [SerializeField] private Transform playerMeshRotation;
    [SerializeField] private Transform playerCrosshairDot;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Animator animator;
    [SerializeField] private Light spotLight;
    private Transform mainCamera;
    private Vector3 m_currentDirection = Vector3.zero;
    private float currentV = 0;
    private float currentH = 0;
    private Vector2 inputAxis = Vector2.zero;
    public bool isPlayerMoving = false;
    private const float maxDotDistance = 2f;
    private const float joystickInterpolation = 50;
    private Vector3 direction;
    private Rigidbody rigidbody;
    private Vector3Int lastPlayerPosInt;

    private void Awake()
    {
        Instance = this;
        SaveLoadTrigger.OnSaveTrigger += SaveGameData;
    }

    private void Start()
    {
        SceneLoader.OnSceneChanged += OnSceneChanged;
        rigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main.transform;
        Joystick.OnJoystickMove += OnJoystickMove;
    }

    private void OnDestroy()
    {
        SceneLoader.OnSceneChanged -= OnSceneChanged;
        SaveLoadTrigger.OnSaveTrigger -= SaveGameData;
        Joystick.OnJoystickMove -= OnJoystickMove;
    }

    private void FixedUpdate()
    {
        if (isPlayerMoving) //Player Moving
        {
            rigidbody.MovePosition(transform.position + m_currentDirection * moveSpeed * Time.deltaTime);
        }
    }

    private void Update()
    {
        if (GameEvents.IsGamePaused())
        {
            isPlayerMoving = false;
            inputAxis = Vector2.zero;
            return;
        }
        MovePlayer();
    }

    private void MovePlayer()
    {
        currentV = Mathf.Lerp(currentV, inputAxis.y, Time.deltaTime * joystickInterpolation);
        currentH = Mathf.Lerp(currentH, inputAxis.x, Time.deltaTime * joystickInterpolation);
        direction = mainCamera.forward * currentV + mainCamera.right * currentH;
        direction.y = 0;
        direction = direction.normalized;
        if (direction != Vector3.zero) //Player Moving
        {
            MainCanvasManager.OnHideFloatingUi?.Invoke();
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * joystickInterpolation);
            playerMeshRotation.rotation = Quaternion.LookRotation(m_currentDirection);
            isPlayerMoving = true;

            if (lastPlayerPosInt != Vector3Int.FloorToInt(transform.position))
            {
                lastPlayerPosInt = Vector3Int.FloorToInt(transform.position);
                GrassManager.OnStepOnGrass?.Invoke(lastPlayerPosInt);
            }

        }
        else //Stopped
        {
            isPlayerMoving = false;
        }
        animator.SetBool("IsRunning", isPlayerMoving);
        //rigidbody.isKinematic = !isPlayerMoving;
    }

    private void OnJoystickMove(Vector2 position, bool isMoving)
    {
        if (GameEvents.IsGamePaused())
        {
            return;
        }
        //animator.SetBool("Grounded", inputAxis != Vector2.zero);
        playerCrosshairDot.gameObject.SetActive(isMoving);
        playerCrosshairDot.transform.localPosition = position * maxDotDistance;
        inputAxis = position;
    }

    private void OnSceneChanged(Scenes scenes)
    {
        //string activeSceneName = SceneManager.GetActiveScene().name;
        //Scenes scenes = (Scenes)Enum.Parse(typeof(Scenes), SceneManager.GetActiveScene().name);
        switch (scenes)
        {
            case Scenes.Loading:
                break;
            case Scenes.FarmHome:
                moveSpeed = 10;
                transform.position = SaveLoadManager.saveData.playerStats.playerPosition;
                spotLight.enabled = false;
                break;
            case Scenes.Mines:
                spotLight.enabled = true;
                moveSpeed = 7f;
                break;
            case Scenes.Forest:
                spotLight.enabled = false;
                moveSpeed = 10;
                break;
            case Scenes.Town:

                transform.position = SaveLoadManager.saveData.playerStats.playerPosition;
                spotLight.enabled = false;
                moveSpeed = 10;
                break;
            default:
                break;
        }
        //print(scenes + ": " + SaveLoadManager.saveData.playerPosition + "  " + transform.position);
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public Transform GetPlayerTransform()
    {
        return transform;
    }


    #region Save Game on Trigger
    private void SaveGameData()
    {
        //Scenes scenes = (Scenes)Enum.Parse(typeof(Scenes), SceneManager.GetActiveScene().name);
        //switch (scenes)
        //{
        //    case Scenes.Loading:
        //        break;
        //    case Scenes.FarmHome:
        //        SaveLoadManager.SavePlayerPosition(GameVariables.housePosition);
        //        break;
        //    case Scenes.Mines:
        //    case Scenes.Forest:
        //        SaveLoadManager.SavePlayerPosition(SaveLoadManager.saveData.playerStats.playerPosition);
        //        break;
        //    case Scenes.Town:
        //        SaveLoadManager.SavePlayerPosition(SaveLoadManager.saveData.playerStats.playerPosition);
        //        break;
        //    default:
        //        break;
        //}
    }
    #endregion
}