using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using MiningTown.IO;

public class UiLivestock : MonoBehaviour, IPointerEnterHandler
{
    public Action<int> OnDropItem;
    public Action<int, int> OnGemButtonClick;
    [SerializeField] private Image livestockImage;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button gemsCompleteButton;
    [SerializeField] private TextMeshProUGUI gemCountText;
    private int gemsNeededToCompelete;
    private Item currentItem;
    private int uiLivestockIndex;
    private float gemCostInterval;

    private void Start()
    {
        gemsCompleteButton.onClick.AddListener(GemButtonClick);
    }

    private void OnDestroy()
    {
        gemsCompleteButton.onClick.RemoveListener(GemButtonClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnDropItem?.Invoke(uiLivestockIndex);
    }

    public void Init(int index)
    {
        uiLivestockIndex = index;
    }

    public void InitLivestock(int outputItemId, Sprite livestockSprite)
    {
        currentItem = ItemDatabase.GetItemById(outputItemId);
        livestockImage.sprite = livestockSprite;
    }

    public void OnHungry()
    {
        ToggleIsEating(false);
    }

    public void OnEating(TimeSpan timeSpan)
    {
        ToggleIsEating(true);
        timerText.text = timeSpan.ToFormattedDuration();
        if (currentItem == null)
        {
            print(uiLivestockIndex + " Is null");
            return;
        }
        gemCostInterval = (currentItem.yieldDurationInMins * 60) / currentItem.buyValueInGems;
        gemsNeededToCompelete = (int)(timeSpan.TotalSeconds / gemCostInterval) + 1;
        gemCountText.text = GameVariables.tmp_gemIcon + gemsNeededToCompelete;
    }

    public void OnHarvest()
    {
        ToggleIsEating(false);
    }

    private void ToggleIsEating(bool isEating)
    {
        gemsCompleteButton.gameObject.SetActive(isEating);
        if (!isEating)
        {
            timerText.text = "";
        }
    }

    private void GemButtonClick()
    {
        OnGemButtonClick?.Invoke(gemsNeededToCompelete, uiLivestockIndex);
    }
}