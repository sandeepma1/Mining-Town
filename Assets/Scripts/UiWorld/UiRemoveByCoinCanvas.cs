using System;
using MiningTown.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiRemoveByCoinCanvas : MonoBehaviour
{
    public static Action<int, Vector3, Action> OnShowCanvas;
    public static Action OnHideCanvas;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private TextMeshProUGUI coinCountText;
    [SerializeField] private Button coinButton;
    private RectTransform rectTransform;
    private Camera mainCamera;
    private bool isCanvasVisible;
    private int coinNeededToCompelete;
    private Action CompleteByCoin;

    private void Start()
    {
        OnHideCanvas += HideCanvas;
        OnShowCanvas += ShowCanvas;
        mainCamera = Camera.main;
        ToggleCanvas(false);
        rectTransform = mainPanel.GetComponent<RectTransform>();
        coinButton.onClick.AddListener(OnCoinButtonClicked);
    }

    private void OnDestroy()
    {
        OnHideCanvas -= HideCanvas;
        OnShowCanvas -= ShowCanvas;
        coinButton.onClick.RemoveListener(OnCoinButtonClicked);
    }

    private void ShowCanvas(int itemId, Vector3 pos, Action CompleteByCoin)
    {
        this.CompleteByCoin = CompleteByCoin;
        coinNeededToCompelete = ItemDatabase.GetRemoveByCoinByItemId(itemId);
        coinCountText.text = GameVariables.tmp_coinIcon + coinNeededToCompelete;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(pos);
        rectTransform.position = screenPos;
        ToggleCanvas(true);
    }

    private void HideCanvas()
    {
        if (isCanvasVisible)
        {
            ToggleCanvas(false);
        }
    }

    private void OnCoinButtonClicked()
    {
        ToggleCanvas(false);
        bool hasCoins = PlayerCurrencyManager.ReduceCoins(coinNeededToCompelete);
        if (hasCoins)
        {
            CompleteByCoin?.Invoke();
        }
        else
        {
            //Show some common ui to purchase gems
        }
    }

    protected void ToggleCanvas(bool isVisible)
    {
        isCanvasVisible = isVisible;
        mainPanel.SetActive(isVisible);
    }
}