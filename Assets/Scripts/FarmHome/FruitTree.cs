using System;
using System.Collections;
using UnityEngine;
using MiningTown.IO;

public class FruitTree : MovableUnitBase, IInteractable
{
    [SerializeField] private Transform showUiPosition;
    [SerializeField] private GameObject[] fruitsQuad;
    [SerializeField] private Renderer[] treeLeaves;
    //[SerializeField] private ParticleSystem harvestParticleSystem;
    private int yieldTotalSeconds;
    private FruitTreeData fruitTreeData;
    private Item currentFruidTreeItem;
    private int yieldFruitCount = 3;
    private Vector3 boxColliderSize;
    private int treeIndex;

    private FruitTreeState FruitTreeState
    {
        get
        {
            return fruitTreeData.fruitTreeState;
        }
        set
        {
            fruitTreeData.fruitTreeState = value;
            switch (fruitTreeData.fruitTreeState)
            {
                case FruitTreeState.Growing:
                    ToggleFruits(false);
                    break;
                case FruitTreeState.Harvest:
                    ToggleFruits(true);
                    break;
                case FruitTreeState.Dead:
                    OnDead();
                    break;
                default:
                    break;
            }
        }
    }

    internal void InitFruitTree(FruitTreeData fruitTreeData, Source source, int treeIndex, bool isNewlyPlaced)
    {
        boxColliderSize = GetComponent<BoxCollider>().size;
        currentFruidTreeItem = ItemDatabase.GetItemById(ItemReceipesDatabase.GetFruitTreeReceipeBySourceId(source.id).outputItemId);
        transform.position = fruitTreeData.pos;
        transform.name = source.slug + treeIndex.ToString();
        this.treeIndex = treeIndex;
        this.fruitTreeData = fruitTreeData;
        this.fruitTreeData.yieldDateTimeJson = fruitTreeData.yieldDateTimeJson;
        this.fruitTreeData.yieldDateTime = JsonUtility.FromJson<JsonDateTime>(fruitTreeData.yieldDateTimeJson);

        if (fruitTreeData.maxHarvestTimes > 0)
        {
            yieldTotalSeconds = (int)fruitTreeData.yieldDateTime.Subtract(DateTime.UtcNow).TotalSeconds;
            if (yieldTotalSeconds <= 0)
            {
                FruitTreeState = FruitTreeState.Harvest;
            }
            else
            {
                FruitTreeState = FruitTreeState.Growing;
            }
            StartCoroutine(TickPerSecond());
        }
        else
        {
            FruitTreeState = FruitTreeState.Dead;
        }
        if (isNewlyPlaced)
        {
            isInBuildMenu = true;
            EnableEditMode();
        }
    }

    private void OnDead()
    {
        ToggleFruits(false);
        for (int i = 0; i < treeLeaves.Length; i++)
        {
            treeLeaves[i].material.color = ColorConstants.FruitTreesDead;
        }
    }

    private void TreeRemovedByCoin()
    {
        SaveLoadManager.RemoveFruitTree(fruitTreeData);
        Destroy(gameObject);
    }

    private void CompleteByGems()
    {
        fruitTreeData.yieldDateTime = DateTime.UtcNow;
        fruitTreeData.yieldDateTimeJson = JsonUtility.ToJson((JsonDateTime)fruitTreeData.yieldDateTime);
        yieldTotalSeconds = 0;
        FruitTreeState = FruitTreeState.Harvest;
    }

    private void OnHarvestClicked()
    {
        if (fruitTreeData.maxHarvestTimes <= 0)
        {
            FruitTreeState = FruitTreeState.Dead;
            StopCoroutine(TickPerSecond());
        }
        else
        {
            FruitTreeState = FruitTreeState.Growing;
            fruitTreeData.yieldDateTime = DateTime.UtcNow.AddMinutes(currentFruidTreeItem.yieldDurationInMins);
            fruitTreeData.yieldDateTimeJson = JsonUtility.ToJson((JsonDateTime)fruitTreeData.yieldDateTime);
            yieldTotalSeconds = (int)fruitTreeData.yieldDateTime.Subtract(DateTime.UtcNow).TotalSeconds;

            DroppedItemManager.OnDropResource(currentFruidTreeItem,
                PlayerMovement.Instance.GetPlayerTransform(), yieldFruitCount);
            //SaveLoadManager.AddUpdateItem(currentFruidTreeItem.itemId, yieldFruitCount);
            //PlayerCurrencyManager.AddXp(currentFruidTreeItem.xpOnHarvest);
        }
        fruitTreeData.maxHarvestTimes--;
    }


    #region Save Building position
    protected override void SavePositionData()
    {
        base.SavePositionData();
        fruitTreeData.pos = transform.position;
    }
    #endregion

    private IEnumerator TickPerSecond()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(1f);
            if (FruitTreeState == FruitTreeState.Growing)
            {
                if (yieldTotalSeconds <= 0)
                {
                    FruitTreeState = FruitTreeState.Harvest;
                }
                else
                {
                    yieldTotalSeconds = (int)fruitTreeData.yieldDateTime.Subtract(DateTime.UtcNow).TotalSeconds;
                }
            }
        }
    }

    private void ToggleFruits(bool isVisible)
    {
        for (int i = 0; i < fruitsQuad.Length; i++)
        {
            fruitsQuad[i].SetActive(isVisible);
        }
    }


    #region IIneractable
    public void InteractOnClick()
    {
        switch (FruitTreeState)
        {
            case FruitTreeState.Growing:
                UiTimeRemainingCanvas.OnShowCanvas?.Invoke
                    (currentFruidTreeItem.itemId, showUiPosition.position, fruitTreeData.yieldDateTime, CompleteByGems);
                break;
            case FruitTreeState.Harvest:
                OnHarvestClicked();
                break;
            case FruitTreeState.Dead:
                UiRemoveByCoinCanvas.OnShowCanvas?.Invoke(currentFruidTreeItem.itemId, showUiPosition.position, TreeRemovedByCoin);
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
        return boxColliderSize;
    }

    public IneractableType GetIneractableType()
    {
        return IneractableType.FruitTree;
    }
    #endregion
}