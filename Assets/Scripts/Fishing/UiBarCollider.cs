using System;
using UnityEngine;

public class UiBarCollider : MonoBehaviour
{
    public Action<bool> OnFishTriggerStay;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnFishTriggerStay?.Invoke(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnFishTriggerStay?.Invoke(false);
    }
}