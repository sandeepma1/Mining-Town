using System;
using System.Collections.Generic;
using MiningTown.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiBuyItemsByGems : UiBasicCanvasWindowBase
{
    public static Action<int, int, Action<int>> OnShowBuyItemsMenuForReceipe;
    public static Action<int, int, int, Action<int>> OnShowBuyItemsMenuForSingleItemId;

    [SerializeField] private UiRequiredItem[] uiRequiredItems;
    [SerializeField] private Button buyWithGemsButton;
    [SerializeField] private TextMeshProUGUI buyWithGemsText;
    private Action<int> OnBuyWithGems;
    private int actionParameter;
    private List<int> removeItemIds = new List<int>();
    private List<int> removeCount = new List<int>();
    private List<int> buyItemIds = new List<int>();
    private List<int> buyCount = new List<int>();
    private int totalGemsCountToBuy;
    private Receipe currentReceipe;

    private void Start()
    {
        isPopup = true;
        //OnShowBuyItemsMenu += ShowBuyItemsMenuEvent;
        OnShowBuyItemsMenuForReceipe += ShowBuyItemsMenuForReceipe;
        OnShowBuyItemsMenuForSingleItemId += ShowBuyItemsMenuBySingleItemId;
        buyWithGemsButton.onClick.AddListener(OnBuyWithGemsButtonClicked);
        ToggleCanvas(false);
    }

    private void OnDestroy()
    {
        OnShowBuyItemsMenuForReceipe -= ShowBuyItemsMenuForReceipe;
        OnShowBuyItemsMenuForSingleItemId -= ShowBuyItemsMenuBySingleItemId;
        buyWithGemsButton.onClick.RemoveListener(OnBuyWithGemsButtonClicked);
    }

    private void OnBuyWithGemsButtonClicked()
    {
        if (PlayerCurrencyManager.ReduceGems(totalGemsCountToBuy))
        {
            ToggleCanvas(false);
            for (int i = 0; i < removeItemIds.Count; i++)
            {
                SaveLoadManager.RemoveFarmBarnItem(removeItemIds[i], removeCount[i]);
            }
            OnBuyWithGems?.Invoke(actionParameter);
            OnBuyWithGems = null;
            removeItemIds = new List<int>();
            removeCount = new List<int>();
        }
    }

    private void ShowBuyItemsMenuForReceipe(int outputItemId, int actionParameter, Action<int> OnBuyWithGemsButtonClick)
    {
        //reset all old values
        currentReceipe = ItemReceipesDatabase.GetReceipeById(outputItemId);
        this.actionParameter = actionParameter;
        OnBuyWithGems = OnBuyWithGemsButtonClick;
        buyItemIds = new List<int>();
        buyCount = new List<int>();
        removeItemIds = new List<int>();
        removeCount = new List<int>();
        totalGemsCountToBuy = 0;

        //Check if all required items exists in inventory
        CheckIfHaveItem(currentReceipe.reqId1, currentReceipe.reqCount1);
        CheckIfHaveItem(currentReceipe.reqId2, currentReceipe.reqCount2);
        CheckIfHaveItem(currentReceipe.reqId3, currentReceipe.reqCount3);

        //First disable all uiRequiredItems
        for (int i = 0; i < uiRequiredItems.Length; i++)
        {
            uiRequiredItems[i].gameObject.SetActive(false);
        }
        //Enable only those uiRequiredItems
        for (int i = 0; i < buyItemIds.Count; i++)
        {
            totalGemsCountToBuy += ItemDatabase.GetBuyValueInGemsByItemId(buyItemIds[i]) * buyCount[i];
            uiRequiredItems[i].gameObject.SetActive(true);
            uiRequiredItems[i].InitRequiredCount(buyItemIds[i], buyCount[i]);
        }
        buyWithGemsText.text = GameVariables.tmp_gemIcon + totalGemsCountToBuy;
        ToggleCanvas(true);
    }

    private void ShowBuyItemsMenuBySingleItemId(int itemId, int count, int actionParameter, Action<int> OnBuyWithGemsButtonClick)
    {
        //reset all old values        
        this.actionParameter = actionParameter;
        OnBuyWithGems = OnBuyWithGemsButtonClick;
        buyItemIds = new List<int>();
        buyCount = new List<int>();
        removeItemIds = new List<int>();
        removeCount = new List<int>();
        totalGemsCountToBuy = 0;

        //Check if all required items exists in inventory
        CheckIfHaveItem(itemId, count);

        //First disable all uiRequiredItems
        for (int i = 0; i < uiRequiredItems.Length; i++)
        {
            uiRequiredItems[i].gameObject.SetActive(false);
        }
        //Enable only those uiRequiredItems
        for (int i = 0; i < buyItemIds.Count; i++)
        {
            totalGemsCountToBuy += ItemDatabase.GetBuyValueInGemsByItemId(buyItemIds[i]) * buyCount[i];
            uiRequiredItems[i].gameObject.SetActive(true);
            uiRequiredItems[i].InitRequiredCount(buyItemIds[i], buyCount[i]);
        }
        buyWithGemsText.text = GameVariables.tmp_gemIcon + totalGemsCountToBuy;
        ToggleCanvas(true);
    }

    private void CheckIfHaveItem(int itemId, int count)
    {
        if (itemId <= 0)
        {
            return;
        }
        int haveItemCount = SaveLoadManager.DoesItemExistsReturnCount(itemId);
        if (count > haveItemCount) //Have to buy
        {
            buyItemIds.Add(itemId);
            buyCount.Add(count - haveItemCount);
            //If some items are there then add this to remove account
            if (haveItemCount > 0)
            {
                removeItemIds.Add(itemId);
                removeCount.Add(haveItemCount);
            }
        }
        else // No need to buy, just reduce
        {
            removeItemIds.Add(itemId);
            removeCount.Add(count);
        }
    }
}