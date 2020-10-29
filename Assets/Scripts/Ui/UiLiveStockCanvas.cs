using System;
using MiningTown.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UiLiveStockCanvas : UiBasicCanvasWindowBase
{
    public static Action<int> OnItemDroppedOnLivestockBuilding;
    public static Action<LivestockData, List<TimeSpan>> OnShowCanvas;
    public static Action OnHideCanvas;
    [Header("Child Class")]
    [Space(10)]
    [SerializeField] private Image cursorImage;
    [Header("Scroll prefabs content")]
    [Space(10)]
    [SerializeField] private TextMeshProUGUI sourceNameText;
    [Header("Last Queue Box")]
    [Space(10)]
    [SerializeField] private RectTransform lastUiLivestock;
    [SerializeField] private Image lastUiLivestockImage;
    [SerializeField] private Button gemsBuyNewLivestockButton;
    [SerializeField] private TextMeshProUGUI gemsBuyNewBoxText;
    [SerializeField] private UiLiveStockiDragItem[] UiLiveStockiDragItems = new UiLiveStockiDragItem[3];
    [SerializeField] private UiLivestock[] uiLivestocks = new UiLivestock[5];
    public LivestockData livestockData;
    public int currentDraggedRequiredIndex = -1;
    private int gemsNeededToCompelete;
    private bool isCanvasVisible;
    private int[] unlockLivestocksGemsPrice = new int[] { 0, 6, 12, 24, 36 };
    private Item outputItem;
    private Receipe currentReceipe;
    private List<TimeSpan> timeSpans = new List<TimeSpan>();

    private void Start()
    {
        for (int i = 0; i < GameVariables.maxLivestock; i++)
        {
            timeSpans.Add(new TimeSpan());
        }
        OnShowCanvas += ShowCanvas;
        OnHideCanvas += HideCanvas;
        for (int i = 0; i < UiLiveStockiDragItems.Length; i++)
        {
            UiLiveStockiDragItems[i].OnDragStart += OnDragStart;
            UiLiveStockiDragItems[i].OnDragItemPosition += OnDragItemPosition;
            UiLiveStockiDragItems[i].OnDragEnd += OnDragEnd;
        }
        for (int i = 0; i < uiLivestocks.Length; i++)
        {
            uiLivestocks[i].Init(i);
            uiLivestocks[i].OnDropItem += OnUiLivestocksDropItem;
            uiLivestocks[i].OnGemButtonClick += OnGemButtonClick;
        }
        gemsBuyNewLivestockButton.onClick.AddListener(OnGemsBuyNewQueueBoxButtonClicked);
        HideCanvas();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < uiLivestocks.Length; i++)
        {
            uiLivestocks[i].OnDropItem -= OnUiLivestocksDropItem;
            uiLivestocks[i].OnGemButtonClick -= OnGemButtonClick;
        }
        OnShowCanvas -= ShowCanvas;
        OnHideCanvas -= HideCanvas;
        gemsBuyNewLivestockButton.onClick.RemoveListener(OnGemsBuyNewQueueBoxButtonClicked);
    }

    private void Update()
    {
        if (!isCanvasVisible)
        {
            return;
        }

        CalculateStates();
    }

    private void CalculateStates()
    {
        for (int i = 0; i < uiLivestocks.Length; i++)
        {
            switch (livestockData.livestockStates[i])
            {
                case LivestockState.Hungry:
                    uiLivestocks[i].OnHungry();
                    break;
                case LivestockState.Eating:
                    uiLivestocks[i].OnEating(timeSpans[i]);
                    break;
                case LivestockState.Harvest:
                    uiLivestocks[i].OnHarvest();
                    break;
                default:
                    break;
            }
        }
    }

    private void OnUiLivestocksDropItem(int livestockIndex)
    {
        if (currentDraggedRequiredIndex < 0 || livestockData.livestockStates[livestockIndex] == LivestockState.Eating)
        {
            return;
        }

        //First check required vs have item count
        int requiredCount = 0;
        int haveCount = 0;
        int currentDraggedItemId = 0;
        switch (currentDraggedRequiredIndex)
        {
            case 0:
                requiredCount = currentReceipe.reqCount1;
                currentDraggedItemId = currentReceipe.reqId1;
                haveCount = SaveLoadManager.DoesItemExistsReturnCount(currentDraggedItemId);
                break;
            case 1:
                requiredCount = currentReceipe.reqCount2;
                currentDraggedItemId = currentReceipe.reqId2;
                haveCount = SaveLoadManager.DoesItemExistsReturnCount(currentDraggedItemId);
                break;
            case 2:
                requiredCount = currentReceipe.reqCount3;
                currentDraggedItemId = currentReceipe.reqId3;
                haveCount = PlayerCurrencyManager.GetCoinCount();
                break;
            default:
                break;
        }

        if (currentDraggedRequiredIndex != 2) // Tihs is 1st and 2nd feed
        {
            if (requiredCount <= haveCount)
            {
                //Feed to livestock
                UiFarmBarnInventory.OnReduceItemInInventory(currentDraggedRequiredIndex, requiredCount);
                FeedToLivestock(livestockIndex);
            }
            else
            {
                UiBuyItemsByGems.OnShowBuyItemsMenuForSingleItemId?.Invoke(currentDraggedItemId, requiredCount, livestockIndex, FeedToLivestock);
            }
        }
        else // Tihs is 3rd feed
        {
            if (requiredCount <= haveCount)
            {
                //Feed to livestock
                PlayerCurrencyManager.ReduceCoins(requiredCount);
                FeedToLivestock(livestockIndex);
            }
            else
            {
                PlayerCurrencyManager.ShowPopupToGetCoins();
            }
        }

    }

    private void FeedToLivestock(int livestockIndex)
    {
        livestockData.yieldDateTimes[livestockIndex] = DateTime.UtcNow.AddMinutes(outputItem.yieldDurationInMins);
        livestockData.yieldDateTimeJsons[livestockIndex] = JsonUtility.ToJson((JsonDateTime)livestockData.yieldDateTimes[livestockIndex]);
        livestockData.livestockStates[livestockIndex] = LivestockState.Eating;
        for (int i = 0; i < UiLiveStockiDragItems.Length; i++)
        {
            UiLiveStockiDragItems[i].UpdateCountText();
        }
    }

    private void ShowCanvas(LivestockData livestockData, List<TimeSpan> timeSpans)
    {
        this.timeSpans = timeSpans;
        outputItem = ItemDatabase.GetItemById(livestockData.outputItemId);
        if (!isCanvasVisible)
        {
            this.livestockData = livestockData;
            //Create new UiDrag items and Queue Box only if other building is clicked
            //if (this.livestockData != livestockData)
            //{
            //    this.livestockData = livestockData;
            //}
            //Init current item
            InitUiDragItems();
            InitUiLivestocks();
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

    private void InitUiDragItems()
    {
        //Set header text
        sourceNameText.text = SourceDatabase.GetSourceNameById(livestockData.sourceId);
        //0 and 1 are fram items, 2 is paid item
        currentReceipe = ItemReceipesDatabase.GetReceipeById(livestockData.outputItemId);
        UiLiveStockiDragItems[0].Init(0, currentReceipe.reqId1, currentReceipe.reqCount1);
        UiLiveStockiDragItems[1].Init(1, currentReceipe.reqId2, currentReceipe.reqCount2);
        UiLiveStockiDragItems[2].Init(2, currentReceipe.reqId3, currentReceipe.reqCount3);
    }

    private void InitUiLivestocks()
    {
        string livestockSlug = SourceDatabase.GetSlugBySourceId(livestockData.sourceId);
        lastUiLivestockImage.sprite = AtlasBank.Instance.GetSpriteByName(livestockSlug, AtlasType.UiItems);
        //Hide/show last ui live stock
        for (int i = 0; i < uiLivestocks.Length; i++)
        {
            if (i < livestockData.unlockedQueueCount) // These are unlocked
            {
                uiLivestocks[i].gameObject.SetActive(true);
                uiLivestocks[i].InitLivestock(livestockData.outputItemId, lastUiLivestockImage.sprite);
                CalculateStates();
            }
            else // These are locked
            {
                uiLivestocks[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateLastNewQueueBox()
    {
        if (livestockData.unlockedQueueCount >= GameVariables.maxLivestock)
        {
            lastUiLivestock.gameObject.SetActive(false);
        }
        else
        {
            lastUiLivestock.gameObject.SetActive(true);
            lastUiLivestock.SetAsLastSibling();
            gemsBuyNewBoxText.text = GameVariables.tmp_gemIcon + unlockLivestocksGemsPrice[livestockData.unlockedQueueCount];
        }
    }

    protected override void ToggleCanvas(bool isVisible)
    {
        base.ToggleCanvas(isVisible);
        cursorImage.gameObject.SetActive(isVisible);
        if (isVisible)
        {
            for (int i = 0; i < UiLiveStockiDragItems.Length; i++)
            {
                UiLiveStockiDragItems[i].UpdateCountText();
            }
        }
        isCanvasVisible = isVisible;
    }


    #region Gem releated buttons     
    private void OnGemButtonClick(int gemCountToComplete, int index)
    {
        bool hasGems = PlayerCurrencyManager.ReduceGems(gemsNeededToCompelete);
        if (hasGems)
        {
            print("CompleteByGems");
            livestockData.yieldDateTimes[index] = DateTime.UtcNow;
            livestockData.yieldDateTimeJsons[index] = JsonUtility.ToJson((JsonDateTime)livestockData.yieldDateTimes[index]);
            timeSpans[index] = TimeSpan.Zero;
        }
        else
        {
            //Show some common ui to purchase gems
        }
    }

    private void OnGemsBuyNewQueueBoxButtonClicked()
    {
        bool haveGems = PlayerCurrencyManager.ReduceGems(unlockLivestocksGemsPrice[livestockData.unlockedQueueCount - 1]);
        if (!haveGems)
        {
            return;
        }
        livestockData.unlockedQueueCount++;
        uiLivestocks[livestockData.unlockedQueueCount - 1].gameObject.SetActive(true);
        uiLivestocks[livestockData.unlockedQueueCount - 1].InitLivestock(livestockData.outputItemId, lastUiLivestockImage.sprite);
        UpdateLastNewQueueBox();
    }
    #endregion


    #region Drag Drop Stuff
    private void OnDragStart(int requiredIndex, Sprite sprite)
    {
        currentDraggedRequiredIndex = requiredIndex;
        cursorImage.sprite = sprite;
    }

    private void OnDragItemPosition(Vector2 dragItePosition)
    {
        cursorImage.enabled = true;
        cursorImage.transform.position = dragItePosition;
    }

    private void OnDragEnd()
    {
        cursorImage.enabled = false;
        currentDraggedRequiredIndex = -1;
    }
    #endregion
}