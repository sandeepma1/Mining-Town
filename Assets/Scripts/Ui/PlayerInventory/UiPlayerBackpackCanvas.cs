using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using MiningTown.IO;

public class UiPlayerBackpackCanvas : UiBasicCanvasWindowBase
{
    public static UiPlayerBackpackCanvas Instance = null;
    public static Action<int, int> OnAddItemToBackpack;
    public static Action<int, int> OnRemoveItemFromBackpack;
    public static Action<Transform> OnMoveBackpackPanel;
    public static Action<int, int, int> OnBackpackSlotClicked;
    [Header("Child Class")]
    [Space(10)]
    [SerializeField] private RectTransform gridContentParent;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private Button showInvButton;
    [SerializeField] private UiSlotItem uiSlotItemPrefab;
    [SerializeField] private Button addNewSlotButton;
    [SerializeField] private TextMeshProUGUI addNewSlotButtonText;
    [SerializeField] private RectTransform slotSelectorRect;
    [Header("Bottom Panel")]
    [Space(10)]
    [SerializeField] private Button sortButton;
    [SerializeField] private Button consumeButton;
    [SerializeField] private Button addToQuickslotButton;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemRestoreText;
    [SerializeField] private Button deleteItemButton;
    private List<UiSlotItem> uiSlotItems = new List<UiSlotItem>();
    private RectTransform addNewSlotButtonRect;
    private RectTransform mainPanelRect;
    private RectTransform gridRect;
    private int currentSelectedSlotId = 0;
    private int currentSelectedItemId = 0;
    private Item currentSelectedItem;
    private List<int> slotIdsWithItemId = new List<int>();
    private List<int> emptySlotIds = new List<int>();
    public bool isBackpackSelected;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    private void Start()
    {
        SceneLoader.OnSceneChanged += OnSceneChanged;
        OnMoveBackpackPanel += MoveBackpackPanel;
        OnAddItemToBackpack += AddItemToBackpackEventHandler;
        OnRemoveItemFromBackpack += RemoveItemFromBackpackEventHandler;
        SaveLoadTrigger.OnSaveTrigger += SaveGameData;

        showInvButton.onClick.AddListener(OnShowBackpackButtonPressed);
        addNewSlotButton.onClick.AddListener(OnAddNewSlotButtonPressed);
        sortButton.onClick.AddListener(OnSortButtonPressed);
        consumeButton.onClick.AddListener(OnConsumeButtonPressed);
        addToQuickslotButton.onClick.AddListener(OnAddToQuickslotButtonPressed);
        deleteItemButton.onClick.AddListener(OnDeleteItemButtonPressed);

        mainPanelRect = mainPanel.GetComponent<RectTransform>();
        gridRect = gridLayoutGroup.GetComponent<RectTransform>();
        addNewSlotButtonRect = addNewSlotButton.GetComponent<RectTransform>();
        InitLoadBackpack();
        StartCoroutine(FitGridCell());
    }

    private void OnDestroy()
    {
        SceneLoader.OnSceneChanged -= OnSceneChanged;
        OnMoveBackpackPanel -= MoveBackpackPanel;
        OnAddItemToBackpack -= AddItemToBackpackEventHandler;
        OnRemoveItemFromBackpack -= RemoveItemFromBackpackEventHandler;
        SaveLoadTrigger.OnSaveTrigger -= SaveGameData;

        showInvButton.onClick.RemoveListener(OnShowBackpackButtonPressed);
        addNewSlotButton.onClick.RemoveListener(OnAddNewSlotButtonPressed);
        sortButton.onClick.RemoveListener(OnSortButtonPressed);
        consumeButton.onClick.RemoveListener(OnConsumeButtonPressed);
        addToQuickslotButton.onClick.RemoveListener(OnAddToQuickslotButtonPressed);
        deleteItemButton.onClick.RemoveListener(OnDeleteItemButtonPressed);
    }


    #region All init load backpack functions
    private void InitLoadBackpack()
    {
        //Create Empty slots first
        for (int i = 0; i < SaveLoadManager.saveData.backpackData.backpackSlotsCount; i++)
        {
            CreateEmptySlot(i);
        }
        //Check if backpack is all full purchased
        if (SaveLoadManager.saveData.backpackData.backpackSlotsCount >= GameVariables.bp_MaxSlots)
        {
            addNewSlotButton.gameObject.SetActive(false);
        }
        //Load backpack saved items
        foreach (KeyValuePair<int, int> item in SaveLoadManager.backpackItemsDict)
        {
            AddUpdateItem(new ItemIdWithCount(item.Key, item.Value), false);
        }
    }

