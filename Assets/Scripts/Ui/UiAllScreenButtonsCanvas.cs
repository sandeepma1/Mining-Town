using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiAllScreenButtonsCanvas : MonoBehaviour
{
    public static Action<bool> OnToggleAllButtons;
    public static Action OnSettingButtonClick;
    public static Action OnBuildButtonClick;
    public static Action OnInventoryButtonClick;
    public static Action OnEditModeButtonClick;

    [SerializeField] private RectTransform leftPanel;
    [SerializeField] private RectTransform rightPanel;

    [SerializeField] private Button settingsButton;
    [SerializeField] private Button buildButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button editModeButton;

    private const float panelWidth = 200;
    private const float animDuration = 0.25f;

    private void Start()
    {
        OnToggleAllButtons += ToggleAllButtons;

        settingsButton.onClick.AddListener(SettingButtonClick);
        buildButton.onClick.AddListener(BuildButtonClick);
        inventoryButton.onClick.AddListener(InventoryButtonClick);
        editModeButton.onClick.AddListener(EditModeButtonClick);
    }

    private void OnDestroy()
    {
        OnToggleAllButtons += ToggleAllButtons;

        settingsButton.onClick.RemoveListener(SettingButtonClick);
        buildButton.onClick.RemoveListener(BuildButtonClick);
        inventoryButton.onClick.RemoveListener(InventoryButtonClick);
        editModeButton.onClick.RemoveListener(EditModeButtonClick);
    }


    #region All Buttons Events
    private void SettingButtonClick()
    {
        HideAllButtons();
        OnSettingButtonClick?.Invoke();
    }
    private void BuildButtonClick()
    {
        HideAllButtons();
        OnBuildButtonClick?.Invoke();
    }
    private void InventoryButtonClick()
    {
        HideAllButtons();
        OnInventoryButtonClick?.Invoke();
    }
    private void EditModeButtonClick()
    {
        HideAllButtons();
        OnEditModeButtonClick?.Invoke();
    }
    #endregion

    private void HideAllButtons()
    {
        ToggleAllButtons(false);
    }

    private void ToggleAllButtons(bool isVisible)
    {
        float pos = isVisible ? 0 : panelWidth;

        leftPanel.DOAnchorPosX(pos * -1, animDuration);
        rightPanel.DOAnchorPosX(pos, animDuration);
        Joystick.OnToggleJoystick?.Invoke(isVisible);
    }
}