using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiDragDetector : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public static Action<bool> OnDragEvent;
    public static Action<Vector2> OnBeginDragEvent;

    private void Awake()
    {
        GetComponent<Image>().raycastTarget = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        OnDragEvent?.Invoke(true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnDragEvent?.Invoke(true);
        OnBeginDragEvent?.Invoke(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnDragEvent?.Invoke(false);
    }
}