    private void CreateEmptySlot(int index)
    {
        UiSlotItem uiSlotItem = Instantiate(uiSlotItemPrefab, gridLayoutGroup.transform);
        uiSlotItem.InitEmptySlots(index);
        uiSlotItem.OnSlotClicked += OnSlotClicked;
        uiSlotItems.Add(uiSlotItem);
        addNewSlotButtonRect.SetAsLastSibling();
        addNewSlotButtonText.text = (SaveLoadManager.saveData.backpackData.backpackSlotsCount + 1) * GameVariables.bp_newSlotRateMultiplier + GameVariables.tmp_gemIcon;
    }

    #endregion


    #region Add Remove Update items in Inventory 
    //TODO: Refactor all below functions that uses this below logic as it is calculated all the times
    public bool IfBackpackHasEmptySpace(ItemIdWithCount item)
    {
        ItemIdWithCount itemToAdd = new ItemIdWithCount(item.itemId, item.itemCount);
        //Get all occurance of that itemId
        slotIdsWithItemId = new List<int>();
        emptySlotIds = new List<int>();
        for (int i = 0; i < uiSlotItems.Count; i++)
        {
            if (uiSlotItems[i].DoesSlotHasSpace(itemToAdd.itemId))
            {
                slotIdsWithItemId.Add(i);
            }
            if (uiSlotItems[i].GetItemId() == 0) // this is empty slot
            {
                emptySlotIds.Add(i);
            }
        }

        if (slotIdsWithItemId.Count == 0 && emptySlotIds.Count == 0)
        {
            //print("Cannot add item" + ItemDatabase.GetItemNameById(itemToAdd.itemId) + " + " + itemToAdd.itemCount);
            return false;
        }
        return true;
    }

    public int IfBackpackHasEmptySpaceSendCount(int itemId)
    {
        int emptySpaces = 0;
        //Get all occurance of that itemId
        slotIdsWithItemId = new List<int>();
        emptySlotIds = new List<int>();
        for (int i = 0; i < uiSlotItems.Count; i++)
        {
            if (uiSlotItems[i].DoesSlotHasSpace(itemId))
            {
                slotIdsWithItemId.Add(i);
            }
            if (uiSlotItems[i].GetItemId() == 0) // this is empty slot
            {
                emptySlotIds.Add(i);
            }
        }

        int maxSlotSize = SaveLoadManager.saveData.backpackData.maxSlotSize;
        if (emptySlotIds.Count > 0)
        {
            emptySpaces += emptySlotIds.Count * maxSlotSize;
        }
        for (int i = 0; i < slotIdsWithItemId.Count; i++)
        {
            int canAddCount = maxSlotSize - uiSlotItems[slotIdsWithItemId[i]].GetItemCount();
            emptySpaces += canAddCount;
        }
        return emptySpaces;
    }

    private void AddUpdateItem(ItemIdWithCount item, bool saveData = true)
    {
        int maxSlotSize = SaveLoadManager.saveData.backpackData.maxSlotSize;
        ItemIdWithCount itemToAdd = new ItemIdWithCount(item.itemId, item.itemCount);
        //Get all occurance of that itemId
        slotIdsWithItemId = new List<int>();
        emptySlotIds = new List<int>();
        for (int i = 0; i < uiSlotItems.Count; i++)
        {
            if (uiSlotItems[i].DoesSlotHasSpace(itemToAdd.itemId))
            {
                slotIdsWithItemId.Add(i);
            }
            if (uiSlotItems[i].GetItemId() == 0) // this is empty slot
            {
                emptySlotIds.Add(i);
            }
        }

        if (slotIdsWithItemId.Count == 0 && emptySlotIds.Count == 0)
        {
            print("Cannot add item" + ItemDatabase.GetItemNameById(itemToAdd.itemId) + " + " + itemToAdd.itemCount);
            return;
        }

        //Fill the slot if there any item already exists
        for (int i = 0; i < slotIdsWithItemId.Count; i++)
        {
            int canAddCount = maxSlotSize - uiSlotItems[slotIdsWithItemId[i]].GetItemCount();
            if (itemToAdd.itemCount >= canAddCount)
            {
                itemToAdd.itemCount -= canAddCount;
                uiSlotItems[slotIdsWithItemId[i]].OnUpdateItemCount(canAddCount);
                if (saveData)
                {
                    AddUpdateBackpackItemInDict(itemToAdd.itemId, canAddCount);
                }
            }
            else
            {
                uiSlotItems[slotIdsWithItemId[i]].OnUpdateItemCount(itemToAdd.itemCount);
                if (saveData)
                {
                    AddUpdateBackpackItemInDict(itemToAdd.itemId, itemToAdd.itemCount);
                }
                itemToAdd.itemCount = 0;
                return;
            }
        }

        //Add new item in slot
        for (int i = 0; i < emptySlotIds.Count; i++)
        {
            if (itemToAdd.itemCount > 0)
            {
                if (itemToAdd.itemCount >= maxSlotSize)
                {
                    uiSlotItems[emptySlotIds[i]].AddNewItemToSlot(new ItemIdWithCount(itemToAdd.itemId, maxSlotSize));
                    if (saveData)
                    {
                        AddUpdateBackpackItemInDict(itemToAdd.itemId, maxSlotSize);
                    }
                    itemToAdd.itemCount -= maxSlotSize;
                }
                else
                {
                    uiSlotItems[emptySlotIds[i]].AddNewItemToSlot(itemToAdd);
                    if (saveData)
                    {
                        AddUpdateBackpackItemInDict(itemToAdd.itemId, itemToAdd.itemCount);
                    }
                    itemToAdd.itemCount = 0;
                    return;
                }
            }
            else
            {
                break;
            }
        }

        if (itemToAdd.itemCount > 0)
        {
            print("Cannot add item" + ItemDatabase.GetItemNameById(itemToAdd.itemId) + " + " + itemToAdd.itemCount);
        }
    }

