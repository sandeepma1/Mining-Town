using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using MiningTown.IO;

public class UiStartAdventurePanel : MonoBehaviour
{
    public Action<int> OnLevelSelected;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private UiSlotItem uiSlotItemPrefab;
    [SerializeField] private Button levelsTabButton;
    [SerializeField] private Button farmItemsTabButton;
    private RectTransform levelsTabButtonRectTransform;
    private RectTransform farmItemsTabButtonRectTransform;

    [Header("Farm Barn Items")]
    [Space(10)]
    [SerializeField] private RectTransform farmItemsContent;
    private GridLayoutGroup farmItemsPanelGridLayoutGroup;
    private List<UiSlotItem> uiFarmItemsSlots = new List<UiSlotItem>();

    [Header("Level selection")]
    [Space(10)]
    [SerializeField] private UiLevelItem uiLevelItemPrefab;
    [SerializeField] private RectTransform levelsRectContent;
    private List<UiLevelItem> uiLevelItems = new List<UiLevelItem>();
    private GridLayoutGroup levelsPanelGridLayoutGroup;
    private int maxLevels = 10;
    private int levelDivider = 2;

    [Header("Backpack panel")]
    [Space(10)]
    [SerializeField] private ScrollRect backpackScrollRect;
    [SerializeField] private RectTransform backpackContentParent;

    [Header("Add Remove Panel")]
    [Space(10)]
    [SerializeField] private GameObject addRemovePanel;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescText;
    [SerializeField] private TextMeshProUGUI recoverAmountText;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemCountText;
    [SerializeField] private Slider countSlider;
    [SerializeField] private Button addRemoveButton;
    [SerializeField] private Button cancleAddRemovePanelButton;
    [SerializeField] private TextMeshProUGUI addRemoveButtonText;
    private List<ItemIdWithCount> itemsToRemoveFromBarn = new List<ItemIdWithCount>();
    private List<ItemIdWithCount> itemsToAddInInventory = new List<ItemIdWithCount>();

