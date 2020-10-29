using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MiningTown.IO;
using System;

[System.Serializable]
public class UiBarnItem : MonoBehaviour
{
    public Action<ItemIdWithCount> OnBarnItemButtonClick;
    [SerializeField] private Button button;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemCountText;
    private ItemIdWithCount farmItemsData;

    public int ItemCount
    {
        get
        {
            return farmItemsData.itemCount;
        }
        set
        {
            farmItemsData.itemCount = value;
            itemCountText.text = farmItemsData.itemCount.ToString();
        }
    }

    private void Start()
    {
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClicked);
    }

    public void UpdateItemCount(int count)
    {
        ItemCount = count;
    }

    public void Init(ItemIdWithCount farmItemsData)
    {
        this.farmItemsData = farmItemsData;
        string slug = ItemDatabase.GetItemSlugById(this.farmItemsData.itemId);
        itemImage.sprite = AtlasBank.Instance.GetSpriteByName(slug, AtlasType.UiItems);
        ItemCount = this.farmItemsData.itemCount;
    }

    private void OnButtonClicked()
    {
        //UiItemClickPopup.OnShowSellItemMenu?.Invoke(farmItemsData);
        OnBarnItemButtonClick?.Invoke(farmItemsData);
    }
}