using System;
using System.Collections;
using System.Collections.Generic;
using MiningTown.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UiFarmBarnInventory : UiBasicCanvasWindowBase
{
    public static Action<int, int> OnAddUpdateItemToBarn;
    public static Action<int> OnRemoveItemFromInventory;
    public static Action<int, int> OnReduceItemInInventory;
    public static Action OnShowBarnInventory;
    [Header("Child Class")]
    [Space(10)]
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private UiTabButton[] uiTabButtons;
    [SerializeField] private Transform contentParent;
    [SerializeField] private UiBarnItem uiBarnItemPrefab;
    [SerializeField] private Button showBackpackButton;
    [SerializeField] private TextMeshProUGUI showBackpackButtonText;
    [SerializeField] private ScrollRect backpackScrollView;
    [SerializeField] private RectTransform backpackContentPanel;
    private bool isBackpackVisible = false;
    private Dictionary<int, UiBarnItem> uiBarnItemsDict = new Dictionary<int, UiBarnItem>(); //itemID, UiBarnItem
    //private Dictionary<int, int> barnItemsDict = new Dictionary<int, int>(); //itemID, count
    private int lastClickedTabId;
    private RectTransform mainPanelRect;
    private const float backpackShowSize = 1600;
    private const float backpackHideSize = 1200;

    private void Start()
    {
        OnLoadFarmItemsData();
        mainPanelRect = mainPanel.GetComponent<RectTransform>();
        SaveLoadTrigger.OnSaveTrigger += SaveGameData;
        OnAddUpdateItemToBarn += AddUpdateItemToBarn;
        OnRemoveItemFromInventory += RemoveFarmBarnItem;
        OnReduceItemInInventory += ReduceItemInInventory;
        OnShowBarnInventory += ShowBarnInventory;
        UiAllScreenButtonsCanvas.OnInventoryButtonClick += ShowBarnInventory;
        showBackpackButton.onClick.AddListener(OnShowBackpackButtonClicked);
        InitTabButtons();
        StartCoroutine(FitGridLayout());
        ToggleCanvas(false);
    }

    private void OnDestroy()
    {
        SaveLoadTrigger.OnSaveTrigger -= SaveGameData;
        OnAddUpdateItemToBarn -= AddUpdateItemToBarn;
        OnRemoveItemFromInventory -= RemoveFarmBarnItem;
        OnReduceItemInInventory -= ReduceItemInInventory;
        OnShowBarnInventory -= ShowBarnInventory;
        UiAllScreenButtonsCanvas.OnInventoryButtonClick -= ShowBarnInventory;
        showBackpackButton.onClick.RemoveListener(OnShowBackpackButtonClicked);
    }

    private void InitTabButtons()
    {
        for (int i = 0; i < uiTabButtons.Length; i++)
        {
            uiTabButtons[i].Init(i);
            uiTabButtons[i].OnTabButtonClicked += OnTabButtonClicked;
        }
    }

    private IEnumerator FitGridLayout()
    {
        yield return new WaitForEndOfFrame();
        gridLayoutGroup.FitGridCell();
    }

    private void CreateNewUiBarnItem(ItemIdWithCount farmItemsData)
    {
        UiBarnItem uiBarnItem = Instantiate(uiBarnItemPrefab, contentParent);
        uiBarnItem.Init(farmItemsData);
        uiBarnItem.OnBarnItemButtonClick += OnBarnItemButtonClick;
        uiBarnItemsDict.Add(farmItemsData.itemId, uiBarnItem);
    }


    #region Add update remove delete items 
    private void AddUpdateItemToBarn(int itemId, int count)
    {
        if (uiBarnItemsDict.ContainsKey(itemId)) //Update count
        {
            uiBarnItemsDict[itemId].ItemCount += count;
        }
        else
        {
            CreateNewUiBarnItem(new ItemIdWithCount(itemId, count));
        }
    }

    private void RemoveFarmBarnItem(int itemId)
    {
        if (uiBarnItemsDict.ContainsKey(itemId))
        {
            Destroy(uiBarnItemsDict[itemId].gameObject);
            uiBarnItemsDict.Remove(itemId);
        }
    }

    private void ReduceItemInInventory(int itemId, int count)
    {
        if (uiBarnItemsDict.ContainsKey(itemId))
        {
            uiBarnItemsDict[itemId].ItemCount -= count;
            if (uiBarnItemsDict[itemId].ItemCount <= 0)
            {
                RemoveFarmBarnItem(itemId);
            }
        }
    }
    #endregion


    #region UI functions
    protected override void ToggleCanvas(bool isVisible)
    {
        base.ToggleCanvas(isVisible);
        ToggleBackpack();
    }

    private void OnTabButtonClicked(int tabId)
    {
        if (lastClickedTabId == tabId)
        {
            return;
        }
        //Change Tab colors
        uiTabButtons[lastClickedTabId].ToggleButtonPressed(ColorConstants.UnselectedTabColor, false);
        uiTabButtons[tabId].ToggleButtonPressed(ColorConstants.SelectedTabColor, true);

        if (tabId == 0) //show all items
        {
            foreach (KeyValuePair<int, UiBarnItem> item in uiBarnItemsDict)
            {
                item.Value.gameObject.SetActive(true);
            }
        }
        else //Show filtered items
        {
            //First disable all items
            foreach (KeyValuePair<int, UiBarnItem> item in uiBarnItemsDict)
            {
                item.Value.gameObject.SetActive(false);
            }
            //Enable only those objects by tab id
            List<int> itemIds = ItemDatabase.GetAllItemsByCategoryId(tabId);
            for (int i = 0; i < itemIds.Count; i++)
            {
                if (uiBarnItemsDict.ContainsKey(itemIds[i]))
                {
                    uiBarnItemsDict[itemIds[i]].gameObject.SetActive(true);
                }
            }
        }
        lastClickedTabId = tabId;
    }

    private void ShowBarnInventory()
    {
        ToggleCanvas(true);
    }

    private void OnBarnItemButtonClick(ItemIdWithCount farmItemsData)
    {
        if (isBackpackVisible)
        {
            //Check backpack if it has space to add max items 
            int backpackEmptySpacesCount = UiPlayerBackpackCanvas.Instance.IfBackpackHasEmptySpaceSendCount(farmItemsData.itemId);
            if (backpackEmptySpacesCount > 0)
            {
                if (farmItemsData.itemCount >= backpackEmptySpacesCount)
                {
                    UiItemClickPopup.OnShowAddToBackpackMenu?.Invoke(new ItemIdWithCount(farmItemsData.itemId, backpackEmptySpacesCount));
                }
                else
                {
                    UiItemClickPopup.OnShowAddToBackpackMenu?.Invoke(farmItemsData);
                }
            }
            else
            {
                UiFeedbackPopupCanvas.OnShowFeedbackPopup?.Invoke(GameVariables.msg_lowOnBackpackSpace);
            }

        }
        else
        {
            UiItemClickPopup.OnShowSellItemMenu?.Invoke(farmItemsData);
        }
    }

    private void OnShowBackpackButtonClicked()
    {
        isBackpackVisible = !isBackpackVisible;
        ToggleBackpack();
    }

    private void ToggleBackpack()
    {
        if (mainPanelRect == null)
        {
            mainPanelRect = mainPanel.GetComponent<RectTransform>();
        }
        if (isBackpackVisible)
        {
            UiPlayerBackpackCanvas.OnMoveBackpackPanel?.Invoke(backpackContentPanel);
            showBackpackButtonText.text = "Hide Backpack";
            mainPanelRect.sizeDelta = new Vector2(mainPanelRect.sizeDelta.x, backpackShowSize);
            backpackScrollView.gameObject.SetActive(true);
            backpackScrollView.content = backpackContentPanel.GetChild(0).GetComponent<RectTransform>();
        }
        else
        {
            showBackpackButtonText.text = "Show Backpack";
            mainPanelRect.sizeDelta = new Vector2(mainPanelRect.sizeDelta.x, backpackHideSize);
            backpackScrollView.gameObject.SetActive(false);
        }
    }
    #endregion


    #region Save Load Barn items
    private void OnLoadFarmItemsData()
    {
        List<ItemIdWithCount> farmItemsDatas = SaveLoadManager.GetAllFarmItems();
        for (int i = 0; i < farmItemsDatas.Count; i++)
        {
            CreateNewUiBarnItem(farmItemsDatas[i]);
        }
    }

    private void SaveGameData()
    {
        print("Saving farm bar data");
        List<ItemIdWithCount> farmItemsData = new List<ItemIdWithCount>();
        foreach (KeyValuePair<int, UiBarnItem> item in uiBarnItemsDict)
        {
            farmItemsData.Add(new ItemIdWithCount(item.Key, item.Value.ItemCount));
        }
        SaveLoadManager.SaveFarmBarnItems(farmItemsData);
    }
    #endregion
}