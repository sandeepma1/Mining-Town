using System;
using UnityEngine;
using UnityEngine.UI;
using MiningTown.IO;
using System.Collections.Generic;
using TMPro;

public class UiCropsCanvas : UiBasicCanvasWindowBase
{
    public static Action<int> OnCropDroppedOnRaisedBed;//
    public static Action<int> OnShowCanvas;
    public static Action<float> OnScanNearbyBeds;
    public static Action OnHideCanvas;
    public static Action OnCropMenuClosed;
    [Header("Child Class")]
    [Space(10)]
    [SerializeField] private Image cursorImage;
    [SerializeField] private Transform contentParent;
    [SerializeField] private UiDragItem uiDraggableItemPrefab;
    [SerializeField] private ScrollRect parentScrollRect;
    [SerializeField] private UiDropItem uiDropItem;
    [SerializeField] private TextMeshProUGUI headerText;
    private List<Crops> allCropsInfo = new List<Crops>();
    private List<UiDragItem> uiDragItems = new List<UiDragItem>();
    [SerializeField] private UiToggleItem[] uiToggleItems;
    public int currentDraggedItemId = -1;
    private bool isCanvasVisible;
    private RectTransform uiDropItemRect;
    private int lastUiToggleItemIndex = 0;
    public RaisedBedDropRate raisedBedDropRate;

    private void Start()
    {
        uiDropItemRect = uiDropItem.GetComponent<RectTransform>();
        OnShowCanvas += ShowCanvas;
        OnHideCanvas += HideCanvas;
        uiDropItem.OnDropItem += UiDropItemHandler;
        allCropsInfo = CropsDatabase.GetAllCrops();
        InitUiToggleItem();
        PopulateItems();
    }

    private void OnDestroy()
    {
        OnShowCanvas -= ShowCanvas;
        OnHideCanvas -= HideCanvas;
        uiDropItem.OnDropItem -= UiDropItemHandler;
    }

    private void PopulateItems()
    {
        for (int i = 0; i < allCropsInfo.Count; i++)
        {
            UiDragItem uiDraggerItem = Instantiate(uiDraggableItemPrefab, contentParent);
            string slug = ItemDatabase.GetItemSlugById(allCropsInfo[i].outputItemId);

            uiDraggerItem.Init(DescriptionType.Crop, allCropsInfo[i].outputItemId,
                AtlasBank.Instance.GetSpriteByName(slug, AtlasType.UiItems), parentScrollRect);
            uiDraggerItem.OnDragStart += OnDragStart;
            uiDraggerItem.OnDragItemPosition += OnDragItemPosition;
            uiDraggerItem.OnDragEnd += OnDragEnd;
            uiDraggerItem.OnClickItem += OnClickItem;
            uiDragItems.Add(uiDraggerItem);
        }
    }

    private void InitUiToggleItem()
    {
        for (int i = 0; i < uiToggleItems.Length; i++)
        {
            uiToggleItems[i].Init(i);
            uiToggleItems[i].OnToggleValueChanged += OnToggleValueChanged;
        }
    }

    private void OnToggleValueChanged(int index)
    {
        if (lastUiToggleItemIndex == index)
        {
            return;
        }
        uiToggleItems[lastUiToggleItemIndex].DeselectButton();
        raisedBedDropRate = (RaisedBedDropRate)index;
        SetScanSize();
        lastUiToggleItemIndex = index;
    }

    private void SetScanSize()
    {
        switch (raisedBedDropRate)
        {
            case RaisedBedDropRate.Single:
                OnScanNearbyBeds?.Invoke(0);
                break;
            case RaisedBedDropRate.TwoByTwo:
                OnScanNearbyBeds?.Invoke(4f);
                break;
            case RaisedBedDropRate.ThreeByThree:
                OnScanNearbyBeds?.Invoke(7.75f);
                break;
            default:
                break;
        }
    }

    private void ShowCanvas(int bedIndex)
    {
        SetScanSize();
        int bedNumber = bedIndex + 1;
        headerText.text = "Raised Bed No. " + bedNumber;
        if (!isCanvasVisible)
        {
            ToggleCanvas(true);
        }
    }

    private void HideCanvas()
    {
        if (isCanvasVisible)
        {
            ToggleCanvas(false);
        }
    }

    protected override void ToggleCanvas(bool isVisible)
    {
        base.ToggleCanvas(isVisible);
        if (isVisible)
        {
            for (int i = 0; i < uiDragItems.Count; i++)
            {
                uiDragItems[i].UpdateCountText();
            }
        }
        //else
        //{
        //    SetScanSize();
        //}
        isCanvasVisible = isVisible;
        if (!isCanvasVisible)
        {
            OnCropMenuClosed?.Invoke();
        }
    }


    #region Drag Drop Stuff
    private void OnDragStart(int itemId, Sprite sprite)
    {
        uiDropItemRect.SetAsLastSibling();
        currentDraggedItemId = itemId;
        cursorImage.sprite = sprite;
    }

    private void OnDragItemPosition(Vector2 dragItePosition)
    {
        cursorImage.enabled = true;
        cursorImage.transform.position = dragItePosition;
    }

    private void OnDragEnd()
    {
        uiDropItemRect.SetAsFirstSibling();
        cursorImage.enabled = false;
        currentDraggedItemId = -1;
    }

    private void UiDropItemHandler()
    {
        if (currentDraggedItemId >= 0)
        {
            OnCropDroppedOnRaisedBed?.Invoke(currentDraggedItemId);
            HideCanvas();
        }
    }

    private void OnClickItem(int itemId)
    {
        if (itemId >= 0)
        {
            OnCropDroppedOnRaisedBed?.Invoke(itemId);
            HideCanvas();
        }
    }
    #endregion
}