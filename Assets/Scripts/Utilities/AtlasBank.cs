using UnityEngine.U2D;
using UnityEngine;
using MiningTown.IO;

public class AtlasBank : Singleton<AtlasBank>
{
    [SerializeField] private SpriteAtlas farmHome;
    [SerializeField] private SpriteAtlas buildUiAtlas;
    private Sprite missingSprite;

    private void Start()
    {
        missingSprite = GetSpriteByName("Blockx100", AtlasType.UiItems);
    }

    public Sprite GetSprite(int itemId, AtlasType type)
    {
        return GetSpriteByName(ItemDatabase.GetItemSlugById(itemId), type);
    }

    public Sprite GetSpriteByItemId(int itemId)
    {
        return GetSpriteByName(ItemDatabase.GetItemSlugById(itemId), AtlasType.UiItems);
    }

    public Sprite GetSpriteByName(string name, AtlasType type)
    {
        Sprite sprite = null;

        switch (type)
        {
            case AtlasType.UiItems:
                sprite = farmHome.GetSprite(name);
                break;
            case AtlasType.UiBuildMenu:
                sprite = buildUiAtlas.GetSprite(name);
                break;
        }
        if (sprite == null)
        {
            Debug.Log("Sprite named " + name + " not found in Bank Type " + type);
            sprite = missingSprite;
        }
        return sprite;
    }
}

public enum AtlasType
{
    UiItems,
    UiBuildMenu
}