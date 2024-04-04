using System;
using System.Collections;
using MiningTown.IO;
using UnityEngine;
using DG.Tweening;

public class RaisedBed : MovableUnitBase, IInteractable, IShakeable
{
    public Action<int> OnRaisedBedClicked;
    [SerializeField] private Transform cropParent;
    [SerializeField] private ParticleSystem harvestParticleSystem;
    private int bedIndex;
    private int currentGrowthIndex = -1;
    private int yieldTotalSeconds;
    private Transform[] cropParents;
    private SpriteRenderer[] cropSprites;
    private Animator[] cropAnimators;
    private RaisedBedData raisedBedData;
    public int[] cropStages = new int[4];
    private int yieldCropCount = 1;

    public RaisedBedState RaisedBedState
    {
        get
        {
            return raisedBedData.raisedBedState;
        }
        private set
        {
            raisedBedData.raisedBedState = value;
            if (harvestParticleSystem == null)
            {
                harvestParticleSystem = GetComponentInChildren<ParticleSystem>();
            }
            switch (raisedBedData.raisedBedState)
            {
                case RaisedBedState.Idle:
                    harvestParticleSystem.Stop();
                    break;
                case RaisedBedState.Growing:
                    harvestParticleSystem.Stop();
                    break;
                case RaisedBedState.Harvest:
                    harvestParticleSystem.Play();
                    break;
                default:
                    break;
            }
        }
    }

    private Item currentCropItem;
    private int CurrentCropId
    {
        get
        {
            return raisedBedData.currentCropId;
        }
        set
        {
            raisedBedData.currentCropId = value;
            if (value > 0)
            {
                currentCropItem = ItemDatabase.GetItemById(value);
            }
            else
            {
                currentCropItem = null;
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UiBuildingEditModeCanvas.OnToggleEditMode -= OnToggleBuildMode;
    }

    private void OnToggleBuildMode(bool inEditMode)
    {
        if (!isInBuildMenu)
        {
            boxCollider.isTrigger = !inEditMode;
        }
    }

    private void CompleteByGems()
    {
        raisedBedData.yieldDateTime = DateTime.UtcNow;
        raisedBedData.yieldDateTimeJson = JsonUtility.ToJson((JsonDateTime)raisedBedData.yieldDateTime);
        yieldTotalSeconds = 0;
        RaisedBedState = RaisedBedState.Harvest;
        int index = GetGrowthIndex();
        AssignSpriteByIndex(index);
        currentGrowthIndex = -1;
    }

    private IEnumerator TickPerSecond()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(1f);
            if (RaisedBedState == RaisedBedState.Growing)
            {
                if (yieldTotalSeconds <= 0)
                {
                    RaisedBedState = RaisedBedState.Harvest;
                }
                else
                {
                    yieldTotalSeconds = (int)raisedBedData.yieldDateTime.Subtract(DateTime.UtcNow).TotalSeconds;
                }
                int index = GetGrowthIndex();
                if (currentGrowthIndex != index)
                {
                    AssignSpriteByIndex(index);
                    currentGrowthIndex = index;
                }
            }
        }
    }

    public void InitRaisedBed(RaisedBedData raisedBedData, Source source, int bedIndex, bool isNewlyPlaced)
    {
        UiBuildingEditModeCanvas.OnToggleEditMode += OnToggleBuildMode;
        boxCollider = GetComponent<BoxCollider>();
        CreateCropLayout();
        harvestParticleSystem.Stop();
        this.bedIndex = bedIndex;
        transform.name = source.slug + bedIndex.ToString();
        this.raisedBedData = raisedBedData;
        transform.position = this.raisedBedData.pos;
        CurrentCropId = raisedBedData.currentCropId;
        this.raisedBedData.yieldDateTimeJson = raisedBedData.yieldDateTimeJson;
        this.raisedBedData.yieldDateTime = JsonUtility.FromJson<JsonDateTime>(raisedBedData.yieldDateTimeJson);

        if (CurrentCropId > 0)
        {
            CreateCropStages();
            yieldTotalSeconds = (int)raisedBedData.yieldDateTime.Subtract(DateTime.UtcNow).TotalSeconds;
            if (yieldTotalSeconds <= 0)
            {
                RaisedBedState = RaisedBedState.Harvest;
            }
            else
            {
                RaisedBedState = RaisedBedState.Growing;
            }
            StartCoroutine(AnimateShowHideCrops(true));
            int index = GetGrowthIndex();
            currentGrowthIndex = index;
            AssignSpriteByIndex(index);
        }
        else
        {
            RaisedBedState = RaisedBedState.Idle;
        }
        if (isNewlyPlaced)
        {
            isInBuildMenu = true;
            EnableEditMode();
        }
        StartCoroutine(TickPerSecond());
    }


    #region MovableUnitBase &Save Building position
    protected override void SavePositionData()
    {
        base.SavePositionData();
        raisedBedData.pos = transform.position;
    }

    protected override void AddThisNewBuilding()
    {
        base.AddThisNewBuilding();
        SaveLoadManager.AddNewRaisedBeds(raisedBedData);
        bedIndex = SaveLoadManager.saveData.raisedBedDatas.Count - 1;
    }
    #endregion


