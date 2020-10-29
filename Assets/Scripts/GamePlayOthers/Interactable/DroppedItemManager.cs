using System;
using UnityEngine;
using System.Collections;

public class DroppedItemManager : MonoBehaviour
{
    public static Action<Item, Transform, int> OnDropResource;
    [SerializeField] private DroppedItem droppedItemPrefab;
    private bool isAtFarm;

    private void Awake()
    {
        OnDropResource += DropResource;
        SceneLoader.OnSceneChanged += OnSceneChanged;
    }

    private void OnDestroy()
    {
        OnDropResource -= DropResource;
        SceneLoader.OnSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scenes scenes)
    {
        switch (scenes)
        {
            case Scenes.Town:
            case Scenes.Forest:
            case Scenes.Mines:
            case Scenes.Loading:
                isAtFarm = false;
                break;
            case Scenes.FarmHome:
                isAtFarm = true;
                break;
            default:
                break;
        }
    }

    private void DropResource(Item itemToDrop, Transform worldTransform, int count)
    {
        StartCoroutine(DropItem(itemToDrop, worldTransform, count));
        if (itemToDrop.xpOnHarvest > 0)
        {
            StartCoroutine(DropXp(worldTransform, count));
        }
    }

    private IEnumerator DropItem(Item itemToDrop, Transform worldTransform, int count)
    {
        for (int i = 0; i < count; i++)
        {
            DroppedItem droppedItem = Instantiate(droppedItemPrefab, worldTransform.position, Quaternion.identity, this.transform);
            droppedItem.InitItem(itemToDrop, count, isAtFarm);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator DropXp(Transform worldTransform, int count)
    {
        // Always drop Xp after a while
        yield return new WaitForSeconds(0.15f);
        DroppedItem droppedItem = Instantiate(droppedItemPrefab, worldTransform.position, Quaternion.identity, this.transform);
        droppedItem.InitXp(count);
    }
}