using System.Collections.Generic;
using UnityEngine;
using MiningTown.IO;

public class DecorationManager : MonoBehaviour
{
    private List<Decoration> decorations = new List<Decoration>();

    private void Start()
    {
        UiBuildCanvas.OnAddNewDecoration += OnAddNewDecoration;
        InitAllDecorations();
    }

    private void OnDestroy()
    {
        UiBuildCanvas.OnAddNewDecoration -= OnAddNewDecoration;
    }

    private void InitAllDecorations()
    {
        List<DecorationData> decorationDatas = SaveLoadManager.GetAllDecorations();
        for (int i = 0; i < decorationDatas.Count; i++)
        {
            CreateNewDecoration(decorationDatas[i], false);
        }
    }

    private void OnAddNewDecoration(DecorationData decorationData)
    {
        CreateNewDecoration(decorationData, true);
    }

    private void CreateNewDecoration(DecorationData decorationData, bool isNewelyPlaced)
    {
        Decoration decoration = Instantiate(Resources.Load<Decoration>
               (GameVariables.path_decorationPrefabFolder + DecorationDatabase.GetDecorationSlugById(decorationData.decoId))
               as Decoration, transform);
        decoration.InitDecoration(decorationData, isNewelyPlaced);
        decorations.Add(decoration);
    }
}