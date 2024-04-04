using System;
using UnityEngine;
using UnityEngine.UI;


public class UiPopupMenu : Singleton<UiPopupMenu>
{
    [SerializeField] private GameObject backgroundPanel;
    //[SerializeField] private UiSettingsPopup settingsCanvas;

    private Button backgroundButton;

    private void Start()
    {
        backgroundButton = backgroundPanel.GetComponent<Button>();
        backgroundButton.onClick.AddListener(OnCloseButtonClicked);
        ClosePopup();
    }

    private void OnDestroy()
    {
        backgroundButton.onClick.RemoveListener(OnCloseButtonClicked);
    }

    private void OnSettingsButtonClicked()
    {
        ShowPopup();
    }

    private void OnCloseButtonClicked()
    {
        ClosePopup();
    }

    public void ShowPopup()
    {
        backgroundPanel.SetActive(true);
        //settingsCanvas.OpenPopup();
    }

    public void ClosePopup()
    {
        backgroundPanel.SetActive(false);
        //settingsCanvas.ClosePopup();
    }
}
