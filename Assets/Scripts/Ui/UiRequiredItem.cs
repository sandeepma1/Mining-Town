using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MiningTown.IO;

public class UiRequiredItem : MonoBehaviour
{
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TextMeshProUGUI itemCountText;

    //Generally for items
    public void InitReceipeItems(int itemId, int requiredCount)
    {
        if (itemId == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }
        itemIconImage.sprite = AtlasBank.Instance.GetSpriteByName(ItemDatabase.GetItemSlugById(itemId), AtlasType.UiItems);
        int hasItemCount = SaveLoadManager.DoesItemExistsReturnCount(itemId);
        itemCountText.text = hasItemCount + "/" + requiredCount;
        if (hasItemCount < requiredCount)
        {
            itemCountText.color = ColorConstants.ThirdUiColor;
        }
        else
        {
            itemCountText.color = ColorConstants.SecondaryUiColor;
        }
    }

    //For crops
    public void InitCoinItems(int requiredCount)
    {
        if (requiredCount < 0)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }
        itemIconImage.sprite = AtlasBank.Instance.GetSpriteByName("coin", AtlasType.UiItems);
        if (requiredCount <= 0)
        {
            itemCountText.text = "Free";
        }
        else
        {
            itemCountText.text = requiredCount.ToString();
        }
    }

    public void InitRequiredCount(int itemId, int count)
    {
        itemIconImage.sprite = AtlasBank.Instance.GetSpriteByName(ItemDatabase.GetItemSlugById(itemId), AtlasType.UiItems);
        itemCountText.text = count.ToString();
    }
}