using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UiSlotItem : MonoBehaviour
{
    public Action<int, int, int> OnSlotClicked;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemCountText;
    private Button button;
    private int slotIndex;
    private int itemId;
    private int ItemCount
    {
        get
        {
            return itemCount;
        }
        set
        {
            itemCount = value;
            itemCountText.text = value.ToString();
            if (value <= 0)
            {
                itemId = 0;
                itemImage.sprite = AtlasBank.Instance.GetSpriteByName("Blank", AtlasType.UiItems);
                itemCountText.text = "";
            }
        }
    }
    private int itemCount;

    private void Start()
    {
        //print("UiSlotItem Start");
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }

    public void InitEmptySlots(int slotIndex)
    {
        this.slotIndex = slotIndex;
    }

    public void InitItemData(int slotIndex, ItemIdWithCount item)
    {
        this.slotIndex = slotIndex;
        AddNewItemToSlot(item);
    }

    public void OnButtonClicked()
    {
        OnSlotClicked?.Invoke(slotIndex, itemId, ItemCount);
    }

    public void AddNewItemToSlot(ItemIdWithCount item)
    {
        if (item == null)
        {
            return;
        }
        itemId = item.itemId;
        ItemCount = item.itemCount;
        itemImage.sprite = AtlasBank.Instance.GetSpriteByItemId(itemId);
    }

    public void OnUpdateItemCount(int count)
    {
        if (itemId == 0)
        {
            return;
        }
        ItemCount += count;
    }

    public void ReduceRemoveToSlot(int count)
    {
        if (itemId == 0)
        {
            return;
        }
        ItemCount -= count;
    }

    public void ResetSlot()
    {
        ItemCount = 0;
    }

    public int GetItemId()
    {
        return itemId;
    }

    public int GetItemCount()
    {
        return ItemCount;
    }

    public bool DoesSlotHasSpace(int itemId)
    {
        if (this.itemId == itemId)
        {
            if (ItemCount >= SaveLoadManager.saveData.backpackData.maxSlotSize)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }
}