    private void RemoveReduceItem(int itemId, int itemCount)
    {
        uiSlotItems[currentSelectedSlotId].ReduceRemoveToSlot(itemCount);
        ReduceRemoveBackpackItemInDict(itemCount);
    }

    private void RemoveReduceItem(int count)
    {
        uiSlotItems[currentSelectedSlotId].ReduceRemoveToSlot(count);
        ReduceRemoveBackpackItemInDict(count);
    }
    #endregion


    #region UI functions
    private void OnRefreshSlotDetails()
    {
        OnSlotClicked(currentSelectedSlotId, currentSelectedItemId, uiSlotItems[currentSelectedSlotId].GetItemCount());
    }

    private void OnSlotClicked(int slotId, int itemId, int itemCount)
    {
        OnBackpackSlotClicked?.Invoke(slotId, itemId, itemCount);
        currentSelectedSlotId = slotId;
        currentSelectedItemId = uiSlotItems[currentSelectedSlotId].GetItemId();
        slotSelectorRect.transform.SetParent(uiSlotItems[currentSelectedSlotId].transform);
        slotSelectorRect.anchoredPosition = Vector2.zero;
        if (isBackpackSelected)
        {
            PopulateBottomInfo();
        }
        else
        {
            UiItemClickPopup.OnShowRemoveFromBackpackMenu(new ItemIdWithCount(itemId, itemCount));
        }
    }

    private void OnShowBackpackButtonPressed()
    {
        ToggleCanvas(true);
    }

    private void OnAddNewSlotButtonPressed()
    {
        int newSlotPrice = SaveLoadManager.saveData.backpackData.backpackSlotsCount * GameVariables.bp_newSlotRateMultiplier;
        if (PlayerCurrencyManager.HaveGems(newSlotPrice))
        {
            if (SaveLoadManager.saveData.backpackData.backpackSlotsCount <= GameVariables.bp_MaxSlots)
            {
                PlayerCurrencyManager.ReduceGems(newSlotPrice);
                SaveLoadManager.saveData.backpackData.backpackSlotsCount++;
                CreateEmptySlot(SaveLoadManager.saveData.backpackData.backpackSlotsCount - 1);
                StartCoroutine(FitMainPanel());
            }
            else
            {
                addNewSlotButton.gameObject.SetActive(false);
            }
        }
    }

    protected override void ToggleCanvas(bool isVisible)
    {
        base.ToggleCanvas(isVisible);
        isBackpackSelected = isVisible;
        showInvButton.gameObject.SetActive(!isVisible);
        if (isVisible && uiSlotItems.Count > 0)
        {
            uiSlotItems[currentSelectedSlotId].OnButtonClicked();
            MoveBackpackPanel(gridContentParent);
            StartCoroutine(FitGridCell());
            StartCoroutine(FitMainPanel());
        }
    }

    private IEnumerator FitGridCell()
    {
        yield return new WaitForEndOfFrame();
        float cellSize = gridLayoutGroup.FitGridCell();
        slotSelectorRect.sizeDelta = new Vector2(cellSize, cellSize);
        StartCoroutine(FitMainPanel());
    }

    private IEnumerator FitMainPanel()
    {
        //Bottom Panel = 125
        //GridPanel Y pos = 100
        //+150 Y margin
        yield return new WaitForEndOfFrame();
        // print(gridLayoutGroup.GetComponent<RectTransform>().sizeDelta.y);
        float totalHeight = 125 + gridLayoutGroup.GetComponent<RectTransform>().sizeDelta.y + 150;
        mainPanelRect.sizeDelta = new Vector2(mainPanelRect.sizeDelta.x, totalHeight);
        //print(totalHeight);
    }
    #endregion


