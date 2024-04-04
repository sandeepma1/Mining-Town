using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MiningTown.IO;
using UnityEngine;
using UnityEngine.UI;

public class ProdBuilding : MovableUnitBase, IInteractable
{
    public Action<int> OnProdBuildingClicked;
    [SerializeField] private ParticleSystem workingParticleSystem;
    [SerializeField] private Image harvestItemPrefab;
    [SerializeField] private Transform mainPanel;
    [SerializeField] private GameObject harvestCanvas;
    private int prodIndex;
    private int yieldTotalSeconds;
    private ProdBuildingData prodBuildingData;
    private Vector3 boxColliderSize;

    private ProdBuildingState ProdBuildingState
    {
        get
        {
            return prodBuildingData.prodBuildingState;
        }
        set
        {
            prodBuildingData.prodBuildingState = value;
            switch (prodBuildingData.prodBuildingState)
            {
                case ProdBuildingState.Idle:
                    workingParticleSystem.Stop();
                    break;
                case ProdBuildingState.Working:
                    workingParticleSystem.Play();
                    //Show box making animation
                    break;
                default:
                    break;
            }
        }
    }

    private void Update()
    {
        if (ProdBuildingState == ProdBuildingState.Working)
        {
            if (prodBuildingData.queuedItemIds.Count > 0)
            {
                ProdBuildingState = ProdBuildingState.Working;
                yieldTotalSeconds = (int)prodBuildingData.yieldDateTimes[0].Subtract(DateTime.UtcNow).TotalSeconds;

                //Production done remove item
                if (yieldTotalSeconds <= 0)
                {
                    int itemId = prodBuildingData.queuedItemIds[0];
                    AddDoneItemsForHarvest(itemId);
                    prodBuildingData.queuedItemIds.RemoveAt(0);
                    prodBuildingData.yieldDateTimeJsons.RemoveAt(0);
                    prodBuildingData.yieldDateTimes.RemoveAt(0);
                    if (prodBuildingData.queuedItemIds.Count <= 0)
                    {
                        ProdBuildingState = ProdBuildingState.Idle;
                    }
                }
            }
            else
            {
                ProdBuildingState = ProdBuildingState.Idle;
            }
        }
    }

    public void InitProdBuilding(ProdBuildingData prodBuildingData, Source source, int prodIndex, bool isNewlyPlaced)
    {
        boxColliderSize = GetComponent<BoxCollider>().size;
        this.prodIndex = prodIndex;
        transform.name = source.slug + prodIndex.ToString();
        this.prodBuildingData = prodBuildingData;
        transform.position = this.prodBuildingData.pos;
        ShowHarvestItemsOnInit();
        //Set all DateTimes from DateTimeJsons
        this.prodBuildingData.yieldDateTimes = new List<DateTime>();
        for (int i = 0; i < this.prodBuildingData.yieldDateTimeJsons.Count; i++)
        {
            this.prodBuildingData.yieldDateTimes.Add(JsonUtility.FromJson<JsonDateTime>(prodBuildingData.yieldDateTimeJsons[i]));
        }

        //Reverse loop to check all the queue items and remove if done
        for (int i = this.prodBuildingData.queuedItemIds.Count - 1; i >= 0; i--)
        {
            yieldTotalSeconds = (int)this.prodBuildingData.yieldDateTimes[i].Subtract(DateTime.UtcNow).TotalSeconds;
            if (yieldTotalSeconds <= 0)
            {
                int itemId = this.prodBuildingData.queuedItemIds[i];
                AddDoneItemsForHarvest(itemId);
                print("Done item " + this.prodBuildingData.queuedItemIds[i]);
                //Show items on Building to pickup
                this.prodBuildingData.queuedItemIds.RemoveAt(i);
                this.prodBuildingData.yieldDateTimes.RemoveAt(i);
                this.prodBuildingData.yieldDateTimeJsons.RemoveAt(i);
            }
        }

        // Set the first queue item yield time in seconds and states
        if (this.prodBuildingData.queuedItemIds.Count > 0)
        {
            yieldTotalSeconds = (int)this.prodBuildingData.yieldDateTimes[0].Subtract(DateTime.UtcNow).TotalSeconds;
            ProdBuildingState = ProdBuildingState.Working;
        }
        else
        {
            ProdBuildingState = ProdBuildingState.Idle;
        }
        if (isNewlyPlaced)
        {
            isInBuildMenu = true;
            EnableEditMode();
        }
    }

    private void CompleteByGems()
    {
        prodBuildingData.yieldDateTimes[0] = DateTime.UtcNow;
        prodBuildingData.yieldDateTimeJsons[0] = JsonUtility.ToJson((JsonDateTime)prodBuildingData.yieldDateTimes[0]);
        yieldTotalSeconds = 0;
        if (prodBuildingData.queuedItemIds.Count > 1)
        {
            for (int i = 1; i < prodBuildingData.queuedItemIds.Count; i++)
            {
                Item item = ItemDatabase.GetItemById(prodBuildingData.queuedItemIds[i]);
                int secondsToAdd = ItemDatabase.GetYieldDurationInMinsById(item.itemId) * 60;
                if (i > 1)
                {
                    secondsToAdd += (int)prodBuildingData.yieldDateTimes[i - 1].Subtract(DateTime.UtcNow).TotalSeconds;
                }
                print(secondsToAdd);
                DateTime yieldDateTime = DateTime.UtcNow.AddSeconds(secondsToAdd);
                prodBuildingData.yieldDateTimeJsons[i] = JsonUtility.ToJson((JsonDateTime)yieldDateTime);
                prodBuildingData.yieldDateTimes[i] = yieldDateTime;
            }
        }
    }


