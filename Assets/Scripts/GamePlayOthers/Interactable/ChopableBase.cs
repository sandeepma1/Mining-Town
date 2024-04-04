using System;
using MiningTown.IO;
using UnityEngine;

public class ChopableBase : MonoBehaviour, IChopable
{
    public Action<ChopableBase> OnChopableDied;
    [SerializeField] private Transform uiHealthBarPosition;
    private float health;
    private HealthBar healthBar;
    private ForestObject forestObject;

    private void Start()
    {
        healthBar = Instantiate(PrefabBank.Instance.GetHealthBarPrefab(), uiHealthBarPosition);
        healthBar.transform.localPosition = Vector3.zero;
    }

    public void Init(ForestObject forestObject, Vector2Int pos)
    {
        transform.localPosition = new Vector3(pos.x, 0, pos.y);
        this.forestObject = forestObject;
        health = forestObject.hitPoints;
    }

    public void TakeHit(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            OnChopableDied?.Invoke(this);
            MiningLevelGenerator.OnBakeNavMesh?.Invoke();
            if (forestObject.outputItemId > 0)
            {
                int dropCount = UnityEngine.Random.Range(forestObject.minDrop, forestObject.maxDrop + 1);
                if (dropCount > 0)
                {
                    DroppedItemManager.OnDropResource(ItemDatabase.GetItemById(forestObject.outputItemId), transform, dropCount);
                }
            }
            Destroy(this.gameObject, 0.2f);
        }
        else
        {
            healthBar.OnHealthChanged(health / forestObject.hitPoints);
        }
    }


    #region IChopable
    public Transform GetTransform()
    {
        return transform;
    }

    public void Hit(int damage)
    {
        TakeHit(damage);
    }
    #endregion
}
