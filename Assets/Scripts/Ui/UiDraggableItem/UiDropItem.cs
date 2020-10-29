using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiDropItem : MonoBehaviour, IDropHandler
{
    public Action OnDropItem;
    public void OnDrop(PointerEventData eventData)
    {
        OnDropItem?.Invoke();
    }
}