    #region MovableUnitBase & Save Building position
    protected override void SavePositionData()
    {
        base.SavePositionData();
        prodBuildingData.pos = transform.position;
    }

    protected override void AddThisNewBuilding()
    {
        base.AddThisNewBuilding();
        SaveLoadManager.AddNewProdBuilding(prodBuildingData);
        prodIndex = SaveLoadManager.saveData.prodBuildingDatas.Count - 1;
    }
    #endregion


    #region Growing
    public void OnItemDroppedOnProdBuilding(int outputItemId)
    {
        //Check if queue is empty
        if (prodBuildingData.queuedItemIds.Count >= prodBuildingData.unlockedQueueCount)
        {
            print("Prod queue is full");
            //TODO: Show UI to unlock new queue box
            return;
        }

        if (ItemReceipesDatabase.DoesHasAllItemsInReceipe(outputItemId, true)) //This will check and remove all receipe items
        {
            //Add to Production
            AddItemToProduction(outputItemId);
        }
        else
        {
            print("no items ");
            UiBuyItemsByGems.OnShowBuyItemsMenuForReceipe(outputItemId, outputItemId, OnBuyRemainingItemsClicked);
        }
    }

    private void OnBuyRemainingItemsClicked(int outputItemId)
    {
        AddItemToProduction(outputItemId);
        UiProdBuildingCanvas.OnUpdateUiItems?.Invoke();
    }

    private void AddItemToProduction(int itemId)
    {
        // print("AddItemToProduction " + itemId);
        ProdBuildingState = ProdBuildingState.Working;
        int secondsToAdd = ItemDatabase.GetYieldDurationInMinsById(itemId) * 60;
        if (prodBuildingData.queuedItemIds.Count == 1)
        {
            secondsToAdd += yieldTotalSeconds;
        }
        if (prodBuildingData.queuedItemIds.Count > 1)
        {
            int lastItemIndex = prodBuildingData.queuedItemIds.Count - 1;
            secondsToAdd += (int)prodBuildingData.yieldDateTimes[lastItemIndex].Subtract(DateTime.UtcNow).TotalSeconds;
        }
        prodBuildingData.queuedItemIds.Add(itemId);
        DateTime yieldDateTime = DateTime.UtcNow.AddSeconds(secondsToAdd);
        prodBuildingData.yieldDateTimeJsons.Add(JsonUtility.ToJson((JsonDateTime)yieldDateTime));
        prodBuildingData.yieldDateTimes.Add(yieldDateTime);
    }
    #endregion


    #region Harvest and World Canvas
    private void ShowHarvestItemsOnInit()
    {
        if (prodBuildingData.doneItemIds == null)
        {
            return;
        }
        if (prodBuildingData.doneItemIds.Count > 0)
        {
            ToggleMainPanel(true);
            for (int i = 0; i < prodBuildingData.doneItemIds.Count; i++)
            {
                CreateItemsToAddOnWorldCanvas(prodBuildingData.doneItemIds[i]);
            }
        }
        else
        {
            ToggleMainPanel(false);
        }
    }

    private void AddDoneItemsForHarvest(int itemId)
    {
        prodBuildingData.doneItemIds.Add(itemId);
        CreateItemsToAddOnWorldCanvas(itemId);
        ToggleMainPanel(true);
    }

    private void CreateItemsToAddOnWorldCanvas(int itemId)
    {
        Image harvestItem = Instantiate(harvestItemPrefab, mainPanel);
        harvestItem.gameObject.SetActive(true);
        string slug = ItemDatabase.GetItemSlugById(itemId);
        harvestItem.sprite = AtlasBank.Instance.GetSpriteByName(slug, AtlasType.UiItems);
        harvestItem.name = slug;
    }

    private void HarvestAllItems()
    {
        for (int i = 0; i < prodBuildingData.doneItemIds.Count; i++)
        {
            //int xpToAdd = ItemDatabase.GetItemXpOnHarvestById(prodBuildingData.doneItemIds[i]);
            //SaveLoadManager.AddUpdateItem(prodBuildingData.doneItemIds[i], 1);
            //PlayerCurrencyManager.AddXp(xpToAdd);

            DroppedItemManager.OnDropResource(ItemDatabase.GetItemById(prodBuildingData.doneItemIds[i]), PlayerMovement.Instance.GetPlayerTransform(), 1);
            //UiResourceSpawnCanvas.OnHarvestResource?.Invoke(ItemDatabase.GetItemSlugById(prodBuildingData.doneItemIds[i]), transform, 1);
            //UiResourceSpawnCanvas.OnHarvestResource?.Invoke("xp", transform, xpToAdd);
        }
        foreach (Transform child in mainPanel)
        {
            Destroy(child.gameObject);
        }
        prodBuildingData.doneItemIds = new List<int>();
        ToggleMainPanel(false);
    }

    private bool HasHarvestItems()
    {
        return prodBuildingData.doneItemIds.Count > 0;
    }

    private void ToggleMainPanel(bool isVisible)
    {
        if (harvestCanvas.gameObject.activeInHierarchy == !isVisible)
        {
            harvestCanvas.gameObject.SetActive(isVisible);
        }
    }
    #endregion


    #region IFarmIneractable
    public void InteractOnClick()
    {
        if (HasHarvestItems())
        {
            HarvestAllItems();
        }
        else
        {
            OnProdBuildingClicked?.Invoke(prodIndex);
            UiProdBuildingCanvas.OnShowCanvas?.Invoke(prodBuildingData, CompleteByGems);
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
        return IneractableType.ProdBuilding;
    }
    #endregion
}