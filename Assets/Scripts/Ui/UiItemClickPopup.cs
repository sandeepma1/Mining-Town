using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MiningTown.IO;
using System;

public class UiItemClickPopup : MonoBehaviour
{
    public static Action<ItemIdWithCount> OnShowSellItemMenu;
    public static Action<ItemIdWithCount> OnShowAddToBackpackMenu;
    public static Action<ItemIdWithCount> OnShowRemoveFromBackpackMenu;
    public static Action<ItemIdWithCount> OnShowDestroyItemMenu;
    public static Action<ItemIdWithCount> OnPopupMainButtonClicked;
    [SerializeField] private Button backgroundCloseButton;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemCountText;
    [SerializeField] private Slider countSlider;
    [SerializeField] private Button mainPopupButton;
    [SerializeField] private TextMeshProUGUI mainPopupButtonText;
    [SerializeField] private GameObject consumeDivider;
    [SerializeField] private Button consumeButton;
    [SerializeField] private TextMeshProUGUI consumeButtonText;
    private RectTransform mainPanelRect;
    private int maxItemCount;
    private Item currentItem;
    private int totalValueInCoins;
    private ItemIdWithCount currentFarmItem;
    private const float smallPanelSize = 600;
    private const float bigPanelSize = 800;

    private ItemPopupState ItemPopupState
    {
        get
        {
            return itemPopupState;
        }
        set
        {
            itemPopupState = value;
            print(itemPopupState);
            switch (itemPopupState)
            {
                case ItemPopupState.SellItem:
                    ToggleConsumeSection(true);
                    break;
                case ItemPopupState.AddToBackpack:
                    mainPopupButtonText.text = GameVariables.smsg_addToBackpack;
                    ToggleConsumeSection(false);
                    break;
                case ItemPopupState.RemoveFromBackpack:
                    mainPopupButtonText.text = GameVariables.smsg_addToBarn;
                    ToggleConsumeSection(false);
                    break;
                case ItemPopupState.DestroyItem:
                    mainPopupButtonText.text = GameVariables.smsg_destroyItems;
                    ToggleConsumeSection(false);
                    break;
                default:
                    break;
            }
        }
    }
    private ItemPopupState itemPopupState;

    private int CurrentItemCount
    {
        get
        {
            return currentItemCount;
        }
        set
        {
            currentItemCount = value;
            itemCountText.text = "x" + currentItemCount;
            totalValueInCoins = currentItem.sellValueInCoins * currentItemCount;
            countSlider.value = currentItemCount;
        }
    }
    private int currentItemCount;

    private void Start()
    {
        OnShowSellItemMenu += ShowSellItemMenu;
        OnShowAddToBackpackMenu += ShowAddToBackpackMenu;
        OnShowRemoveFromBackpackMenu += ShowRemoveFromBackpackMenu;
        OnShowDestroyItemMenu += ShowDestroyItemMenu;
        mainPanelRect = mainPanel.GetComponent<RectTransform>();
        backgroundCloseButton.onClick.AddListener(CloseMenu);
        minusButton.onClick.AddListener(OnMinusButtonClick);
        plusButton.onClick.AddListener(OnPlusButtonClick);
        countSlider.onValueChanged.AddListener(OnCountSliderValueChanged);
        mainPopupButton.onClick.AddListener(OnMainPopupButtonClick);
        consumeButton.onClick.AddListener(OnConsumeButtonClick);
        countSlider.minValue = 1;
        CloseMenu();
    }

    private void OnDestroy()
    {
        OnShowSellItemMenu -= ShowSellItemMenu;
        OnShowAddToBackpackMenu -= ShowAddToBackpackMenu;
        OnShowRemoveFromBackpackMenu -= ShowRemoveFromBackpackMenu;
        OnShowDestroyItemMenu -= ShowDestroyItemMenu;

        backgroundCloseButton.onClick.RemoveListener(CloseMenu);
        minusButton.onClick.RemoveListener(OnMinusButtonClick);
        plusButton.onClick.RemoveListener(OnPlusButtonClick);
        countSlider.onValueChanged.RemoveListener(OnCountSliderValueChanged);
        mainPopupButton.onClick.RemoveListener(OnMainPopupButtonClick);
        consumeButton.onClick.RemoveListener(OnConsumeButtonClick);
    }

    private void ShowSellItemMenu(ItemIdWithCount itemIdWithCount)
    {
        PopulatePopupMenu(itemIdWithCount);
        ItemPopupState = ItemPopupState.SellItem;
    }

    private void ShowAddToBackpackMenu(ItemIdWithCount itemIdWithCount)
    {
        PopulatePopupMenu(itemIdWithCount);
        ItemPopupState = ItemPopupState.AddToBackpack;
    }

    private void ShowRemoveFromBackpackMenu(ItemIdWithCount itemIdWithCount)
    {
        PopulatePopupMenu(itemIdWithCount);
        ItemPopupState = ItemPopupState.RemoveFromBackpack;
    }

