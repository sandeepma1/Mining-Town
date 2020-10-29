using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class PlayerFollowCameraManager : MonoBehaviour
{
    public static Action<Vector2> OnTouchEdge;
    public static Action<bool> OnZoomOnPlayer;
    [SerializeField] private float cameraSmooth = 0.1f;
    [Header("Edit Mode Clamp values")]
    [Space(10)]
    [SerializeField] private float eMinX = -20;
    [SerializeField] private float eMaxX = 20;
    [SerializeField] private float eMinZ = -31;
    [SerializeField] private float eMaxZ = -22;
    [Header("Normal Mode Clamp values")]
    [Space(10)]
    [SerializeField] private float minX = -20;
    [SerializeField] private float maxX = 20;
    [SerializeField] private float minZ = -31;
    [SerializeField] private float maxZ = -22;
    [Header("Mines Mode Clamp values")]
    [Space(10)]
    [SerializeField] private float mMinX = 0;
    [SerializeField] private float mMaxX = 0;
    [SerializeField] private float mMinZ = 0;
    [SerializeField] private float mMaxZ = 0;
    [Space(10)]
    [SerializeField] private float zOffset = -20;
    [SerializeField] private float cameraHeight = 25;
    private Transform playerTransform;
    private Vector3 cameraNormalView = new Vector3(55, 0, 0);
    private Vector3 cameraEditView = new Vector3(85, 0, 0);
    private Vector3 posBeforeEditMode;
    private bool isInEditMode;
    private const float zYMinus = 16;
    private const float animDurartion = 0.5f;

    private const float panStep = 0.2f;
    private const float panUpdateTime = 0.001f;
    private float timer = 0;

    //Pan stuff
    [SerializeField] private float PanSpeed = 20f;
    private Camera mainCamera;
    private Vector2 lastPanPosition;
    private int panFingerId; // Touch mode only
    private Vector3 panPos;
    private List<Touch> touches = new List<Touch>();
    private bool isBuildingSelected;
    private bool isZoomAnimationDone;
    private Scenes currentScene;
    private float sceneFovSize = 60;

    private void Awake()
    {
        OnZoomOnPlayer += ZoomOnPlayer;
        OnTouchEdge += TouchEdge;
        MiningLevelGenerator.OnLevelSizeLoaded += OnMiningLevelSizeLoaded;
        ForestLevelGenerator.OnLevelSizeLoaded += OnMiningLevelSizeLoaded;
        UiBuildingEditModeCanvas.OnToggleEditMode += OnToggleEditMode;
        UiBuildingEditModeCanvas.OnToggleButtonsPanel += OnToggleButtonsPanel;
        mainCamera = Camera.main;
        //OnSceneLoaded();
        SceneLoader.OnSceneChanged += OnSceneLoaded;
    }

    private void Start()
    {
        playerTransform = PlayerMovement.Instance.GetPlayerTransform();
        transform.position = playerTransform.transform.position;
    }

    private void OnDestroy()
    {
        SceneLoader.OnSceneChanged -= OnSceneLoaded;
        OnZoomOnPlayer -= ZoomOnPlayer;
        MiningLevelGenerator.OnLevelSizeLoaded -= OnMiningLevelSizeLoaded;
        OnTouchEdge -= TouchEdge;
        UiBuildingEditModeCanvas.OnToggleEditMode -= OnToggleEditMode;
        UiBuildingEditModeCanvas.OnToggleButtonsPanel -= OnToggleButtonsPanel;
    }

    private void ZoomOnPlayer(bool zoom)
    {
        if (zoom)
        {
            mainCamera.DOFieldOfView(45, animDurartion);
        }
        else
        {
            mainCamera.DOFieldOfView(sceneFovSize, animDurartion);
        }
    }

    private void OnMiningLevelSizeLoaded(int arg1, int arg2)
    {
        mMinX = 5;
        mMaxX = arg1 - 5;

        mMinZ = -15;
        mMaxZ = arg2 + zOffset - 5;

    }

    private void OnSceneLoaded(Scenes scene)
    {
        //currentScene = (Scenes)Enum.Parse(typeof(Scenes), SceneManager.GetActiveScene().name);
        currentScene = scene;
        switch (currentScene)
        {
            case Scenes.Loading:
                break;
            case Scenes.FarmHome:
                sceneFovSize = 60;
                break;
            case Scenes.Mines:
                sceneFovSize = 45;
                break;
            case Scenes.Forest:
                sceneFovSize = 55;
                break;
            case Scenes.Town:
                sceneFovSize = 60;
                break;
            default:
                break;
        }
        mainCamera.fieldOfView = sceneFovSize;
    }

    private void Update()
    {
        if (isInEditMode)
        {
            if (!isBuildingSelected)
            {
                HandlePanDragTouch();
            }
            if (isZoomAnimationDone)
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, eMinX, eMaxX), transform.position.y,
                                                    Mathf.Clamp(transform.position.z, eMinZ, eMaxZ));
            }
        }
        else //Player follow clamp values
        {
            switch (currentScene)
            {
                case Scenes.Loading:
                    transform.position = Vector3.Lerp(transform.position, playerTransform.position, cameraSmooth);
                    transform.position = new Vector3(Mathf.Clamp(playerTransform.position.x, minX, maxX), cameraHeight,
                                                     Mathf.Clamp(playerTransform.position.z + zOffset, minZ, maxZ));
                    break;
                case Scenes.FarmHome:
                    transform.position = Vector3.Lerp(transform.position, playerTransform.position, cameraSmooth);
                    transform.position = new Vector3(Mathf.Clamp(playerTransform.position.x, minX, maxX), cameraHeight,
                                                     Mathf.Clamp(playerTransform.position.z + zOffset, minZ, maxZ));
                    break;
                case Scenes.Mines:
                    transform.position = Vector3.Lerp(transform.position, playerTransform.position, cameraSmooth);
                    transform.position = new Vector3(Mathf.Clamp(playerTransform.position.x, mMinX, mMaxX), cameraHeight,
                                                    Mathf.Clamp(playerTransform.position.z + zOffset, mMinZ, mMaxZ));
                    break;
                case Scenes.Forest:
                    transform.position = Vector3.Lerp(transform.position, playerTransform.position, cameraSmooth);
                    transform.position = new Vector3(playerTransform.position.x, cameraHeight, playerTransform.position.z + zOffset);
                    break;
                case Scenes.Town:
                    transform.position = Vector3.Lerp(transform.position, playerTransform.position, cameraSmooth);
                    transform.position = new Vector3(playerTransform.position.x, cameraHeight, playerTransform.position.z + zOffset);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnToggleEditMode(bool isInEditMode)
    {
        if (isInEditMode)
        {
            this.isInEditMode = isInEditMode;
            posBeforeEditMode = transform.position;
            transform.DORotate(cameraEditView, animDurartion);
            //transform.DOMove(new Vector3(transform.position.x, transform.position.y + zYMinus, transform.position.z + zYMinus),
            //     animDurartion);
            transform.DOMove(new Vector3(Mathf.Clamp(transform.position.x, eMinX, eMaxX),
                transform.position.y + zYMinus,
                Mathf.Clamp(transform.position.z + zYMinus, eMinZ, eMaxZ)),
                 animDurartion).OnComplete(() => isZoomAnimationDone = true);
        }
        else
        {
            isZoomAnimationDone = false;
            transform.DORotate(cameraNormalView, animDurartion);
            transform.DOMove(posBeforeEditMode, animDurartion).OnComplete(() => this.isInEditMode = isInEditMode);
        }
    }


    #region Edge movement stuff
    private void TouchEdge(Vector2 touchPos)
    {
        //Hud.SetHudText?.Invoke(touchPos.ToString());
        if (touchPos.x < GameVariables.edgeScreenRect.x) //Left
        {
            EdgeMoveCamera(transform.position.x - panStep, transform.position.z);
        }
        else if (touchPos.x > GameVariables.edgeScreenRect.width + GameVariables.edgeScreenRect.x) //Right
        {
            EdgeMoveCamera(transform.position.x + panStep, transform.position.z);
        }

        else if (touchPos.y < GameVariables.edgeScreenRect.y) //Bottom
        {
            EdgeMoveCamera(transform.position.x, transform.position.z - panStep);
        }
        else if (touchPos.y > GameVariables.edgeScreenRect.height + GameVariables.edgeScreenRect.y) //Up
        {
            EdgeMoveCamera(transform.position.x, transform.position.z + panStep);
        }
    }

    private void EdgeMoveCamera(float x, float z)
    {
        timer += Time.deltaTime;
        if (timer > panUpdateTime)
        {
            timer = 0;
            transform.position = new Vector3(x, transform.position.y, z);
        }
    }
    #endregion


    #region Pan Drag movement stuff
    private void HandlePanDragTouch()
    {
        touches = InputHelper.GetTouches();
        if (touches.Count != 1 || ExtensionMethods.IsPointerOverUIObject())
        {
            return;
        }
        if (touches[0].phase == TouchPhase.Began)
        {
            lastPanPosition = touches[0].position;
            panFingerId = touches[0].fingerId;
        }
        else if (touches[0].fingerId == panFingerId && touches[0].phase == TouchPhase.Moved)
        {
            panPos = mainCamera.ScreenToViewportPoint(lastPanPosition - touches[0].position) * PanSpeed;
            lastPanPosition = touches[0].position;
            transform.position = transform.position + new Vector3(panPos.x, 0, panPos.y);
        }
    }

    private void OnToggleButtonsPanel(bool obj)
    {
        isBuildingSelected = obj;
    }
    #endregion
}