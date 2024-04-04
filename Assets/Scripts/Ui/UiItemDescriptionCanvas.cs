using MiningTown.IO;
using TMPro;
using UnityEngine;
using System;

public class UiItemDescriptionCanvas : MonoBehaviour
{
    public static Action<int> OnShowReceipeDescription;
    public static Action<int> OnShowItemDescription;
    public static Action<int> OnShowCropDescription;
    public static Action<int, int> OnShowPurchaseItemDescription;
    public static Action OnHideCanvas;
    [SerializeField] private UiRequiredItem[] uiRequiredItems;
    [SerializeField] private RectTransform mainPanel;
    [SerializeField] private TextMeshProUGUI itemNameHeaderText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private GameObject requiredItemsPanel;
    [SerializeField] private TextMeshProUGUI yieldTimeText;
    [SerializeField] private TextMeshProUGUI barnItemsCountText;
    private bool isCanvasVisible;
    private Item currentItem;
    private Vector2 movePos;
    private Canvas mainCanvas;

    private void Start()
    {
        OnShowReceipeDescription += ShowReceipeDescription;
        OnShowItemDescription += ShowItemDescription;
        OnShowCropDescription += ShowCropDescription;
        OnShowPurchaseItemDescription += ShowPurchaseItemDescription;
        OnHideCanvas += HideCanvas;
        mainCanvas = GetComponent<Canvas>();
        ToggleCanvas(false);
    }

    private void OnDestroy()
    {
        OnShowReceipeDescription -= ShowReceipeDescription;
        OnShowItemDescription -= ShowItemDescription;
        OnShowCropDescription -= ShowCropDescription;
        OnShowPurchaseItemDescription -= ShowPurchaseItemDescription;
        OnHideCanvas -= HideCanvas;
    }

    private void ShowReceipeDescription(int outputItemId)
    {
        Receipe receipe = ItemReceipesDatabase.GetReceipeById(outputItemId);
        requiredItemsPanel.gameObject.SetActive(true);
        uiRequiredItems[0].InitReceipeItems(receipe.reqId1, receipe.reqCount1);
        uiRequiredItems[1].InitReceipeItems(receipe.reqId2, receipe.reqCount2);
        uiRequiredItems[2].InitReceipeItems(receipe.reqId3, receipe.reqCount3);

        currentItem = ItemDatabase.GetItemById(outputItemId);
        itemNameHeaderText.text = currentItem.name;
        itemDescriptionText.text = currentItem.description;

        SetYieldTimeText(currentItem.yieldDurationInMins.MinsToFormattedDuration());
        SetBarnItemsCountText(SaveLoadManager.DoesItemExistsReturnCount(outputItemId));
        MovePanelToPosition();
    }

    private void ShowItemDescription(int itemId)
    {
        requiredItemsPanel.gameObject.SetActive(false);
        currentItem = ItemDatabase.GetItemById(itemId);
        itemNameHeaderText.text = currentItem.name;
        itemDescriptionText.text = currentItem.description;
        SetYieldTimeText("");
        SetBarnItemsCountText(SaveLoadManager.DoesItemExistsReturnCount(itemId));
        MovePanelToPosition();
    }

    private void ShowPurchaseItemDescription(int itemId, int costCount)
    {
        requiredItemsPanel.gameObject.SetActive(true);
        uiRequiredItems[0].InitCoinItems(costCount);
        uiRequiredItems[1].InitCoinItems(-1);
        uiRequiredItems[2].InitCoinItems(-1);

        currentItem = ItemDatabase.GetItemById(itemId);
        itemNameHeaderText.text = currentItem.name;
        itemDescriptionText.text = currentItem.description;

        SetYieldTimeText("");
        SetBarnItemsCountText(-1);
        MovePanelToPosition();
    }

    private void ShowCropDescription(int cropId)
    {
        Crops crop = CropsDatabase.GetCropById(cropId);
        requiredItemsPanel.gameObject.SetActive(true);
        uiRequiredItems[0].InitCoinItems(crop.coinCost);
        uiRequiredItems[1].InitCoinItems(-1);
        uiRequiredItems[2].InitCoinItems(-1);

        currentItem = ItemDatabase.GetItemById(cropId);
        itemNameHeaderText.text = currentItem.name;
        itemDescriptionText.text = currentItem.description;

        SetYieldTimeText(currentItem.yieldDurationInMins.MinsToFormattedDuration());
        barnItemsCountText.text = GameVariables.tmp_invIcon + SaveLoadManager.DoesItemExistsReturnCount(cropId);
        MovePanelToPosition();
    }

    private void SetYieldTimeText(string text)
    {
        if (String.IsNullOrEmpty(text))
        {
            yieldTimeText.gameObject.SetActive(false);
        }
        else
        {
            yieldTimeText.gameObject.SetActive(true);
            yieldTimeText.text = GameVariables.tmp_timeIcon + text;
        }
    }

    private void SetBarnItemsCountText(int count)
    {
        if (count < 0)
        {
            barnItemsCountText.gameObject.SetActive(false);
        }
        else
        {
            barnItemsCountText.gameObject.SetActive(true);
            barnItemsCountText.text = GameVariables.tmp_invIcon + count;
        }
    }

    private void MovePanelToPosition()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvas.transform as RectTransform,
           Input.mousePosition, mainCanvas.worldCamera, out movePos);
        mainPanel.position = mainCanvas.transform.TransformPoint(movePos);
        ToggleCanvas(true);
    }

    private void HideCanvas()
    {
        if (isCanvasVisible)
        {
            ToggleCanvas(false);
        }
    }

    private void ToggleCanvas(bool isVisible)
    {
        isCanvasVisible = isVisible;
        mainPanel.gameObject.SetActive(isVisible);
    }
}

[System.Serializable]
public enum DescriptionType
{
    Crop,
    Receipe,
    Item
}