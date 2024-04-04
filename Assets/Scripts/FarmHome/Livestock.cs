using System;
using System.Collections;
using System.Collections.Generic;
using MiningTown.IO;
using UnityEngine;
using UnityEngine.UI;

public class Livestock : MovableUnitBase, IInteractable
{
    public Action<int> OnLivestockBuildingClicked;
    [SerializeField] private Image harvestItemPrefab;
    [SerializeField] private Transform mainPanel;
    [SerializeField] private GameObject harvestCanvas;
    private int livestockBuildingIndex;
    public int doneItemIdCount;
    public LivestockData livestockData;
    private Vector3 boxColliderSize;
    private List<TimeSpan> timeSpans = new List<TimeSpan>();

    private void LateUpdate()
    {
        if (livestockData != null)
        {
            CalculateStates();
        }
    }

    private void CalculateStates()
    {
        for (int i = 0; i < livestockData.unlockedQueueCount; i++)
        {
            timeSpans[i] = livestockData.yieldDateTimes[i].Subtract(DateTime.UtcNow);
            //Hungry
            if (livestockData.livestockStates[i] == LivestockState.Hungry)
            {
                //wait
            }
            //OnHarvest
            else if (timeSpans[i].TotalSeconds <= 0)
            {
                if (livestockData.livestockStates[i] != LivestockState.Harvest)
                {
                    livestockData.livestockStates[i] = LivestockState.Harvest;
                    AddDoneItemsForHarvest();
                }
            }
            //Eating
            else
            {
                livestockData.livestockStates[i] = LivestockState.Eating;
            }
        }
    }

    public void InitLivestock(LivestockData livestockData, Source source, int livestockIndex, bool isNewlyPlaced)
    {
        for (int i = 0; i < GameVariables.maxLivestock; i++)
        {
            timeSpans.Add(new TimeSpan());
        }
        boxColliderSize = GetComponent<BoxCollider>().size;
        livestockBuildingIndex = livestockIndex;
        transform.name = source.slug + livestockIndex.ToString();
        this.livestockData = livestockData;
        transform.position = this.livestockData.pos;
        this.livestockData.yieldDateTimes = new DateTime[GameVariables.maxLivestock];
        for (int i = 0; i < this.livestockData.yieldDateTimeJsons.Length; i++)
        {
            if (String.IsNullOrEmpty(livestockData.yieldDateTimeJsons[i]))
            {
                this.livestockData.yieldDateTimes[i] = DateTime.UtcNow;
                this.livestockData.yieldDateTimeJsons[i] = JsonUtility.ToJson((JsonDateTime)this.livestockData.yieldDateTimes[i]);
            }
            else
            {
                this.livestockData.yieldDateTimes[i] = JsonUtility.FromJson<JsonDateTime>(livestockData.yieldDateTimeJsons[i]);
            }
        }
        ShowHarvestItemsOnInit();
        CalculateStates();
        if (isNewlyPlaced)
        {
            isInBuildMenu = true;
            EnableEditMode();
        }
    }

    private void CompleteByGems()
    {
        //prodBuildingData.yieldDateTimes[0] = DateTime.UtcNow;
        //prodBuildingData.yieldDateTimeJsons[0] = JsonUtility.ToJson((JsonDateTime)prodBuildingData.yieldDateTimes[0]);
        //yieldTotalSeconds = 0;
        //if (prodBuildingData.queuedItemIds.Count > 1)
        //{
        //    for (int i = 1; i < prodBuildingData.queuedItemIds.Count; i++)
        //    {
        //        Item item = ItemDatabase.GetItemById(prodBuildingData.queuedItemIds[i]);
        //        int secondsToAdd = ItemDatabase.GetYieldDurationInMinsById(item.itemId) * 60;
        //        if (i > 1)
        //        {
        //            secondsToAdd += (int)prodBuildingData.yieldDateTimes[i - 1].Subtract(DateTime.UtcNow).TotalSeconds;
        //        }
        //        print(secondsToAdd);
        //        DateTime yieldDateTime = DateTime.UtcNow.AddSeconds(secondsToAdd);
        //        prodBuildingData.yieldDateTimeJsons[i] = JsonUtility.ToJson((JsonDateTime)yieldDateTime);
        //        prodBuildingData.yieldDateTimes[i] = yieldDateTime;
        //    }
        //}
    }


