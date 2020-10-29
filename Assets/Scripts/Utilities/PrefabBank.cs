using UnityEngine;

public class PrefabBank : Singleton<PrefabBank>
{
    [SerializeField] private HealthBar healthBarPrefab;
    [SerializeField] private DroppedItem droppedItemPrefab;

    protected override void Awake()
    {
        base.Awake();
    }

    public HealthBar GetHealthBarPrefab()
    {
        return healthBarPrefab;
    }

    public DroppedItem GetDroppedItemPrefab()
    {
        return droppedItemPrefab;
    }
}