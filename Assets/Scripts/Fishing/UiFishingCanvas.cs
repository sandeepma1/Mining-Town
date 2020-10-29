using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class UiFishingCanvas : MonoBehaviour
{
    public static Action<bool, FishObject> OnFishingToggle;
    [Header("Common")]
    [Space(10)]
    [SerializeField] private GameObject mainFishingPanel;

    [Header("PowerBar")]
    [Space(10)]
    [SerializeField] private GameObject powerBarPanel;
    [SerializeField] private RectTransform powerBarRect;
    private Image powerBarImage;
    private float powerBarMaxHeight;
    private Sequence pingPong;
    private const Ease ease = Ease.Linear;

    [Header("BaitWait")]
    [Space(10)]
    [SerializeField] private RectTransform baitAlertRect;
    private Vector2 baitAlertRectDefaultSize;
    private float baitWaitTimer;
    private float baitWaitTime;
    private const float baitWaitAlertDuration = 2f;
    private float baitWaitAlertTimer;
    private bool isFishBaitAlert;

    [Header("Fish movement")]
    [Space(10)]
    [SerializeField] private int maxDir = 3;
    [SerializeField] private float aminDuration = 0.5f;
    private Vector2 fishDefaultPosition;
    private Rigidbody2D barRigidbody2D;
    private float randomThrust = 10f;
    private int ranDirection = 0;
    private float moveYPosition;
    private float fishMoveTimer;
    private const float fishMoveMaxTimer = 0.1f;

    [Header("Bar movement")]
    [Space(10)]
    private Vector2 force;
    private float barJumpThrust = 250;
    private Vector2 barDefaultPosition;
    private bool isMainButtonDown = false;
    private FishObject currentFishObject;
    private float pressHoldMainButtonTimer = 0;
    public float pressHoldMainButtonTime = 0.25f;
    private FishingState fishingState;

    [SerializeField] private Button cancelFishingButton;
    [SerializeField] private Button mainButton;
    [SerializeField] private Transform realShaftImage;
    [SerializeField] private RectTransform fishImageRect;
    [SerializeField] private RectTransform reelMainBarRect;
    [SerializeField] private RectTransform barRect;
    [SerializeField] private RectTransform progressBarRect;
    [SerializeField] private RectTransform progressBarBgRect;
    [SerializeField] private float progressBarReduceFactor = 50;
    private const float reelRotateSpeed = 50;
    private float fishMoveMax;
    private float mainBarHalfSize;
    private UiBarCollider uiBarCollider;
    private Image barImage;
    private bool isFishInside;

    private void Start()
    {
        uiBarCollider = barRect.GetComponent<UiBarCollider>();
        barImage = barRect.GetComponent<Image>();
        barRigidbody2D = barRect.GetComponent<Rigidbody2D>();
        powerBarImage = powerBarRect.GetComponent<Image>();

        baitAlertRectDefaultSize = baitAlertRect.sizeDelta;
        powerBarImage.color = ColorConstants.FishingMenuOrangeColor;
        powerBarMaxHeight = powerBarRect.sizeDelta.y;
        powerBarRect.sizeDelta = new Vector2(powerBarRect.sizeDelta.x, 0);
        fishDefaultPosition = fishImageRect.anchoredPosition;
        barDefaultPosition = barRect.anchoredPosition;
        force = new Vector2(0, barRigidbody2D.mass);

        OnFishingToggle += FishingMenuToggle;
        uiBarCollider.OnFishTriggerStay += OnFishTriggerStay;
        cancelFishingButton.onClick.AddListener(OnCancelFishingButtonClicked);
        mainButton.onClick.AddListener(OnMainButtonClicked);

        mainBarHalfSize = (reelMainBarRect.sizeDelta.y - 30) / 2;
        fishMoveMax = mainBarHalfSize - (fishImageRect.sizeDelta.y / 2);
        FishingMenuToggle(false);
    }

    private void OnDestroy()
    {
        OnFishingToggle -= FishingMenuToggle;
        uiBarCollider.OnFishTriggerStay -= OnFishTriggerStay;
        cancelFishingButton.onClick.RemoveListener(OnCancelFishingButtonClicked);
        mainButton.onClick.RemoveListener(OnMainButtonClicked);
    }

    private void Update()
    {
        //Hud.SetHudText?.Invoke(fishingState.ToString());
        switch (fishingState)
        {
            case FishingState.BaitWait:
                BaitWaitTimer();
                break;
            case FishingState.Fishing:
                Fishing();
                break;
            default:
                break;
        }
    }

    private void ResetAllValues()
    {
        fishingState = FishingState.WaitingForFisihing;
        powerBarPanel.SetActive(false);
        reelMainBarRect.gameObject.SetActive(false);
        fishImageRect.anchoredPosition = fishDefaultPosition;
        barRect.anchoredPosition = barDefaultPosition;
        progressBarRect.sizeDelta = new Vector2(progressBarBgRect.sizeDelta.x, progressBarBgRect.sizeDelta.y / 3);
        isFishInside = false;
        isMainButtonDown = false;
        baitAlertRect.gameObject.SetActive(false);
        baitAlertRect.sizeDelta = baitAlertRectDefaultSize;
    }


    #region PowerSelection
    private void StartPowerBar()
    {
        fishingState = FishingState.PowerSelection;
        powerBarPanel.SetActive(true);
        powerBarRect.sizeDelta = new Vector2(powerBarRect.sizeDelta.x, powerBarMaxHeight);
        pingPong = DOTween.Sequence();
        pingPong.Append(powerBarRect.DOSizeDelta(new Vector2(powerBarRect.sizeDelta.x, 0), 1))
            .Join(powerBarImage.DOColor(ColorConstants.FishingMenuGreenColor, 1));
        pingPong.SetLoops(-1, LoopType.Yoyo);
        pingPong.SetEase(ease);
    }

    private void FinishPowerSelection()
    {
        PlayerCurrencyManager.ReduceEnergy(GameVariables.energy_fishing);
        float power = powerBarRect.sizeDelta.y / powerBarMaxHeight;
        //Depending on "power" play animation. Play player animation to throw the bait
        pingPong.Pause();
        powerBarPanel.SetActive(false);
        pingPong.Rewind();
        StartWaitForBait();
    }
    #endregion


    #region Wait for Bait
    private void StartWaitForBait()
    {
        baitWaitTimer = 0;
        baitWaitAlertTimer = 0;
        baitWaitTime = UnityEngine.Random.Range(currentFishObject.minBaitWait, currentFishObject.maxBaitWait);
        fishingState = FishingState.BaitWait;
    }

    //Wait for fish to catch the bait
    private void BaitWaitTimer()
    {
        baitWaitTimer += Time.deltaTime;
        if (baitWaitTimer >= baitWaitTime)
        {
            baitAlertRect.gameObject.SetActive(true);
            baitAlertRect.DOSizeDelta(baitAlertRectDefaultSize * 2, baitWaitAlertDuration);
            TriggerBaitAlert();
        }
    }

    //Fish Caught, user has to click the alert to start fishing
    private void TriggerBaitAlert()
    {
        isFishBaitAlert = true;
        baitWaitAlertTimer += Time.deltaTime;
        if (baitWaitAlertTimer >= baitWaitAlertDuration)
        {
            baitWaitAlertTimer = 0;
            isFishBaitAlert = false;
            CancelFishing();
        }
    }

    //On main button clicked at "BaitWait" state
    private void FinishBaitWait()
    {
        if (isFishBaitAlert)
        {
            isFishBaitAlert = false;
            if (currentFishObject.strugglePower > 0) // These are fishes
            {
                fishingState = FishingState.Fishing;
                reelMainBarRect.gameObject.SetActive(true);
            }
            else // These are emptycan, boots, trash, etc
            {
                UiFishCaughCanvas.OnShowFishCaughtCanvas?.Invoke(currentFishObject);
                FishingMenuToggle(false);
            }

        }
        else
        {
            CancelFishing();
        }
        baitAlertRect.gameObject.SetActive(false);
    }
    #endregion


    #region Fish movement
    private void Fishing()
    {
        //Move Fish in main UI
        fishMoveTimer += Time.deltaTime;
        if (fishMoveTimer > fishMoveMaxTimer)
        {
            fishMoveTimer = 0;
            ranDirection = UnityEngine.Random.Range(0, maxDir);
            randomThrust = UnityEngine.Random.Range(0, currentFishObject.strugglePower);
            if (ranDirection == 0) //Up
            {
                moveYPosition = fishImageRect.anchoredPosition.y - randomThrust;
            }
            if (ranDirection == 1) //Down
            {
                moveYPosition = fishImageRect.anchoredPosition.y + randomThrust;
            }
            fishImageRect.DOAnchorPosY(Mathf.Clamp(moveYPosition, -fishMoveMax, fishMoveMax), aminDuration);
        }
        //Auto Reel 
        ReelFishingRod();

        if (isMainButtonDown)
        {
            pressHoldMainButtonTimer += Time.deltaTime;
            if (pressHoldMainButtonTimer >= pressHoldMainButtonTime)
            {
                pressHoldMainButtonTimer = 0;
                OnMainButtonClicked();
            }
        }
    }

    private void ReelFishingRod()
    {
        if (isFishInside)
        {
            realShaftImage.Rotate(0, 0, 6 * reelRotateSpeed * Time.deltaTime * -1);
            progressBarRect.sizeDelta =
                new Vector2(progressBarRect.sizeDelta.x, progressBarRect.sizeDelta.y + progressBarReduceFactor);
            if (progressBarRect.sizeDelta.y >= progressBarBgRect.sizeDelta.y)
            {
                UiFishCaughCanvas.OnShowFishCaughtCanvas?.Invoke(currentFishObject);
                FishingMenuToggle(false);
            }
        }
        else
        {
            progressBarRect.sizeDelta =
                  new Vector2(progressBarRect.sizeDelta.x, progressBarRect.sizeDelta.y - progressBarReduceFactor);
            realShaftImage.Rotate(0, 0, 6 * reelRotateSpeed * 2 * Time.deltaTime * 1);
            if (progressBarRect.sizeDelta.y <= 0)
            {
                FishingMenuToggle(false);
            }
        }
    }

    private void OnFishTriggerStay(bool isFishInside)
    {
        this.isFishInside = isFishInside;
        barImage.color = isFishInside ? ColorConstants.FishingMenuGreenColor : ColorConstants.FishingMenuOrangeColor;
    }
    #endregion


    #region Cancel Fishing
    private void CancelFishing()
    {
        OnCancelFishingButtonClicked();
    }
    #endregion


    private void FishingMenuToggle(bool startFishing, FishObject fishObject = null)
    {
        ResetAllValues();
        if (startFishing)
        {
            StartPowerBar();
        }
        currentFishObject = fishObject;
        if (currentFishObject != null)
        {
            print(currentFishObject.name);
        }
        PlayerFollowCameraManager.OnZoomOnPlayer?.Invoke(startFishing);
        UiAllScreenButtonsCanvas.OnToggleAllButtons?.Invoke(!startFishing);
        mainFishingPanel.gameObject.SetActive(startFishing);
    }


    #region All UI Buttons    
    private void OnMainButtonClicked()
    {
        switch (fishingState)
        {
            case FishingState.PowerSelection:
                FinishPowerSelection();
                break;
            case FishingState.BaitWait:
                FinishBaitWait();
                break;
            case FishingState.Fishing:
                barRigidbody2D.AddForce(force * barJumpThrust, ForceMode2D.Impulse);
                break;
            default:
                break;
        }
    }

    private void OnCancelFishingButtonClicked()
    {
        FishingMenuToggle(false);
    }

    //On Press and hold main button    
    public void OnMainButtonDown()
    {
        isMainButtonDown = true;
    }

    public void OnMainButtonUp()
    {
        isMainButtonDown = false;
    }
    #endregion
}

public enum FishingState
{
    WaitingForFisihing,
    PowerSelection,
    BaitWait,
    Fishing,
}