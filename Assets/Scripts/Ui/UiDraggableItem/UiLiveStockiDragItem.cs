using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class UiLiveStockiDragItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Action<int, Sprite> OnDragStart;
    public Action<Vector2> OnDragItemPosition;
    public Action OnDragEnd;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI inInventoryCountText;
    private bool isDragging;
    private int requiredIndex;
    private int itemId;
    private int requiredCount;

    public void Init(int requiredIndex, int itemId, int requiredCount)
    {
        this.requiredIndex = requiredIndex;
        this.itemId = itemId;
        this.requiredCount = requiredCount;
        itemImage.sprite = AtlasBank.Instance.GetSpriteByItemId(itemId);
    }

    public void UpdateCountText()
    {
        if (requiredIndex == 2)
        {
            inInventoryCountText.text = GameVariables.tmp_coinIcon + requiredCount.ToString();
        }
        else
        {
            inInventoryCountText.text = GameVariables.tmp_invIcon + requiredCount
                + "/"
                + SaveLoadManager.DoesItemExistsReturnCount(itemId);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        HideItemDescCanvas();
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            OnDragItemPosition?.Invoke(eventData.position);
            return;
        }
        isDragging = true;
        OnDragStart?.Invoke(requiredIndex, itemImage.sprite);
        itemImage.color = Color.gray;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnDragEnd?.Invoke();
        isDragging = false;
        itemImage.color = Color.white;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Invoke("ShowItemDescCanvas", GameVariables.touchHoldDescDuration);
    }

    //Dont delete this unsed function as it is used by Invoke
    private void ShowItemDescCanvas()
    {
        if (requiredIndex == 2)
        {
            UiItemDescriptionCanvas.OnShowPurchaseItemDescription?.Invoke(itemId, requiredCount);
        }
        else
        {
            UiItemDescriptionCanvas.OnShowItemDescription?.Invoke(itemId);
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