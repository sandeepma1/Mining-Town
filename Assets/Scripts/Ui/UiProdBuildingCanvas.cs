using System;
using System.Collections.Generic;
using MiningTown.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiProdBuildingCanvas : UiBasicCanvasWindowBase
{
    public static Action<int> OnItemDroppedOnProdBuilding;
    public static Action OnUpdateUiItems;
    public static Action<ProdBuildingData, Action> OnShowCanvas;
    public static Action OnHideCanvas;

    [Header("Child Class")]
    [Space(10)]
    [SerializeField] private Image cursorImage;
    [SerializeField] private ScrollRect parentScrollRect;
    [SerializeField] private UiDropItem uiDropItem;
    [Header("Scroll prefabs content")]
    [Space(10)]
    [SerializeField] private UiQueueBox uiQueueBoxPrefab;
    [SerializeField] private Transform queueBoxContentParent;
    [SerializeField] private UiDragItem uiDraggableItemPrefab;
    [SerializeField] private Transform draggableItemContentParent;
    [SerializeField] private TextMeshProUGUI sourceNameText;
    [Header("First Queue Box")]
    [Space(10)]
    [SerializeField] private Transform dropTransform;
    [SerializeField] private Transform progressGemPanel;
    [SerializeField] private RectTransform progressBarRect;
    [SerializeField] private TextMeshProUGUI timeRemainingText;
    [SerializeField] private Button gemsCompleteProdButton;
    [SerializeField] private TextMeshProUGUI gemsCompleteProdCountText;
    [Header("Last Queue Box")]
    [Space(10)]
    [SerializeField] private RectTransform lastQueueLockedBox;
    [SerializeField] private Button gemsBuyNewBoxButton;
    [SerializeField] private TextMeshProUGUI gemsBuyNewBoxText;
    private RectTransform dropWindow;
    private List<UiDragItem> uiDragItems = new List<UiDragItem>();
    private List<UiQueueBox> uiQueueBoxes = new List<UiQueueBox>();
    public ProdBuildingData prodBuildingData;
    private int currentDraggedItemId = -1;
    private float progressBarWidth;
    private Vector2 progressGemPanelPosition = new Vector2(0, -15);
    private TimeSpan timeSpan;
    private DateTime currentYieldDateTime;
    private float currentYieldTick;
    private Item currentItem;
    private float gemCostInterval;
    private int gemsNeededToCompelete;
    private Action CompleteByGemsProdBuilding;
    private bool isCanvasVisible;
    private bool areAnyQueueItems;
    private int[] unlockQueueBoxGemsPrice = new int[] { 0, 3, 6, 9, 12, 15, 18, 21 };

    private void Start()
    {
        dropWindow = uiDropItem.GetComponent<RectTransform>();
        OnShowCanvas += ShowCanvas;
        OnHideCanvas += HideCanvas;
        OnUpdateUiItems += UpdateUiItems;
        uiDropItem.OnDropItem += UiDropItemHandler;
        progressBarWidth = progressBarRect.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        gemsCompleteProdButton.onClick.AddListener(OnItemCompleteGemButtonClicked);
        gemsBuyNewBoxButton.onClick.AddListener(OnGemsBuyNewQueueBoxButtonClicked);
    }

    private void OnDestroy()
    {
        OnShowCanvas -= ShowCanvas;
        OnHideCanvas -= HideCanvas;
        OnUpdateUiItems -= UpdateUiItems;
        uiDropItem.OnDropItem -= UiDropItemHandler;
        gemsCompleteProdButton.onClick.RemoveListener(OnItemCompleteGemButtonClicked);
        gemsBuyNewBoxButton.onClick.RemoveListener(OnGemsBuyNewQueueBoxButtonClicked);
    }

    private void Update()
    {
        if (isCanvasVisible && areAnyQueueItems)
        {
            UpdateFirstQueueBox();
        }
    }

    private void UpdateFirstQueueBox()
    {
        if (prodBuildingData == null || uiQueueBoxes == null || uiQueueBoxes.Count == 0)
        {
            Debug.LogError("prodBuildingData, uiQueueBoxes are null or empty");
            return;
        }
        //Populate First Queue box
        if (prodBuildingData.queuedItemIds.Count >= 1)
        {
            timeSpan = currentYieldDateTime.Subtract(DateTime.UtcNow);
            if (timeSpan.TotalSeconds <= 0)
            {
                UpdateOtherQueueBoxes();
            }
            CalculateBarAndTime();
            ToggleProgressGemPanel(true);
        }
        else
        {
            ToggleProgressGemPanel(false);
            if (uiQueueBoxes.Count > 0)
            {
                uiQueueBoxes[0].RemoveItem();
            }
        }
    }

    private void CalculateBarAndTime()
    {
        timeRemainingText.text = timeSpan.ToFormattedDuration();
        currentYieldTick = progressBarWidth / (currentItem.yieldDurationInMins * 60);
        progressBarRect.sizeDelta = new Vector2(progressBarWidth - (currentYieldTick * (float)timeSpan.TotalSeconds), 0);

        gemsNeededToCompelete = (int)(timeSpan.TotalSeconds / gemCostInterval) + 1;
        gemsCompleteProdCountText.text = GameVariables.tmp_gemIcon + gemsNeededToCompelete;
    }

    private void UpdateOtherQueueBoxes()
    {
        InitCurrentItem();
        for (int i = 0; i < uiQueueBoxes.Count; i++)
        {
            uiQueueBoxes[i].RemoveItem();
        }
        for (int i = 0; i < prodBuildingData.queuedItemIds.Count; i++)
        {
            string slug = ItemDatabase.GetItemSlugById(prodBuildingData.queuedItemIds[i]);
            uiQueueBoxes[i].AddItem(slug);
        }
    }

    private void UpdateLastNewQueueBox()
    {
        if (prodBuildingData.unlockedQueueCount >= GameVariables.maxQueueBox)
        {
            lastQueueLockedBox.gameObject.SetActive(false);
        }
        else
        {
            lastQueueLockedBox.gameObject.SetActive(true);
            gemsBuyNewBoxText.text = GameVariables.tmp_gemIcon + unlockQueueBoxGemsPrice[prodBuildingData.unlockedQueueCount];
        }
    }

    private void InitCurrentItem()
    {
        if (prodBuildingData.queuedItemIds.Count >= 1)
        {
            currentYieldDateTime = prodBuildingData.yieldDateTimes[0];
            currentItem = ItemDatabase.GetItemById(prodBuildingData.queuedItemIds[0]);
            currentYieldTick = progressBarWidth / (currentItem.yieldDurationInMins * 60);
            gemCostInterval = (currentItem.yieldDurationInMins * 60) / currentItem.buyValueInGems;
            areAnyQueueItems = true;
        }
        else
        {
            areAnyQueueItems = false;
        }
    }

    private void ToggleProgressGemPanel(bool isVisible)
    {
        if (isVisible)
        {
            progressGemPanel.SetParent(uiQueueBoxes[0].transform);
            progressGemPanel.localPosition = progressGemPanelPosition;
        }
        else
        {
            progressGemPanel.SetParent(dropTransform);
        }
        progressGemPanel.gameObject.SetActive(isVisible);
    }

    private void CreateQueueBoxes()
    {
        //First move the ProgressGemPanel
        ToggleProgressGemPanel(false);
        //Then delete all UiQueueBoxs
        for (int i = 0; i < uiQueueBoxes.Count; i++)
        {
            Destroy(uiQueueBoxes[i].gameObject);
        }
        uiQueueBoxes = new List<UiQueueBox>();

        //Then create all new UiQueueBoxs
        for (int i = 0; i < prodBuildingData.unlockedQueueCount; i++)
        {
            UiQueueBox uiQueueBox = Instantiate(uiQueueBoxPrefab, queueBoxContentParent);
            uiQueueBoxes.Add(uiQueueBox);
        }
        if (prodBuildingData.unlockedQueueCount >= GameVariables.maxQueueBox)
        {
            lastQueueLockedBox.gameObject.SetActive(false);
        }
        else
        {
            lastQueueLockedBox.gameObject.SetActive(true);
            lastQueueLockedBox.SetAsLastSibling();
        }
    }

    private void CreateNewUiDragItems()
    {
        //Set header text
        sourceNameText.text = SourceDatabase.GetSourceNameById(prodBuildingData.sourceId);
        //First delete all UiDragItems
        foreach (var item in uiDragItems)
        {
            Destroy(item.gameObject);
        }
        uiDragItems = new List<UiDragItem>();

        //Then create all new UiDragItems
        List<Receipe> allFarmingReceipes = ItemReceipesDatabase.GetAllReceipeBySourceId(prodBuildingData.sourceId);
        for (int i = 0; i < allFarmingReceipes.Count; i++)
        {
            UiDragItem uiDraggerItem = Instantiate(uiDraggableItemPrefab, draggableItemContentParent);
            string slug = ItemDatabase.GetItemSlugById(allFarmingReceipes[i].outputItemId);

            uiDraggerItem.Init(DescriptionType.Receipe, allFarmingReceipes[i].outputItemId,
                AtlasBank.Instance.GetSpriteByName(slug, AtlasType.UiItems), parentScrollRect);
            uiDraggerItem.OnDragStart += OnDragStart;
            uiDraggerItem.OnDragItemPosition += OnDragItemPosition;
            uiDraggerItem.OnDragEnd += OnDragEnd;
            uiDragItems.Add(uiDraggerItem);
        }
    }

    private void ShowCanvas(ProdBuildingData prodBuildingData, Action CompleteByGems)
    {
        CompleteByGemsProdBuilding = CompleteByGems;
        if (!isCanvasVisible)
        {
            //Create new UiDrag items and Queue Box only if other building is clicked
            if (this.prodBuildingData != prodBuildingData)
            {
                this.prodBuildingData = prodBuildingData;

            }
            //Init current item
            CreateNewUiDragItems();
            InitCurrentItem();
            CreateQueueBoxes();
            UpdateFirstQueueBox();
            UpdateOtherQueueBoxes();
            UpdateLastNewQueueBox();
            ToggleCanvas(true);
        }
    }

    private void HideCanvas()
    {
        if (isCanvasVisible)
        {
            ToggleCanvas(false);
        }
    }

    protected override void ToggleCanvas(bool isVisible)
    {
        base.ToggleCanvas(isVisible);

        if (isVisible)
        {
            for (int i = 0; i < uiDragItems.Count; i++)
            {
                uiDragItems[i].UpdateCountText();
            }
        }
        isCanvasVisible = isVisible;
    }


    #region Gem releated buttons 
    private void OnItemCompleteGemButtonClicked()
    {
        ToggleCanvas(false);
        bool hasGems = PlayerCurrencyManager.ReduceGems(gemsNeededToCompelete);
        if (hasGems)
        {
            CompleteByGemsProdBuilding?.Invoke();
        }
        else
        {
            //Show some common ui to purchase gems
        }
    }

    private void OnGemsBuyNewQueueBoxButtonClicked()
    {
        bool haveGems = PlayerCurrencyManager.ReduceGems(unlockQueueBoxGemsPrice[prodBuildingData.unlockedQueueCount]);
        if (!haveGems)
        {
            return;
        }
        prodBuildingData.unlockedQueueCount++;
        UiQueueBox uiQueueBox = Instantiate(uiQueueBoxPrefab, queueBoxContentParent);
        uiQueueBoxes.Add(uiQueueBox);
        if (prodBuildingData.unlockedQueueCount >= GameVariables.maxQueueBox)
        {
            lastQueueLockedBox.gameObject.SetActive(false);
        }
        else
        {
            lastQueueLockedBox.gameObject.SetActive(true);
            lastQueueLockedBox.SetAsLastSibling();
            gemsBuyNewBoxText.text = GameVariables.tmp_gemIcon + unlockQueueBoxGemsPrice[prodBuildingData.unlockedQueueCount];
        }
    }
    #endregion


    #region Drag Drop Stuff
    private void OnDragStart(int itemId, Sprite sprite)
    {
        dropWindow.SetAsLastSibling();
        currentDraggedItemId = itemId;
        cursorImage.sprite = sprite;
    }

    private void OnDragItemPosition(Vector2 dragItePosition)
    {
        cursorImage.enabled = true;
        cursorImage.transform.position = dragItePosition;
    }

    private void OnDragEnd()
    {
        dropWindow.SetAsFirstSibling();
        cursorImage.enabled = false;
        currentDraggedItemId = -1;
    }

    private void UiDropItemHandler()
    {
        if (currentDraggedItemId >= 0)
        {
            OnItemDroppedOnProdBuilding?.Invoke(currentDraggedItemId);
        }
        UpdateUiItems();
    }

    private void UpdateUiItems()
    {
        if (prodBuildingData.queuedItemIds.Count == 1)
        {
            InitCurrentItem();
        }
        if (prodBuildingData.queuedItemIds.Count > 0)
        {
            int lastIndex = prodBuildingData.queuedItemIds.Count - 1;
            Item lastItem = ItemDatabase.GetItemById(prodBuildingData.queuedItemIds[lastIndex]);
            uiQueueBoxes[lastIndex].AddItem(lastItem.slug);
        }
        UpdateFirstQueueBox();
    }
    #endregion
}