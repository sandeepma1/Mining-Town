using System.Collections.Generic;
using MiningTown.IO;
using UnityEngine;

public class FruitTreeManager : MonoBehaviour
{
    private List<FruitTree> fruitTrees = new List<FruitTree>();

    private void Start()
    {
        UiBuildCanvas.OnAddFruitTree += OnAddFruitTree;
        InitFruitTrees();
    }

    private void OnDestroy()
    {
        UiBuildCanvas.OnAddFruitTree -= OnAddFruitTree;
    }

    private void OnAddFruitTree(FruitTreeData fruitTreeData)
    {
        SaveLoadManager.AddNewFruitTree(fruitTreeData);
        CreateNewFruitTree(fruitTreeData, SaveLoadManager.GetFruitTreesCount() - 1, true);
    }

    private void InitFruitTrees()
    {
        List<FruitTreeData> fruitTreeDatas = SaveLoadManager.GetAllFruitTrees();
        for (int i = 0; i < fruitTreeDatas.Count; i++)
        {
            CreateNewFruitTree(fruitTreeDatas[i], i, false);
        }
    }

    private void CreateNewFruitTree(FruitTreeData fruitTreeData, int index, bool isNewlyPlace)
    {
        Source fruitTreeSourceInfo = SourceDatabase.GetSourceInfoById(fruitTreeData.sourceId);
        FruitTree fruitTree = Instantiate(Resources.Load<FruitTree>(GameVariables.path_sourcePrefabFolder + fruitTreeSourceInfo.slug) as FruitTree, transform);
        fruitTree.InitFruitTree(fruitTreeData, fruitTreeSourceInfo, index, isNewlyPlace);
        fruitTrees.Add(fruitTree);
    }
}

public enum FruitTreeState
{
    Growing,
    Harvest,
    Dead
}