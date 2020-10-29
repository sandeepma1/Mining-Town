using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory m_instance = null;
    [SerializeField] private InputField inputField;
    [SerializeField] private Button addButton;
    [SerializeField] private Button removeButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button craftButton;
    [SerializeField] private Button damageWeaponButton;

    [SerializeField] private Transform inventorySlotPanel;
    [SerializeField] private Transform armourSlotPanel;
    [SerializeField] private Transform chestSlotPanel;
    [SerializeField] private InventorySlot inventorySlotPrefab;
    [SerializeField] private ArmourSlot armorSlotPrefab;
    [SerializeField] private ChestSlot chestSlotPrefab;
    [SerializeField] private GameObject inventoryItem;
    [SerializeField] private int chestSlotAmount = 6;
    public MyItem selectedItem = null;

    public int inventorySlotAmount = 10;
    public int armourSlotAmount = 4;
    public Image slotSelectedImage;
    public List<MyItem> l_items = new List<MyItem>();
    public List<InventorySlot> slotsGO = new List<InventorySlot>();
    public int selectedSlotID = -1;
    public int maxStackAmount = 10;

    private int inputFeildItemId = -1;
    private InventoryItem[] myInventory;

    private void Awake()
    {
        m_instance = this;
    }

    private void Start()
    {
        InitializeUiButtons();
        CreateInventorySlots();
        CreateArmorSlots();
        CreateChestSlots();
        myInventory = new InventoryItem[slotsGO.Count];
        AddSampleItems();
        Crafting.m_instance.InspectCraftableItems();
    }

    private void OnDestroy()
    {
        addButton.onClick.RemoveListener(AddItemButton);
        removeButton.onClick.RemoveListener(RemoveItemButton);
        deleteButton.onClick.RemoveListener(DeleteSelectedItem);
        damageWeaponButton.onClick.RemoveListener(DecreseWeaponDurability);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < slotsGO.Count; i++)
            {
                if (slotsGO[i].GetComponent<InventorySlot>())
                {
                    if (CheckIfSlotHasItem(i))
                    {
                        print("inv>>>>> " + slotsGO[i].GetComponent<InventorySlot>().id + " " + l_items[i].ID + " " +
                        slotsGO[i].GetComponent<InventorySlot>().transform.GetChild(0).GetComponent<InventoryItemData>().amount);
                    }
                    else
                    {
                        print("inv>>>>> " + slotsGO[i].GetComponent<InventorySlot>().id + " " + l_items[i].ID + " 0");
                    }
                }
                if (slotsGO[i].GetComponent<ArmourSlot>())
                {
                    if (CheckIfSlotHasItem(i))
                    {
                        print("arm  " + slotsGO[i].GetComponent<ArmourSlot>().id + " " + l_items[i].ID + " " +
                        slotsGO[i].GetComponent<ArmourSlot>().transform.GetChild(0).GetComponent<InventoryItemData>().amount);
                    }
                    else
                    {
                        print("arm  " + slotsGO[i].GetComponent<ArmourSlot>().id + " " + l_items[i].ID + " 0");
                    }
                }
                if (slotsGO[i].GetComponent<ChestSlot>())
                {
                    if (CheckIfSlotHasItem(i))
                    {
                        print("che0000000" + slotsGO[i].GetComponent<ChestSlot>().id + " " + l_items[i].ID + " " +
                        slotsGO[i].GetComponent<ChestSlot>().transform.GetChild(0).GetComponent<InventoryItemData>().amount);
                    }
                    else
                    {
                        print("che0000000 " + slotsGO[i].GetComponent<ChestSlot>().id + " " + l_items[i].ID + " 0");
                    }
                }
            }
        }
    }

    private void InitializeUiButtons()
    {
        inputField.onEndEdit.AddListener(OnInputFieldEditEnd);
        addButton.onClick.AddListener(AddItemButton);
        removeButton.onClick.AddListener(RemoveItemButton);
        deleteButton.onClick.AddListener(DeleteSelectedItem);
        damageWeaponButton.onClick.AddListener(DecreseWeaponDurability);
    }

    private void OnInputFieldEditEnd(string input)
    {
        int amount;
        if (int.TryParse(input, out amount))
        {
            inputFeildItemId = amount;
        }
        else
        {
            Debug.LogWarning("Entered text is not a number, please enter only numbers");
        }
    }

    private void AddButtonEventHandler()
    {
        throw new NotImplementedException();
    }

    private void CreateInventorySlots()
    {
        for (int i = 0; i < inventorySlotAmount; i++)
        {
            l_items.Add(new MyItem());
            slotsGO.Add(Instantiate(inventorySlotPrefab, inventorySlotPanel.transform));
            slotsGO[i].GetComponent<InventorySlot>().id = i;
            slotsGO[i].GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

    private void CreateArmorSlots()
    {
        for (int i = inventorySlotAmount; i < inventorySlotAmount + armourSlotAmount; i++)
        {
            l_items.Add(new MyItem());
            slotsGO.Add(Instantiate(inventorySlotPrefab, armourSlotPanel.transform));
            slotsGO[i].GetComponent<InventorySlot>().id = i;
            slotsGO[i].GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

    private void CreateChestSlots()
    {
        for (int i = inventorySlotAmount + armourSlotAmount; i < inventorySlotAmount + armourSlotAmount + chestSlotAmount; i++)
        {
            l_items.Add(new MyItem());
            slotsGO.Add(Instantiate(inventorySlotPrefab, chestSlotPanel.transform));
            slotsGO[i].GetComponent<InventorySlot>().id = i;
            slotsGO[i].GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

    private void AddSampleItems()
    {
        AddItem(3);
        AddItem(3);
        AddItem(3);
        AddItem(5);
        AddItem(2);
        AddItem(2);
        AddItem(2);
        AddItem(2);
        AddItem(2);
        AddItem(1);
        AddItem(1);
        AddItem(1);
        AddItem(9);
        AddItem(7);
    }

    public void AddItem(int id)
    {
        if (id < 0)
        {
            Debug.LogWarning("No item added. Input box (in bottom left corner of screen) is empty, give some item id.");
            return;
        }
        List<int> occurance = new List<int>();
        MyItem itemsToAdd = ItemsDatabase.FetchItemByID(id);
        if (itemsToAdd.Stackable)
        {
            for (int i = 0; i < l_items.Count - (armourSlotAmount + chestSlotAmount); i++)
            {
                if (l_items[i].ID == id)
                {
                    occurance.Add(i);
                }
            }
            if (occurance.Count > 0)
            {
                for (int i = 0; i < occurance.Count; i++)
                {
                    InventoryItemData data = slotsGO[occurance[i]].transform.GetChild(0).GetComponent<InventoryItemData>();
                    if (data.amount >= maxStackAmount)
                    {
                        if (i == occurance.Count - 1)
                        {
                            AddNewItemInUI(itemsToAdd);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        data.amount++;
                        data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
                        break;
                    }
                }
            }
            else
            {
                AddNewItemInUI(itemsToAdd);
            }
        }
        else
        {
            AddNewItemInUI(itemsToAdd);
        }
        SaveInventoryItems();
        Crafting.m_instance.InspectCraftableItems();
    }

    private void AddNewItemInUI(MyItem itemsToAdd)
    {
        if (CheckInventoryHasAtleastOneSpace())
        {
            for (int i = 0; i < l_items.Count - (armourSlotAmount + chestSlotAmount); i++)
            {
                if (l_items[i].ID == -1)
                {
                    l_items[i] = itemsToAdd;
                    GameObject itemsGO = Instantiate(inventoryItem, slotsGO[i].transform);
                    itemsGO.transform.SetAsFirstSibling();
                    itemsGO.GetComponent<RectTransform>().localScale = Vector3.one;
                    itemsGO.GetComponent<InventoryItemData>().slotID = i;
                    itemsGO.GetComponent<InventoryItemData>().type = itemsToAdd.Type;
                    if (itemsToAdd.Durability > 0)
                    {
                        itemsGO.GetComponent<InventoryItemData>().durability = itemsToAdd.Durability;
                        itemsGO.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(90, 10);
                    }
                    else
                    {
                        itemsGO.GetComponent<InventoryItemData>().durability = -1;
                        itemsGO.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(0, 10);
                    }
                    itemsGO.GetComponent<InventoryItemData>().item = itemsToAdd;
                    itemsGO.GetComponent<Image>().sprite = itemsToAdd.Sprite;
                    itemsGO.GetComponent<RectTransform>().anchoredPosition = Vector3.one;
                    break;
                }
            }
        }
    }

    public void RemoveItem(int id)
    {
        if (id < 0)
        {
            Debug.LogWarning("No item removed. Input box (in bottom left corner of screen) is empty, give some item id.");
            return;
        }
        MyItem itemsToRemove = ItemsDatabase.FetchItemByID(id);
        //print ("removed " + itemsToRemove.Name);
        if (itemsToRemove.Stackable)
        {
            for (int i = 0; i < l_items.Count - (armourSlotAmount + chestSlotAmount); i++)
            {
                if (l_items[i].ID == id)
                {
                    InventoryItemData data = slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>();
                    if (data.amount > 1)
                    {
                        data.amount--;
                        data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
                    }
                    else
                    {
                        DestroyItem(i);
                    }
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < l_items.Count; i++)
            {
                if (l_items[i].ID == id)
                {
                    slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().amount--;
                    DestroyItem(i);
                    break;
                }
            }
        }
        //SaveInventoryItems ();
        Crafting.m_instance.InspectCraftableItems();
    }

    public void DeleteSelectedItem()
    {
        if (selectedSlotID == -1)
        {
            Debug.LogWarning("No item selected! Please select some item in the inventory");
            return;
        }
        DestroyItem(selectedSlotID);
    }

    public void DestroyItem(int id)
    {
        for (int i = 0; i < slotsGO[id].transform.childCount; i++)
        {
            if (slotsGO[id].transform.GetChild(i).CompareTag("Item"))
            {
                DestroyImmediate(slotsGO[id].transform.GetChild(i).gameObject); // Be careful used DestroyImmediate, come here if there is any issue
                break;
            }
        }
        l_items[id] = new MyItem();
    }

    private bool CheckItemInInventory(MyItem item)
    {
        for (int i = 0; i < inventorySlotAmount; i++)
        {
            if (l_items[i].ID == item.ID)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckIfSlotHasItem(int slotID)
    {
        if (slotsGO[slotID].transform.childCount > 0 && slotsGO[slotID].transform.GetChild(0).CompareTag("Item"))
        {
            return true;
        }
        return false;
    }

    public int CheckItemAmountInInventory(int id) //do this by slots or items saved
    {
        int amount = 0;
        for (int i = 0; i < inventorySlotAmount; i++)
        {
            if (slotsGO[i].transform.childCount > 0 && slotsGO[i].transform.GetChild(0).CompareTag("Item") && slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().item.ID == id)
            {
                amount += slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().amount;
            }
        }
        return amount;
    }

    public bool CheckInventoryHasAtleastOneSpace()
    {
        for (int i = 0; i < inventorySlotAmount; i++)
        {
            if (slotsGO[i].transform.childCount <= 0)
            {
                return true;
            }
            else if (!slotsGO[i].transform.GetChild(0).CompareTag("Item"))
            {
                return true;
            }
        }
        print("unable to add inventory full");
        return false;
    }

    private void SaveInventoryItems()
    {
        for (int i = 0; i < slotsGO.Count; i++)
        {
            if (slotsGO[i].transform.childCount > 0 && slotsGO[i].transform.GetChild(0).CompareTag("Item"))
            {
                myInventory[i] = new InventoryItem(slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().item.ID,
                    slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().amount,
                    slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().slotID,
                    slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().item.Durability);
            }
            else
            {
                myInventory[i] = new InventoryItem();
            }
        }
    }

    private void LoadInventoryItems()
    {
        InventoryItem[] myInventory = new InventoryItem[slotsGO.Count];// = new InventoryItems ();
        for (int i = 0; i < slotsGO.Count; i++)
        {
            if (slotsGO[i].transform.childCount > 0 && slotsGO[i].transform.GetChild(0).CompareTag("Item"))
            {
                myInventory[i].ID = slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().item.ID;
                myInventory[i].Amount = slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().amount;
                myInventory[i].SlotID = slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().slotID;
                myInventory[i].Health = slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().item.Durability;
            }
            else
            {
                myInventory[i] = new InventoryItem();
            }
        }
    }

    private void DecreseWeaponDurability()
    {
        int amount = 1;
        if (selectedSlotID >= 0 && slotsGO[selectedSlotID].transform.childCount > 0 &&
            slotsGO[selectedSlotID].transform.GetChild(0).CompareTag("Item") &&
            slotsGO[selectedSlotID].transform.GetChild(0).GetComponent<InventoryItemData>().durability >= 0)
        {
            slotsGO[selectedSlotID].transform.GetChild(0).GetComponent<InventoryItemData>().DecreaseItemDurability(amount);
        }
    }

    private void AddItemButton()
    {
        AddItem(inputFeildItemId);
    }

    private void RemoveItemButton()
    {
        RemoveItem(inputFeildItemId);
    }
}

[System.Serializable]
public class InventoryItem
{
    public int ID { get; set; }
    public int Amount { get; set; }
    public int SlotID { get; set; }
    public int Health { get; set; }

    public InventoryItem(int id, int amount, int slotID, int health)
    {
        ID = id;
        Amount = amount;
        SlotID = slotID;
        Health = health;
    }

    public InventoryItem()
    {
        ID = -1;
        Amount = -1;
        SlotID = -1;
        Health = -1;
    }
}