    #region Bottom panel buttons and stuff
    private void OnSortButtonPressed()
    {
        for (int i = 0; i < uiSlotItems.Count; i++)
        {
            uiSlotItems[i].ResetSlot();
        }
        foreach (KeyValuePair<int, int> item in SaveLoadManager.backpackItemsDict)
        {
            AddUpdateItem(new ItemIdWithCount(item.Key, item.Value), false);
        }
        OnRefreshSlotDetails();
    }

    private void OnConsumeButtonPressed()
    {
        bool isConsumed = PlayerCurrencyManager.ConsumeItem(currentSelectedItem);
        if (isConsumed)
        {
            RemoveReduceItem(1);
            uiSlotItems[currentSelectedSlotId].OnButtonClicked();
        }
    }

    private void OnAddToQuickslotButtonPressed()
    {
        throw new NotImplementedException();
    }

    private void OnDeleteItemButtonPressed()
    {
        int itemCount = uiSlotItems[currentSelectedSlotId].GetItemCount();
        UiItemClickPopup.OnShowDestroyItemMenu?.Invoke(new ItemIdWithCount(currentSelectedItemId, itemCount));
    }

    private void PopulateBottomInfo()
    {
        //Check if slot has item or not
        if (currentSelectedItemId > 0) // Has Item
        {
            deleteItemButton.interactable = true;
            consumeButton.interactable = true;
            currentSelectedItem = ItemDatabase.GetItemById(currentSelectedItemId);
            itemNameText.text = currentSelectedItem.name;
            itemDescriptionText.text = currentSelectedItem.description;
        }
        else //Slot empty
        {
            currentSelectedItem = null;
            deleteItemButton.interactable = false;
            consumeButton.interactable = false;
            itemNameText.text = "";
            itemDescriptionText.text = "";
            itemRestoreText.text = "";
            return;
        }

        //Check if item is consumeable or not
        if (currentSelectedItem.energyRestore == 0 && currentSelectedItem.healthRestore == 0)
        {
            consumeButton.interactable = false;
            itemRestoreText.text = GameVariables.smsg_cannotConsume;
        }
        else
        {
            consumeButton.interactable = true;
            string consumeText = "";
            if (currentSelectedItem.energyRestore > 0)
            {
                consumeText += "+" + currentSelectedItem.energyRestore + GameVariables.tmp_energyIcon + "  ";
            }
            if (currentSelectedItem.healthRestore > 0)
            {
                consumeText += "+" + currentSelectedItem.healthRestore + GameVariables.tmp_heartIcon;
            }
            itemRestoreText.text = consumeText;
        }
    }
    #endregion


    #region Helper functions
    private void MoveBackpackPanel(Transform parent)
    {
        gridRect.SetParent(parent);
        gridRect.anchoredPosition = Vector2.zero;
    }

    private void OnSceneChanged(Scenes obj)
    {
        MoveBackpackPanel(gridContentParent);
    }

    #region Other scripts can add remove items in backpack
    private void AddItemToBackpackEventHandler(int itemId, int itemCount)
    {
        AddUpdateItem(new ItemIdWithCount(itemId, itemCount));
    }

    private void RemoveItemFromBackpackEventHandler(int itemId, int itemCount)
    {
        RemoveReduceItem(itemId, itemCount);
    }
    #endregion

    #endregion


    #region Player Backpack Dictionary operations
    private void AddUpdateBackpackItemInDict(int itemId, int count)
    {
        if (SaveLoadManager.backpackItemsDict.ContainsKey(itemId)) //Update count
        {
            SaveLoadManager.backpackItemsDict[itemId] += count;
        }
        else //Create new item
        {
            SaveLoadManager.backpackItemsDict.Add(itemId, count);
        }
    }

    private void ReduceRemoveBackpackItemInDict(int count)
    {
        if (SaveLoadManager.backpackItemsDict.ContainsKey(currentSelectedItemId)) //Update count
        {
            SaveLoadManager.backpackItemsDict[currentSelectedItemId] -= count;
            if (SaveLoadManager.backpackItemsDict[currentSelectedItemId] <= 0)
            {
                SaveLoadManager.backpackItemsDict.Remove(currentSelectedItemId);
            }
        }
    }
    #endregion


    #region Save Load Game on Trigger
    private void SaveGameData()
    {
        List<ItemIdWithCount> backpackItemsData = new List<ItemIdWithCount>();
        foreach (KeyValuePair<int, int> item in SaveLoadManager.backpackItemsDict)
        {
            backpackItemsData.Add(new ItemIdWithCount(item.Key, item.Value));
        }
        SaveLoadManager.SavePlayerBackpackItems(backpackItemsData);
    }
    #endregion
}