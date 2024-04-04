using UnityEngine;
using System;

public class UiResourceSpawnCanvas : MonoBehaviour
{
    public static Action<string, Transform, int> OnHarvestResource;
    //public static Action<Item, Transform, int> OnDropItemOnGround;//item, world position, count
    //public static Action<Transform, int> OnHarvest1ResourceByItemId;
    [SerializeField] private RectTransform xpPosition;
    [SerializeField] private RectTransform inventoryPosition;
    [SerializeField] private RectTransform backpackPosition;
    [SerializeField] private UiResourceSpawn uiResourceSpawnPrefab;
    private Camera mainCamera;
    private Canvas mainCanvas;
    private RectTransform canvasRect;
    private RectTransform itemPosition;

    private void Start()
    {
        SceneLoader.OnSceneChanged += OnSceneChanged;
        OnHarvestResource += HarvestResource;
        //OnHarvest1ResourceByItemId += Harvest1ResourceByItemId;
        mainCanvas = GetComponent<Canvas>();
        canvasRect = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    private void OnDestroy()
    {
        SceneLoader.OnSceneChanged -= OnSceneChanged;
        OnHarvestResource -= HarvestResource;
        //OnHarvest1ResourceByItemId -= Harvest1ResourceByItemId;
    }

    private void OnSceneChanged(Scenes scene)
    {
        switch (scene)
        {
            case Scenes.Loading:
                break;
            case Scenes.FarmHome:
                itemPosition = inventoryPosition;
                break;
            case Scenes.Mines:
                itemPosition = backpackPosition;
                break;
            case Scenes.Forest:
                itemPosition = backpackPosition;
                break;
            case Scenes.Town:
                itemPosition = backpackPosition;
                break;
            default:
                break;
        }
    }

    private void HarvestResource(string slug, Transform worldTransform, int value)
    {
        UiResourceSpawn uiResourceSpawn = Instantiate(uiResourceSpawnPrefab, transform);
        if (slug == "xp")
        {
            if (value > 0) // Show xp only if value is > 0
            {
                uiResourceSpawn.InitItem(slug, worldTransform, mainCanvas, xpPosition.localPosition, mainCamera, value);
            }
        }
        else
        {
            uiResourceSpawn.InitItem(slug, worldTransform, mainCanvas, itemPosition.localPosition, mainCamera, value);
        }
    }
}