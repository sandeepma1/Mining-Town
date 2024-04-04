using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour
{
    public static Crafting m_instance = null;

    [SerializeField] private Transform slotSelectedImage;
    [SerializeField] private Button craftButton;
    [SerializeField] private Transform craftPanelParent;
    [SerializeField] private CraftingSlot craftSlotPrefab;
    [SerializeField] private int[] craftingItems;

    private int selectedItemID = -1;
    private List<CraftingSlot> craftingSlotsGO = new List<CraftingSlot>();

    private void Awake()
    {
        m_instance = this;
    }

    private void Start()
    {
        craftButton.onClick.AddListener(CraftSelectedItem);
        CreateCraftingSlots();
    }

    private void OnDestroy()
    {
        craftButton.onClick.RemoveListener(CraftSelectedItem);
    }

    public void InspectCraftableItems()
    {
        if (craftingSlotsGO.Count == 0)
        {
            return;
        }
        for (int i = 0; i < craftingItems.Length; i++)
        {
            if (CheckRequiredItemsInInventory(craftingItems[i]))
            {
                craftingSlotsGO[i].slotIcon.color = new Color(1, 1, 1, 1);
            }
            else
            {
                craftingSlotsGO[i].slotIcon.color = new Color(0, 0, 0, 1);
            }
        }
    }

    private void CreateCraftingSlots()
    {
        for (int i = 0; i < craftingItems.Length; i++)
        {
            craftingSlotsGO.Add(Instantiate(craftSlotPrefab, craftPanelParent));
            craftingSlotsGO[i].OnCraftingSlotSelected += OnCraftingSlotSelectedEventHandler;
            craftingSlotsGO[i].InitializeSlot(craftingItems[i], i);
        }
    }

    private void OnCraftingSlotSelectedEventHandler(int itemId, int slotId)
    {
        selectedItemID = itemId;
        slotSelectedImage.SetParent(craftingSlotsGO[slotId].transform);
        slotSelectedImage.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        slotSelectedImage.SetAsLastSibling();
    }

    private void CraftSelectedItem()
    {
        if (selectedItemID >= 0)
        {
            if (CheckRequiredItemsInInventory(selectedItemID) && Inventory.m_instance.CheckInventoryHasAtleastOneSpace())
            { //if inventory has all the craftable items
                RemoveItemsToCreateNewItem();
                Inventory.m_instance.AddItem(selectedItemID);
                print("crafted item" + selectedItemID);
            }
            else
            {
                print("missing Items or inventory full");
            }
        }
    }

    private void RemoveItemsToCreateNewItem()
    {
        MyItem itemToCraft = ItemsDatabase.FetchItemByID(selectedItemID);

        if (itemToCraft.ItemID1 >= 0)
        {
            for (int i = 0; i < itemToCraft.ItemAmount1; i++)
            {
                Inventory.m_instance.RemoveItem(itemToCraft.ItemID1);
                print("removed " + itemToCraft.ItemID1);
            }
        }
        if (itemToCraft.ItemID2 >= 0)
        {
            for (int i = 0; i < itemToCraft.ItemAmount2; i++)
            {
                Inventory.m_instance.RemoveItem(itemToCraft.ItemID2);
                print("removed " + itemToCraft.ItemID2);
            }
        }
        if (itemToCraft.ItemID3 >= 0)
        {
            for (int i = 0; i < itemToCraft.ItemAmount3; i++)
            {
                Inventory.m_instance.RemoveItem(itemToCraft.ItemID3);
            }
        }
        if (itemToCraft.ItemID4 >= 0)
        {
            for (int i = 0; i < itemToCraft.ItemAmount4; i++)
            {
                Inventory.m_instance.RemoveItem(itemToCraft.ItemID4);
            }
        }
    }

    private bool CheckRequiredItemsInInventory(int id)
    {
        MyItem itemToCraft = ItemsDatabase.FetchItemByID(id);
        bool item1 = false;
        bool item2 = false;
        bool item3 = false;
        bool item4 = false;

        if (itemToCraft.ItemID1 >= -1 && Inventory.m_instance.CheckItemAmountInInventory(itemToCraft.ItemID1) >= itemToCraft.ItemAmount1)
        {
            item1 = true;
        }
        if (itemToCraft.ItemID2 >= -1 && Inventory.m_instance.CheckItemAmountInInventory(itemToCraft.ItemID2) >= itemToCraft.ItemAmount2)
        {
            item2 = true;
        }
        if (itemToCraft.ItemID3 >= -1 && Inventory.m_instance.CheckItemAmountInInventory(itemToCraft.ItemID3) >= itemToCraft.ItemAmount3)
        {
            item3 = true;
        }
        if (itemToCraft.ItemID4 >= -1 && Inventory.m_instance.CheckItemAmountInInventory(itemToCraft.ItemID4) >= itemToCraft.ItemAmount4)
        {
            item4 = true;
        }

        if (item1 && item2 && item3 && item4)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}