    #region Harvest and World Canvas
    private void ShowHarvestItemsOnInit()
    {
        for (int i = 0; i < livestockData.unlockedQueueCount; i++)
        {
            if (livestockData.livestockStates[i] == LivestockState.Harvest)
            {
                AddDoneItemsForHarvest();
            }
        }
        if (doneItemIdCount <= 0)
        {
            ToggleMainPanel(false);
        }
    }

    private void AddDoneItemsForHarvest()
    {
        doneItemIdCount++;
        CreateItemsToAddOnWorldCanvas();
        ToggleMainPanel(true);
    }

    private void CreateItemsToAddOnWorldCanvas()
    {
        Image harvestItem = Instantiate(harvestItemPrefab, mainPanel);
        harvestItem.gameObject.SetActive(true);
        string slug = ItemDatabase.GetItemSlugById(livestockData.outputItemId);
        harvestItem.sprite = AtlasBank.Instance.GetSpriteByName(slug, AtlasType.UiItems);
        harvestItem.name = slug;
    }

    private void HarvestAllItems()
    {
        for (int i = 0; i < doneItemIdCount; i++)
        {
            //int xpToAdd = ItemDatabase.GetItemXpOnHarvestById(livestockData.outputItemId);
            //SaveLoadManager.AddUpdateItem(livestockData.outputItemId, 1);
            //PlayerCurrencyManager.AddXp(xpToAdd);

            //UiResourceSpawnCanvas.OnHarvestResource?.Invoke(
            //    ItemDatabase.GetItemSlugById(livestockData.outputItemId), transform, 1);
            //UiResourceSpawnCanvas.OnHarvestResource?.Invoke("xp", transform, xpToAdd);

            DroppedItemManager.OnDropResource(ItemDatabase.GetItemById(livestockData.outputItemId),
                PlayerMovement.Instance.GetPlayerTransform(), 1);

        }
        doneItemIdCount = 0;
        for (int i = 0; i < livestockData.unlockedQueueCount; i++)
        {
            if (livestockData.livestockStates[i] == LivestockState.Harvest)
            {
                livestockData.livestockStates[i] = LivestockState.Hungry;
            }
        }
        foreach (Transform child in mainPanel)
        {
            Destroy(child.gameObject);
        }
        ToggleMainPanel(false);
    }

    private void ToggleMainPanel(bool isVisible)
    {
        if (harvestCanvas.gameObject.activeInHierarchy == !isVisible)
        {
            harvestCanvas.gameObject.SetActive(isVisible);
        }
    }
    #endregion


    #region IIneractable
    public void InteractOnClick()
    {
        if (doneItemIdCount > 0)
        {
            HarvestAllItems();
        }
        else
        {
            OnLivestockBuildingClicked?.Invoke(livestockBuildingIndex);
            UiLiveStockCanvas.OnShowCanvas?.Invoke(livestockData, timeSpans);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public Vector3 GetColliderSize()
    {
        return boxColliderSize;
    }

    public IneractableType GetIneractableType()
    {
        return IneractableType.Livestock;
    }
    #endregion


    #region MovableUnitBase & Save Building position
    protected override void SavePositionData()
    {
        base.SavePositionData();
        livestockData.pos = transform.position;
    }

    protected override void AddThisNewBuilding()
    {
        base.AddThisNewBuilding();
        SaveLoadManager.AddNewLivestock(livestockData);
        livestockBuildingIndex = SaveLoadManager.saveData.livestockDatas.Count - 1;
    }

    #endregion
}
