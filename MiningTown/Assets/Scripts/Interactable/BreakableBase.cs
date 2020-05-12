using System;
using UnityEngine;

public class BreakableBase : MonoBehaviour
{
    public Action<BreakableBase> OnBreakableDied;
    public Transform breakableMesh;
    [SerializeField] private Transform uiHealthBarPosition;
    [SerializeField] private float maxHealth = 5;
    private float health;
    private HealthBar healthBar;

    private void Start()
    {
        health = maxHealth;
        healthBar = Instantiate(PrefabBank.Instance.GetHealthBarPrefab(), uiHealthBarPosition);
        healthBar.transform.localPosition = Vector3.zero;
    }

    public void TakeHit()
    {
        health--;
        if (health <= 0)
        {
            OnBreakableDied?.Invoke(this);
            LevelGenerator.OnBakeNavMesh?.Invoke();
            Destroy(this.gameObject);
        }
        else
        {
            healthBar.OnHealthChanged(health / maxHealth);
        }
    }
}