    #region Growing
    public void OnCropDroppedOnRaisedBed(int itemId)
    {
        CurrentCropId = itemId;
        int coinCost = ItemReceipesDatabase.GetCoinCostById(itemId);
        if (!PlayerCurrencyManager.ReduceCoins(coinCost))
        {
            //TODO: Show not enough coins window
            return;
        }
        PlayerCurrencyManager.ReduceEnergy(GameVariables.energy_raisedBed);
        //Set Crop Stages
        CreateCropStages();

        //Start growing stages
        RaisedBedState = RaisedBedState.Growing;
        raisedBedData.yieldDateTime = DateTime.UtcNow.AddMinutes(currentCropItem.yieldDurationInMins);
        raisedBedData.yieldDateTimeJson = JsonUtility.ToJson((JsonDateTime)raisedBedData.yieldDateTime);
        yieldTotalSeconds = (int)raisedBedData.yieldDateTime.Subtract(DateTime.UtcNow).TotalSeconds;
        //Set Crop sprites
        int index = GetGrowthIndex();
        currentGrowthIndex = index;
        AssignSpriteByIndex(index);

        StartCoroutine(AnimateShowHideCrops(true));
    }
    #endregion


    #region Harvest
    private void HarvestCrop()
    {
        RaisedBedState = RaisedBedState.Idle;
        StartCoroutine(AnimateShowHideCrops(false));

        DroppedItemManager.OnDropResource?.Invoke(currentCropItem, transform, yieldCropCount);

        //SaveLoadManager.AddUpdateItem(CurrentCropId, yieldCropCount);
        //PlayerCurrencyManager.AddXp(currentCropItem.xpOnHarvest);

        //UiResourceSpawnCanvas.OnHarvestResource?.Invoke(currentCropItem.slug, transform, yieldCropCount);
        //UiResourceSpawnCanvas.OnHarvestResource?.Invoke("xp", transform, currentCropItem.xpOnHarvest);

        CurrentCropId = 0;
    }
    #endregion


    #region Helper functions
    public int GetBedIndex()
    {
        return bedIndex;
    }

    private int GetGrowthIndex()
    {
        int growthIndex = 0;
        if (yieldTotalSeconds >= cropStages[2])
        {
            growthIndex = 1;
        }
        else if (yieldTotalSeconds.IsWithin(cropStages[1], cropStages[2]))
        {
            growthIndex = 2;
        }
        else if (yieldTotalSeconds.IsWithin(cropStages[0], cropStages[1]))
        {
            growthIndex = 3;
        }
        else if (yieldTotalSeconds <= cropStages[0] + 2)
        {
            growthIndex = 4;
        }
        return growthIndex;
    }

    private void AssignSpriteByIndex(int index)
    {
        if (!index.IsWithin(0, 5))
        {
            return;
        }
        Sprite cropSprite = AtlasBank.Instance.GetSpriteByName(currentCropItem.slug + index, AtlasType.UiItems);
        for (int i = 0; i < cropSprites.Length; i++)
        {
            cropSprites[i].sprite = cropSprite;
        }
    }

    private void CreateCropStages()
    {
        int totalSeconds = currentCropItem.yieldDurationInMins * 60;
        int interval = totalSeconds / 3;
        for (int i = 0; i < 4; i++)
        {
            cropStages[i] = interval * i;
        }
    }

    private void CreateCropLayout()
    {
        if (cropSprites == null || cropSprites.Length == 0)
        {
            int cropCount = RaisedBedCropLayout.layout2x2.Length;
            cropParents = new Transform[cropCount];
            cropSprites = new SpriteRenderer[cropCount];
            cropAnimators = new Animator[cropCount];
            for (int i = 0; i < cropCount; i++)
            {
                cropParents[i] = new GameObject().transform;
                cropParents[i].SetParent(cropParent);
                cropParents[i].localPosition = RaisedBedCropLayout.layout2x2[i];

                cropSprites[i] = Instantiate(Resources.Load<SpriteRenderer>(GameVariables.path_cropPrefabPath) as SpriteRenderer, cropParent);
                cropSprites[i].transform.SetParent(cropParents[i]);
                cropSprites[i].transform.localPosition = Vector3.zero;
                cropSprites[i].transform.localScale = new Vector3(1, 0, 1);

                cropAnimators[i] = cropSprites[i].GetComponent<Animator>();
                cropAnimators[i].Update(UnityEngine.Random.Range(0.1f, 2.5f));
            }
        }
    }

    public RaisedBedData GetRaisedBedData()
    {
        return raisedBedData;
    }
    #endregion


    #region Animate hide show crops
    private const float showYScale = 1;
    private const float hideYScale = 0;
    private const float animDuration = 0.5f;
    private IEnumerator AnimateShowHideCrops(bool isVisible)
    {
        float pos = isVisible ? showYScale : hideYScale;
        if (cropSprites != null)
        {
            for (int i = 0; i < cropSprites.Length; i++)
            {
                cropSprites[i].transform.DOScaleY(pos, animDuration);
                yield return new WaitForEndOfFrame();
            }
        }
        yield return new WaitForEndOfFrame();
    }
    #endregion


    #region IIneractable
    public void InteractOnClick()
    {
        OnRaisedBedClicked?.Invoke(bedIndex);
        switch (RaisedBedState)
        {
            case RaisedBedState.Idle:
                UiCropsCanvas.OnShowCanvas?.Invoke(bedIndex);
                break;
            case RaisedBedState.Growing:
                UiTimeRemainingCanvas.OnShowCanvas?.Invoke(raisedBedData.currentCropId, raisedBedData.pos, raisedBedData.yieldDateTime, CompleteByGems);
                break;
            case RaisedBedState.Harvest:
                HarvestCrop();
                break;
            default:
                break;
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public Vector3 GetColliderSize()
    {
        return boxCollider.size;
    }

    public IneractableType GetIneractableType()
    {
        return IneractableType.RaisedBed;
    }
    #endregion


    #region IShakeable
    public void Shake()
    {
        for (int i = 0; i < cropParents.Length; i++)
        {
            cropParents[i].DOPunchRotation(new Vector3(0, 0, 10), 1, 4, 1);
        }
    }
    #endregion
}