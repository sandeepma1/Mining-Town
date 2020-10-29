using System;
using System.Collections;
using System.Collections.Generic;
using MiningTown.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiEdibleItem : MonoBehaviour
{
    public Action<ItemIdWithCount> OnUiEdibleItemButtonClick;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemCountText;
    private ItemIdWithCount farmItemsData;
    private Button button;

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
        OnUiEdibleItemButtonClick?.Invoke(farmItemsData);
    }
}
