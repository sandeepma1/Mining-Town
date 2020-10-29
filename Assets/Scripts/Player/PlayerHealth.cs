using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public static Action<Transform> OnPlayerHealthBarPosition;
    public static Action<bool> OnToggleGodMode;
    [SerializeField] private Transform healthBarPosition;
    [SerializeField] private LayerMask monsterLayer;

    private void Start()
    {
        OnToggleGodMode += ToggleGodMode;
        OnToggleGodMode?.Invoke(SaveLoadManager.saveData.playerStats.isInGodMode);
        OnPlayerHealthBarPosition?.Invoke(healthBarPosition);
    }

    private void OnDestroy()
    {
        OnToggleGodMode -= ToggleGodMode;
    }

    private void ToggleGodMode(bool godMode)
    {
        SaveLoadManager.saveData.playerStats.isInGodMode = godMode;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (monsterLayer == (monsterLayer | (1 << collision.gameObject.layer)))
        //{
        //    //Damage taken by player
        //    IMonster monster = collision.gameObject.GetComponent<IMonster>();
        //    if (monster != null)
        //    {
        //        TakeDamage(monster.GetDamageValue());
        //    }
        //    ITrap trap = collision.gameObject.GetComponent<ITrap>();
        //    if (trap != null)
        //    {
        //        TakeDamage(trap.GetDamageValue());
        //        trap.OnDamageGivenToPlayer();
        //    }
        //}
    }
}