    private void ShowDestroyItemMenu(ItemIdWithCount itemIdWithCount)
    {
        PopulatePopupMenu(itemIdWithCount);
        ItemPopupState = ItemPopupState.DestroyItem;
    }

    private void CloseMenu()
    {
        TogglePopupMenu(false);
    }

    private void TogglePopupMenu(bool isVisible)
    {
        mainPanel.gameObject.SetActive(isVisible);
        backgroundCloseButton.gameObject.SetActive(isVisible);
    }


    #region UI buttons and slider
    private void OnMinusButtonClick()
    {
        if (CurrentItemCount > 1)
        {
            CurrentItemCount--;
        }
    }

    private void OnPlusButtonClick()
    {
        if (CurrentItemCount < maxItemCount)
        {
            CurrentItemCount++;
        }
    }

    private void OnCountSliderValueChanged(float count)
    {
        CurrentItemCount = (int)count;
        if (itemPopupState == ItemPopupState.SellItem)
        {
            mainPopupButtonText.text = "Sell " + GameVariables.tmp_coinIcon + totalValueInCoins;
        }
    }

    private void OnMainPopupButtonClick()
    {
        OnPopupMainButtonClicked?.Invoke(new ItemIdWithCount(currentItem.itemId, CurrentItemCount));
        switch (ItemPopupState)
        {
            case ItemPopupState.SellItem:
                PlayerCurrencyManager.AddCoins(totalValueInCoins);
                UiFarmBarnInventory.OnReduceItemInInventory?.Invoke(currentItem.itemId, CurrentItemCount);
                break;
            case ItemPopupState.AddToBackpack:
                UiPlayerBackpackCanvas.OnAddItemToBackpack?.Invoke(currentItem.itemId, CurrentItemCount);
                UiFarmBarnInventory.OnReduceItemInInventory?.Invoke(currentItem.itemId, CurrentItemCount);
                break;
            case ItemPopupState.RemoveFromBackpack:
                UiPlayerBackpackCanvas.OnRemoveItemFromBackpack?.Invoke(currentItem.itemId, CurrentItemCount);
                UiFarmBarnInventory.OnAddUpdateItemToBarn?.Invoke(currentItem.itemId, CurrentItemCount);
                break;
            case ItemPopupState.DestroyItem:
                UiPlayerBackpackCanvas.OnRemoveItemFromBackpack?.Invoke(currentItem.itemId, CurrentItemCount);
                break;
            default:
                break;
        }
        CloseMenu();
    }

    private void PopulatePopupMenu(ItemIdWithCount itemIdWithCount)
    {
        if (itemIdWithCount.itemId <= 0)
        {
            return;
        }
        currentItem = ItemDatabase.GetItemById(itemIdWithCount.itemId);
        currentFarmItem = itemIdWithCount;
        maxItemCount = itemIdWithCount.itemCount;
        countSlider.maxValue = itemIdWithCount.itemCount;
        itemNameText.text = currentItem.name;
        descriptionText.text = currentItem.description;
        itemImage.sprite = AtlasBank.Instance.GetSpriteByName(currentItem.slug, AtlasType.UiItems);
        if (maxItemCount / 2 <= 0)
        {
            CurrentItemCount = 1;
        }
        else
        {
            CurrentItemCount = maxItemCount / 2;
        }
        TogglePopupMenu(true);
    }

    private void ToggleConsumeSection(bool isVisible)
    {
        consumeButton.gameObject.SetActive(isVisible);
        consumeDivider.gameObject.SetActive(isVisible);
        if (isVisible)
        {
            mainPanelRect.sizeDelta = new Vector2(mainPanelRect.sizeDelta.x, bigPanelSize);
        }
        else
        {
            mainPanelRect.sizeDelta = new Vector2(mainPanelRect.sizeDelta.x, smallPanelSize);
            return;
        }

        //Consume button
        if (currentItem.energyRestore == 0 && currentItem.healthRestore == 0)
        {
            consumeButton.interactable = false;
            consumeButtonText.text = "Cannot Consume";
        }
        else
        {
            consumeButton.interactable = true;
            string consumeText = "Consume  ";
            if (currentItem.energyRestore > 0)
            {
                consumeText += "+" + currentItem.energyRestore + GameVariables.tmp_energyIcon + "  ";
            }
            if (currentItem.healthRestore > 0)
            {
                consumeText += "+" + currentItem.healthRestore + GameVariables.tmp_heartIcon;
            }
            consumeButtonText.text = consumeText;
        }
    }

    private void OnConsumeButtonClick()
    {
        bool isConsumed = PlayerCurrencyManager.ConsumeItem(currentItem);
        if (isConsumed)
        {
            UiFarmBarnInventory.OnReduceItemInInventory?.Invoke(currentItem.itemId, 1);
            if (currentFarmItem.itemCount <= 0) { CloseMenu(); }
            else { ShowSellItemMenu(currentFarmItem); }
        }
    }
    #endregion
}

public enum ItemPopupState
{
    SellItem,
    AddToBackpack,
    RemoveFromBackpack,
    DestroyItem
}