using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

public class ItemsDatabase : MonoBehaviour
{
    public static ItemsDatabase m_instance = null;
    [HideInInspector]
    public static List<MyItem> database = new List<MyItem>();

    private string fileName = "Items";

    private void Awake()
    {
        m_instance = this;
    }

    private void Start()
    {
        ConstructItemDatabase();
    }

    private void ConstructItemDatabase()
    {
        string[] lines = new string[100];
        string[] chars = new string[100];
        TextAsset itemCSV = Resources.Load("CSV/" + fileName) as TextAsset;
        lines = Regex.Split(itemCSV.text, Environment.NewLine);
        for (int i = 1; i < lines.Length - 1; i++)
        {
            chars = Regex.Split(lines[i], ",");

            database.Add(new MyItem(IntParse(chars[0]), (TypeOfItem)System.Enum.Parse(typeof(TypeOfItem), chars[1]),
                chars[2], IntParse(chars[3]), IntParse(chars[4]), IntParse(chars[5]), IntParse(chars[6]),
                chars[7], bool.Parse(chars[8]), IntParse(chars[9]), (chars[10]), bool.Parse(chars[11]),
                IntParse(chars[12]), IntParse(chars[13]), IntParse(chars[14]), IntParse(chars[15]), IntParse(chars[16]),
                IntParse(chars[17]), IntParse(chars[18]), IntParse(chars[19])));
        }
    }

    public static MyItem FetchItemByID(int id)
    {
        for (int i = 0; i < database.Count; i++)
        {
            if (database[i].ID == id)
            {
                return database[i];
            }
        }
        return null;
    }

    public static Sprite GetSpriteByItemId(int id)
    {
        return database[id].Sprite;
    }

    private int IntParse(string text)
    {
        int num;
        if (int.TryParse(text, out num))
        {
            return num;
        }
        else
            return 0;
    }

    private float FloatParse(string text)
    {
        float result = 0.01f;
        float.TryParse(text, out result);
        return result;
    }
}

[System.Serializable]
public class MyItem
{
    public int ID { get; set; }
    public TypeOfItem Type { get; set; }
    public string Name { get; set; }
    public int Durability { get; set; }
    public int Power { get; set; }
    public int Defence { get; set; }
    public int Vitality { get; set; }
    public string Description { get; set; }
    public bool Stackable { get; set; }
    public int Rarity { get; set; }
    public string Slug { get; set; }
    public bool IsPlaceable { get; set; }
    public int ItemID1 { get; set; }
    public int ItemAmount1 { get; set; }
    public int ItemID2 { get; set; }
    public int ItemAmount2 { get; set; }
    public int ItemID3 { get; set; }
    public int ItemAmount3 { get; set; }
    public int ItemID4 { get; set; }
    public int ItemAmount4 { get; set; }
    public Sprite Sprite { get; set; }

    public MyItem(int id, TypeOfItem type, string name, int durability, int power, int defence, int vitality, string description,
                   bool stackable, int rarity, string slug, bool isplaceable, int itemID1, int itemAmount1, int itemID2, int itemAmount2,
                   int itemID3, int itemAmount3, int itemID4, int itemAmount4)
    {
        this.ID = id;
        this.Type = type;
        this.Name = name;
        this.Durability = durability;
        this.Power = power;
        this.Defence = defence;
        this.Vitality = vitality;
        this.Description = description;
        this.Stackable = stackable;
        this.Rarity = rarity; //10
        this.Slug = slug;
        this.IsPlaceable = isplaceable;
        this.ItemID1 = itemID1;
        this.ItemAmount1 = itemAmount1;
        this.ItemID2 = itemID2;//15
        this.ItemAmount2 = itemAmount2;
        this.ItemID3 = itemID3;
        this.ItemAmount3 = itemAmount3;
        this.ItemID4 = itemID4;
        this.ItemAmount4 = itemAmount4;//20
        this.Sprite = Resources.Load<Sprite>("Textures/Inventory/" + slug);
    }

    public MyItem()
    {
        this.ID = -1;
    }
}

public enum TypeOfItem
{
    item,
    weapon,
    armor,
    building,
    consumable
}