    private const float inventoryYPosition = -900;
    private int maxItemCount;
    ItemIdWithCount currentFarmItem;
    private Item currentItem;
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
            countSlider.value = currentItemCount;
            if (currentItem.energyRestore == 0 && currentItem.healthRestore == 0)
            {
                recoverAmountText.text = " ";
            }
            else
            {
                string consumeText = "Restore  ";
                if (currentItem.energyRestore > 0)
                {
                    consumeText += "+" + currentItem.energyRestore + GameVariables.tmp_energyIcon + "  ";
                }
                if (currentItem.healthRestore > 0)
                {
                    consumeText += "+" + currentItem.healthRestore + GameVariables.tmp_heartIcon;
                }
                recoverAmountText.text = consumeText;
            }
        }
    }
    private int currentItemCount;

    private void Start()
    {
        //Get all three GridLayoutGroups
        levelsPanelGridLayoutGroup = levelsRectContent.GetComponent<GridLayoutGroup>();
        farmItemsPanelGridLayoutGroup = farmItemsContent.GetComponent<GridLayoutGroup>();

        //Get all RectTransforms
        levelsTabButtonRectTransform = levelsTabButton.GetComponent<RectTransform>();
        farmItemsTabButtonRectTransform = farmItemsTabButton.GetComponent<RectTransform>();

        levelsTabButton.onClick.AddListener(OnLevelsTabClicked);
        farmItemsTabButton.onClick.AddListener(OnFarmItemsTabClicked);
        //Add Remove buttons
        minusButton.onClick.AddListener(OnMinusButtonClick);
        plusButton.onClick.AddListener(OnPlusButtonClick);
        countSlider.onValueChanged.AddListener(OnCountSliderValueChanged);
        addRemoveButton.onClick.AddListener(OnAddRemoveButtonClick);
        cancleAddRemovePanelButton.onClick.AddListener(OnCancleAddRemovePanelButtonClick);
        countSlider.minValue = 1;
        OnLevelsTabClicked();
        ToggleAddRemovePanel(false);
    }

    private void OnDestroy()
    {
        levelsTabButton.onClick.RemoveListener(OnLevelsTabClicked);
        farmItemsTabButton.onClick.RemoveListener(OnFarmItemsTabClicked);
        //Add Remove buttons
        minusButton.onClick.RemoveListener(OnMinusButtonClick);
        plusButton.onClick.RemoveListener(OnPlusButtonClick);
        countSlider.onValueChanged.RemoveListener(OnCountSliderValueChanged);
        addRemoveButton.onClick.RemoveListener(OnAddRemoveButtonClick);
        cancleAddRemovePanelButton.onClick.RemoveListener(OnCancleAddRemovePanelButtonClick);
    }

    public void ToggleAdventureMenu(bool isVisible)
    {
        ToggleAddRemovePanel(!isVisible);
        mainPanel.SetActive(isVisible);
        if (isVisible)
        {
            PopulateFarmBarnItems();
            PopulateLevelItems();
            UiPlayerBackpackCanvas.OnMoveBackpackPanel?.Invoke(backpackContentParent);
            backpackScrollRect.content = backpackContentParent.GetChild(0).GetComponent<RectTransform>();
            addRemovePanel.GetComponent<RectTransform>().SetAsLastSibling();
        }
    }


    #region Level Items Stuff
    private void PopulateLevelItems()
    {
        //First destroy all items
        foreach (var item in uiLevelItems)
        {
            Destroy(item.gameObject);
        }
        uiLevelItems = new List<UiLevelItem>();

        //Create new items
        for (int i = 1; i < maxLevels / levelDivider; i++)
        {
            UiLevelItem uiLevelItem = Instantiate(uiLevelItemPrefab, levelsRectContent);
            uiLevelItem.Init(i * levelDivider);
            uiLevelItem.OnButtonClick += OnLevelButtonClicked;
            uiLevelItems.Add(uiLevelItem);
        }
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(FitLevelsGridCell());
        }
    }

    private void OnLevelButtonClicked(int levelId)
    {
        OnLevelSelected?.Invoke(levelId);
    }

    private IEnumerator FitLevelsGridCell()
    {
        yield return new WaitForEndOfFrame();
        levelsPanelGridLayoutGroup.FitGridCell();
    }
    #endregion


    #region Farm Barn Items Stuff
    private void PopulateFarmBarnItems()
    {
        //First destroy all items
        foreach (var item in uiFarmItemsSlots)
        {
            Destroy(item.gameObject);
        }
        uiFarmItemsSlots = new List<UiSlotItem>();

        //Create new items
        for (int i = 0; i < SaveLoadManager.saveData.barnItems.Count; i++)
        {
            UiSlotItem uiSlotItem = Instantiate(uiSlotItemPrefab, farmItemsContent);
            uiSlotItem.InitItemData(i, SaveLoadManager.saveData.barnItems[i]);
            uiSlotItem.OnSlotClicked += OnSlotItemsClicked;
            uiFarmItemsSlots.Add(uiSlotItem);
        }
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(FitFarmBarnItemsGridCell());
        }
    }

    private void OnSlotItemsClicked(int index, int itemId, int itemCount)
    {
        ShowAddRemovePanel(new ItemIdWithCount(itemId, itemCount), false);
    }

    private IEnumerator FitFarmBarnItemsGridCell()
    {
        yield return new WaitForEndOfFrame();
        farmItemsPanelGridLayoutGroup.FitGridCell();
    }
    #endregion


    #region Add Remove Popup
    private void ToggleAddRemovePanel(bool isVisible)
    {
        addRemovePanel.SetActive(isVisible);
    }

    private void ShowAddRemovePanel(ItemIdWithCount farmItem, bool isClickedFromInventory)
    {
        currentItem = ItemDatabase.GetItemById(farmItem.itemId);
        currentFarmItem = farmItem;
        maxItemCount = farmItem.itemCount;
        countSlider.maxValue = farmItem.itemCount;
        itemNameText.text = currentItem.name;
        itemDescText.text = currentItem.description;
        itemImage.sprite = AtlasBank.Instance.GetSpriteByName(currentItem.slug, AtlasType.UiItems);

        CurrentItemCount = maxItemCount / 2 <= 0 ? 1 : maxItemCount / 2;
        addRemoveButtonText.text = isClickedFromInventory ? "Remove From Inventory" : "Add to Inventory";
        ToggleAddRemovePanel(true);
    }

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
    }

    private void OnAddRemoveButtonClick()
    {
        print("Add to Inventory");
        ToggleAddRemovePanel(false);
    }

    private void OnCancleAddRemovePanelButtonClick()
    {
        ToggleAddRemovePanel(false);
    }
    #endregion


    #region Buttons Tabs selection
    private void OnLevelsTabClicked()
    {
        levelsTabButtonRectTransform.SetAsLastSibling();
        farmItemsTabButtonRectTransform.SetAsFirstSibling();
        scrollRect.content = levelsRectContent;
        levelsRectContent.gameObject.SetActive(true);
        farmItemsContent.gameObject.SetActive(false);
    }

    private void OnFarmItemsTabClicked()
    {
        farmItemsTabButtonRectTransform.SetAsLastSibling();
        levelsTabButtonRectTransform.SetAsFirstSibling();
        scrollRect.content = farmItemsContent;
        farmItemsContent.gameObject.SetActive(true);
        levelsRectContent.gameObject.SetActive(false);
    }
    #endregion
}