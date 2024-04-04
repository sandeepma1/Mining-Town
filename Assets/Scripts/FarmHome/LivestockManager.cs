using System;
using System.Collections.Generic;
using MiningTown.IO;
using UnityEngine;

public class LivestockManager : MonoBehaviour
{
    private List<Livestock> livestockBuildings = new List<Livestock>();
    private int currentSelectedlivestockIndex = -1;

    private void Start()
    {
        UiBuildCanvas.OnAddNewLivestockBuilding += OnAddNewLivestockBuilding;
        UiLiveStockCanvas.OnItemDroppedOnLivestockBuilding += OnItemDroppedOnLivestockBuilding;
        InitProdBuildings();
    }

    private void OnDestroy()
    {
        UiBuildCanvas.OnAddNewLivestockBuilding -= OnAddNewLivestockBuilding;
        UiLiveStockCanvas.OnItemDroppedOnLivestockBuilding -= OnItemDroppedOnLivestockBuilding;
    }

    private void OnAddNewLivestockBuilding(LivestockData livestockData)
    {
        CreateNewLivestockBuilding(livestockData, SaveLoadManager.GetLivestocksCount() - 1, true);
    }

    private void InitProdBuildings()
    {
        List<LivestockData> livestockDatas = SaveLoadManager.GetAllLivestocks();
        for (int i = 0; i < livestockDatas.Count; i++)
        {
            CreateNewLivestockBuilding(livestockDatas[i], i, false);
        }
    }

    private void CreateNewLivestockBuilding(LivestockData livestockData, int index, bool isNewlyPlaced)
    {
        Source livestockSourceInfo = SourceDatabase.GetSourceInfoById(livestockData.sourceId);
        Livestock livestock = Instantiate(Resources.Load<Livestock>
            (GameVariables.path_sourcePrefabFolder + livestockSourceInfo.slug) as Livestock, transform);
        livestock.InitLivestock(livestockData, livestockSourceInfo, index, isNewlyPlaced);
        livestock.OnLivestockBuildingClicked += OnLivestockBuildingClicked;
        livestockBuildings.Add(livestock);
    }

    private void OnItemDroppedOnLivestockBuilding(int itemId)
    {
        print(itemId);
        //livestockBuildings[currentSelectedlivestockIndex].OnItemDroppedOnProdBuilding(itemId);
    }

    private void OnLivestockBuildingClicked(int buildingIndex)
    {
        currentSelectedlivestockIndex = buildingIndex;
    }
}

public enum LivestockState
{
    Hungry,
    Eating,
    Harvest
}