using System.Collections.Generic;
using MiningTown.IO;
using UnityEngine;

public class ProdBuildingsManager : MonoBehaviour
{

    private List<ProdBuilding> prodBuildings = new List<ProdBuilding>();
    private int currentSelectedprodBuildingIndex = -1;

    private void Start()
    {
        UiBuildCanvas.OnAddNewProdBuilding += OnAddNewProdBuilding;
        UiProdBuildingCanvas.OnItemDroppedOnProdBuilding += OnItemDroppedOnProdBuilding;
        InitProdBuildings();
    }

    private void OnDestroy()
    {
        UiBuildCanvas.OnAddNewProdBuilding += OnAddNewProdBuilding;
        UiProdBuildingCanvas.OnItemDroppedOnProdBuilding -= OnItemDroppedOnProdBuilding;
    }

    private void OnAddNewProdBuilding(ProdBuildingData prodBuildingData)
    {
        CreateNewProdBuilding(prodBuildingData, SaveLoadManager.GetProdBuildingsCount() - 1, true);
    }

    private void InitProdBuildings()
    {
        List<ProdBuildingData> prodBuildingDatas = SaveLoadManager.GetAllProdBuildings();
        for (int i = 0; i < prodBuildingDatas.Count; i++)
        {
            CreateNewProdBuilding(prodBuildingDatas[i], i, false);
        }
    }

    private void CreateNewProdBuilding(ProdBuildingData prodBuildingData, int index, bool isNewlyPlaced)
    {
        Source prodBuildingSourceInfo = SourceDatabase.GetSourceInfoById(prodBuildingData.sourceId);
        ProdBuilding prodBuilding = Instantiate(Resources.Load<ProdBuilding>
            (GameVariables.path_sourcePrefabFolder + prodBuildingSourceInfo.slug) as ProdBuilding, transform);
        prodBuilding.InitProdBuilding(prodBuildingData, prodBuildingSourceInfo, index, isNewlyPlaced);
        prodBuilding.OnProdBuildingClicked += OnProdBuildingClicked;
        prodBuildings.Add(prodBuilding);
    }

    private void OnItemDroppedOnProdBuilding(int itemId)
    {
        prodBuildings[currentSelectedprodBuildingIndex].OnItemDroppedOnProdBuilding(itemId);
    }

    private void OnProdBuildingClicked(int buildingIndex)
    {
        currentSelectedprodBuildingIndex = buildingIndex;
    }
}

public enum ProdBuildingState
{
    Idle,
    Working,
}