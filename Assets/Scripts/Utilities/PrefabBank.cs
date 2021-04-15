using UnityEngine;

public class PrefabBank : Singleton<PrefabBank>
{
    [SerializeField] private HealthBar healthBarPrefab;

    protected override void Awake()
    {
        base.Awake();
    }

    public HealthBar GetHealthBarPrefab()
    {
        return healthBarPrefab;
    }
}