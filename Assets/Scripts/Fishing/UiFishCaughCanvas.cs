using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MiningTown.IO;

public class UiFishCaughCanvas : MonoBehaviour
{
    public static Action<FishObject> OnShowFishCaughtCanvas;
    [Header("Child Class")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button okButton;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private TextMeshProUGUI fishNameText;
    [SerializeField] private TextMeshProUGUI lengthText;
    [SerializeField] private Image fishIcon;

    private void Start()
    {
        closeButton.onClick.AddListener(OnCloseCanvas);
        okButton.onClick.AddListener(OnCloseCanvas);
        OnShowFishCaughtCanvas += ShowFishCaughtCanvas;
        ToggleViewCanvas(false);
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(OnCloseCanvas);
        okButton.onClick.RemoveListener(OnCloseCanvas);
        OnShowFishCaughtCanvas -= ShowFishCaughtCanvas;
    }

    public void ShowFishCaughtCanvas(FishObject fishObject) //init function
    {
        if (fishObject == null)
        {
            print("fishObject is null, check this");
            return;
        }

        Item fishItem = ItemDatabase.GetItemById(fishObject.fishId);
        DroppedItemManager.OnDropResource(fishItem, PlayerMovement.Instance.GetPlayerTransform(), 1);

        //UiResourceSpawnCanvas.OnHarvestResource?.Invoke(fishItem.slug, PlayerMovement.Instance.GetPlayerTransform(), 1);
        //UiResourceSpawnCanvas.OnHarvestResource?.Invoke("xp", PlayerMovement.Instance.GetPlayerTransform(), fishItem.xpOnHarvest);

        fishNameText.text = fishObject.name;
        if (fishObject.strugglePower > 0)
        {
            lengthText.text = "Lenght: " + UnityEngine.Random.Range(fishObject.maxLengthInCm, fishObject.maxLengthInCm) + "cm";
        }
        else
        {
            lengthText.text = "";
        }
        fishIcon.sprite = AtlasBank.Instance.GetSpriteByName(fishObject.slug, AtlasType.UiItems);
        ToggleViewCanvas(true);
    }

    private void OnCloseCanvas()
    {
        ToggleViewCanvas(false);
    }

    private void ToggleViewCanvas(bool isVisible)
    {
        mainPanel.SetActive(isVisible);
    }
}