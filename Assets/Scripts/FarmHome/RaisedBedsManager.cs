using System;
using System.Collections.Generic;
using MiningTown.IO;
using UnityEngine;

public class RaisedBedsManager : MonoBehaviour
{
    private List<RaisedBed> raisedBeds = new List<RaisedBed>();
    private Source raisedBedSourceInfo;
    private int currentSelectedRaisedBedIndex = -1;
    private List<int> nearbyScannedBedIndexes = new List<int>();

    private void Start()
    {
        UiBuildCanvas.OnAddNewRaisedBed += OnAddNewRaisedBed;
        UiCropsCanvas.OnCropDroppedOnRaisedBed += OnCropDroppedOnRaisedBed;
        PlayerInteraction.OnNearbyScannedBedIndexes += OnNearbyScannedBedIndexes;
        raisedBedSourceInfo = SourceDatabase.GetSourceInfoById(1);
        InitRaisedBeds();
    }

    private void OnDestroy()
    {
        UiBuildCanvas.OnAddNewRaisedBed -= OnAddNewRaisedBed;
        UiCropsCanvas.OnCropDroppedOnRaisedBed -= OnCropDroppedOnRaisedBed;
    }

    private void InitRaisedBeds()
    {
        List<RaisedBedData> raisedBedDatas = SaveLoadManager.GetAllRaisedBeds();
        for (int i = 0; i < raisedBedDatas.Count; i++)
        {
            CreateNewRaisedBed(raisedBedDatas[i], i, false);
        }
    }

    private void CreateNewRaisedBed(RaisedBedData raisedBedData, int index, bool isNewlyPlace)
    {
        RaisedBed raisedBed = Instantiate(Resources.Load<RaisedBed>(GameVariables.path_sourcePrefabFolder + raisedBedSourceInfo.slug) as RaisedBed, transform);
        raisedBed.InitRaisedBed(raisedBedData, raisedBedSourceInfo, index, isNewlyPlace);
        raisedBed.OnRaisedBedClicked += OnRaisedBedClicked;
        raisedBeds.Add(raisedBed);
    }

    private void OnNearbyScannedBedIndexes(List<int> obj)
    {
        nearbyScannedBedIndexes = obj;
    }

    private void OnAddNewRaisedBed(RaisedBedData raisedBedData)
    {
        CreateNewRaisedBed(raisedBedData, SaveLoadManager.GetRaisedBedsCount() - 1, true);
    }

    private void OnCropDroppedOnRaisedBed(int itemId)
    {
        if (raisedBeds == null)
        {
            InitRaisedBeds();
        }

        for (int i = 0; i < nearbyScannedBedIndexes.Count; i++)
        {
            if (!PlayerCurrencyManager.HaveEnergy(GameVariables.energy_raisedBed))
            {
                break;
            }
            raisedBeds[nearbyScannedBedIndexes[i]].OnCropDroppedOnRaisedBed(itemId);
        }

        //List<int> idleIndexes = new List<int>();
        //for (int i = 0; i < raisedBeds.Count; i++)
        //{
        //    if (raisedBeds[i].RaisedBedState == RaisedBedState.Idle) { idleIndexes.Add(i); }
        //}

        //switch (raisedBedDropRate)
        //{
        //    case RaisedBedDropRate.Single:
        //        raisedBeds[currentSelectedRaisedBedIndex].OnCropDroppedOnRaisedBed(itemId);
        //        break;
        //    case RaisedBedDropRate.TwoByTwo:
        //        MultiPlatCropsOnBeds(idleIndexes, 4, itemId);
        //        break;
        //    case RaisedBedDropRate.ThreeByThree:
        //        MultiPlatCropsOnBeds(idleIndexes, 2, itemId);
        //        break;
        //    default:
        //        break;
        //}
    }

    private void MultiPlatCropsOnBeds(List<int> idleIndexes, int nos, int itemId)
    {
        int count = idleIndexes.Count / nos;
        if (count <= 1)
        {
            raisedBeds[currentSelectedRaisedBedIndex].OnCropDroppedOnRaisedBed(itemId);
            return;
        }
        raisedBeds[currentSelectedRaisedBedIndex].OnCropDroppedOnRaisedBed(itemId);
        for (int i = 1; i < count; i++)
        {
            raisedBeds[idleIndexes[i]].OnCropDroppedOnRaisedBed(itemId);
        }
    }

    private void OnRaisedBedClicked(int bedIndex)
    {
        currentSelectedRaisedBedIndex = bedIndex;
    }
}

public enum RaisedBedState
{
    Idle,
    Growing,
    Harvest
}

public enum RaisedBedDropRate
{
    Single,
    TwoByTwo,
    ThreeByThree
}