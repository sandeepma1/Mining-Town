using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using MiningTown.IO;

public class UiTimeRemainingCanvas : MonoBehaviour
{
    public static Action<int, Vector3, DateTime, Action> OnShowCanvas;
    public static Action OnHideCanvas;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private Image itemImage;
    [SerializeField] private RectTransform progressBar;
    [SerializeField] private TextMeshProUGUI timeRemainingText;
    [SerializeField] private TextMeshProUGUI gemCountText;
    [SerializeField] private Button gemButton;
    private RectTransform rectTransform;
    private float progressBarWidth = 230;
    private Camera mainCamera;
    private bool isCanvasVisible;
    private TimeSpan timeSpan;
    private DateTime yieldDateTime;
    private float currentYieldTick;
    private Item currentItem;
    private float gemCostInterval;
    private int gemsNeededToCompelete;
    private Action CompleteByGemsRaisedBed;

    private void Start()
    {
        OnShowCanvas += ShowCanvas;
        OnHideCanvas += HideCanvas;
        progressBarWidth = progressBar.sizeDelta.x;
        mainCamera = Camera.main;
        ToggleCanvas(false);
        rectTransform = mainPanel.GetComponent<RectTransform>();
        gemButton.onClick.AddListener(OnGemButtonClicked);
        StartCoroutine(TickPerSecond());
    }

    private void OnDestroy()
    {
        OnShowCanvas -= ShowCanvas;
        gemButton.onClick.RemoveListener(OnGemButtonClicked);
    }

    private IEnumerator TickPerSecond()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(1f);
            if (isCanvasVisible)
            {
                CalculateBarAndTime();
            }
        }
    }

    private void CalculateBarAndTime()
    {
        timeSpan = yieldDateTime.Subtract(DateTime.UtcNow);
        timeRemainingText.text = timeSpan.ToFormattedDuration();
        progressBar.sizeDelta = new Vector2(progressBarWidth - (currentYieldTick * (float)timeSpan.TotalSeconds), 0);
        gemsNeededToCompelete = (int)(timeSpan.TotalSeconds / gemCostInterval) + 1;
        gemCountText.text = GameVariables.tmp_gemIcon + gemsNeededToCompelete.ToString();
        if (timeSpan.TotalSeconds <= 0)
        {
            HideCanvas();
        }
    }

    private void ShowCanvas(int itemId, Vector3 pos, DateTime yieldDateTime, Action CompleteByGems)
    {
        CompleteByGemsRaisedBed = CompleteByGems;
        timeRemainingText.text = "";
        currentItem = ItemDatabase.GetItemById(itemId);
        gemCountText.text = GameVariables.tmp_gemIcon + currentItem.buyValueInGems.ToString();
        Vector3 screenPos = mainCamera.WorldToScreenPoint(pos);
        itemImage.sprite = AtlasBank.Instance.GetSpriteByName(currentItem.slug, AtlasType.UiItems);
        rectTransform.position = screenPos;
        this.yieldDateTime = yieldDateTime;
        currentYieldTick = progressBarWidth / (currentItem.yieldDurationInMins * 60);
        gemCostInterval = (currentItem.yieldDurationInMins * 60) / currentItem.buyValueInGems;
        //Calculate bar, time to avoide the flicker while showing bar and time
        CalculateBarAndTime();
        ToggleCanvas(true);
    }

    private void HideCanvas()
    {
        if (isCanvasVisible)
        {
            ToggleCanvas(false);
        }
    }

    private void OnGemButtonClicked()
    {
        ToggleCanvas(false);
        bool hasGems = PlayerCurrencyManager.ReduceGems(gemsNeededToCompelete);
        if (hasGems)
        {
            CompleteByGemsRaisedBed?.Invoke();
        }
        else
        {
            //Show some common ui to purchase gems
        }
    }

    protected void ToggleCanvas(bool isVisible)
    {
        isCanvasVisible = isVisible;
        mainPanel.gameObject.SetActive(isVisible);
    }
}