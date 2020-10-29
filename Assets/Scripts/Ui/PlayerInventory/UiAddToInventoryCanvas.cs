using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiningTown.IO;
using UnityEngine.UI;
using System;

public class UiAddToInventoryCanvas : MonoBehaviour
{
    public static Action<bool> OnToggleCanvas;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private Button okButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Transform topContentParent;
    [SerializeField] private Transform bottomContentParent;
    [SerializeField] private UiBarnItem uiBarnItemPrefab;
    private List<UiBarnItem> uiBarnItems = new List<UiBarnItem>();
    private int lastClickedTabId;

    private void Start()
    {
        OnToggleCanvas += ToggleCanvas;
        okButton.onClick.AddListener(OnOkButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);
    }

    private void OnDestroy()
    {
        OnToggleCanvas -= ToggleCanvas;
        okButton.onClick.RemoveListener(OnOkButtonClicked);
        cancelButton.onClick.RemoveListener(OnCancelButtonClicked);
    }

    private void PopulateEdibleItems()
    {
        //First destroy all items
        foreach (var item in uiBarnItems)
        {
            Destroy(item.gameObject);
        }
        uiBarnItems = new List<UiBarnItem>();

        //Create new items
        for (int i = 0; i < SaveLoadManager.saveData.barnItems.Count; i++)
        {
            int itemId = SaveLoadManager.saveData.barnItems[i].itemId;
            int energy = ItemDatabase.GetEnergyRestoreItemId(itemId);
            int health = ItemDatabase.GetHealthRestoreItemId(itemId);

            if (energy > 0 || health > 0)
            {
                UiBarnItem uiBarnItem = Instantiate(uiBarnItemPrefab, topContentParent);
                uiBarnItem.Init(SaveLoadManager.saveData.barnItems[i]);
                uiBarnItems.Add(uiBarnItem);
            }
        }
    }

    private void OnOkButtonClicked()
    {
        ToggleCanvas(false);
    }

    private void OnCancelButtonClicked()
    {
        ToggleCanvas(false);
    }

    private void ToggleCanvas(bool isVisible)
    {
        mainPanel.SetActive(isVisible);
        if (isVisible)
        {
            PopulateEdibleItems();
        }
    }
}
