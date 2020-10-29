using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour, IPointerClickHandler
{
    public Action<int, int> OnCraftingSlotSelected;
    public Image slotIcon;
    public int slotId;
    public int itemId;

    public void InitializeSlot(int itemId, int id)
    {
        slotId = id;
        this.itemId = itemId;
        slotIcon.sprite = ItemsDatabase.GetSpriteByItemId(itemId);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCraftingSlotSelected?.Invoke(itemId, slotId);
    }
}