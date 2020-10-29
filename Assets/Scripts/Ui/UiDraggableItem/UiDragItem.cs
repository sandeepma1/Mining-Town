using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using MiningTown.IO;

public class UiDragItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Action<int, Sprite> OnDragStart;
    public Action<Vector2> OnDragItemPosition;
    public Action OnDragEnd;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI inInventoryCountText;
    private ScrollRect scrollRect;
    private bool isDragging;
    private Vector2 touchPos;
    private int itemId;
    private DescriptionType descriptionType;

    public void Init(DescriptionType descriptionType, int itemId, Sprite itemSprite, ScrollRect scrollRect)
    {
        this.descriptionType = descriptionType;
        this.itemId = itemId;
        this.scrollRect = scrollRect;
        itemImage.sprite = itemSprite;
    }

    public void UpdateCountText()
    {
        inInventoryCountText.text = GameVariables.tmp_invIcon + SaveLoadManager.DoesItemExistsReturnCount(itemId).ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        HideItemDescCanvas();
        isDragging = false;
        touchPos = eventData.position;
        ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.beginDragHandler);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            OnDragItemPosition?.Invoke(eventData.position);
            return;
        }
        Vector2 heading = touchPos - eventData.position;
        Vector2 direction = heading / heading.magnitude;
        //Dragging in up direction
        if (direction.y <= -0.9)
        {
            isDragging = true;
            OnDragStart?.Invoke(itemId, itemImage.sprite);
            itemImage.color = Color.gray;
        }
        else
        {
            ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.dragHandler);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnDragEnd?.Invoke();
        isDragging = false;
        itemImage.color = Color.white;
        ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.endDragHandler);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Invoke("ShowItemDescCanvas", GameVariables.touchHoldDescDuration);
    }

    //Dont delete this unsed function as it is used by Invoke
    private void ShowItemDescCanvas()
    {
        switch (descriptionType)
        {
            case DescriptionType.Crop:
                UiItemDescriptionCanvas.OnShowCropDescription?.Invoke(itemId);
                break;
            case DescriptionType.Receipe:
                UiItemDescriptionCanvas.OnShowReceipeDescription?.Invoke(itemId);
                break;
            case DescriptionType.Item:
                UiItemDescriptionCanvas.OnShowItemDescription?.Invoke(itemId);
                break;
            default:
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        HideItemDescCanvas();
    }

    private void HideItemDescCanvas()
    {
        CancelInvoke("ShowItemDescCanvas");
        UiItemDescriptionCanvas.OnHideCanvas?.Invoke();
    }
}