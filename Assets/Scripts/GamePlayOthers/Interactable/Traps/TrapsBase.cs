using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapsBase : MonoBehaviour, ITrap
{
    [SerializeField] private int damageToPlayer = 2;

    protected virtual void Start()
    {

    }

    public float GetDamageValue()
    {
        return damageToPlayer;
    }

    public void OnDamageGivenToPlayer()
    {
        DamageGivenToPlayer();
    }

    protected virtual void DamageGivenToPlayer()
    {

    }
}