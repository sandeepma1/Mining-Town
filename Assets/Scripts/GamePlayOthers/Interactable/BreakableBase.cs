using System;
using UnityEngine;
using MiningTown.IO;

public class BreakableBase : MonoBehaviour, IBreakable
{
    public Action<BreakableBase, bool> OnBreakableDied;
    public Transform breakableMesh;
    [SerializeField] private Transform uiHealthBarPosition;
    private float health;
    private HealthBar healthBar;
    public bool isLadder = false;
    private Mineral mineral;

    private void Start()
    {
        healthBar = Instantiate(PrefabBank.Instance.GetHealthBarPrefab(), uiHealthBarPosition);
        healthBar.transform.localPosition = Vector3.zero;
    }

    public void Init(Mineral mineral, Vector2Int pos)
    {
        transform.localPosition = new Vector3(pos.x, 0, pos.y);
        this.mineral = mineral;
        health = mineral.hitPoints;
    }

    public void TakeHit(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            OnBreakableDied?.Invoke(this, isLadder);
            MiningLevelGenerator.OnBakeNavMesh?.Invoke();
            if (mineral.outputItemId > 0)
            {
                int dropCount = UnityEngine.Random.Range(mineral.minDrop, mineral.maxDrop + 1);
                if (dropCount > 0)
                {
                    DroppedItemManager.OnDropResource(ItemDatabase.GetItemById(mineral.outputItemId), transform, dropCount);
                }
            }

            Destroy(this.gameObject, 0.2f);
        }
        else
        {
            healthBar.OnHealthChanged(health / mineral.hitPoints);
        }
    }


    #region IBreakable
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
