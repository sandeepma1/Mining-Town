using System;
using UnityEngine;
using UnityEngine.UI;

public class UiBuildingEditModeCanvas : MonoBehaviour
{
    public static Action OnEditOkButtonClick;
    public static Action<bool> OnEditCancelButtonClick; //If true destroy building as it was canceled in build menu
    public static Action OnBuildOkButtonClick;
    public static Action OnBuildCancelButtonClick;
    public static Action<bool> OnToggleButtonsPanel;
    public static Action<bool> OnToggleOkButton;
    public static Action<bool> OnToggleEditMode;
    [Header("Edit Buttons stuff")]
    [Space(10)]
    [SerializeField] private GameObject mainEditButtonsPanel;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button okButton;
    [SerializeField] private Button closeEditModeButton;
    private bool isInBuildMode;

    private void Start()
    {
        UiBuildCanvas.OnIsInBuildMode += OnIsInBuildMode;
        UiAllScreenButtonsCanvas.OnEditModeButtonClick += OnEditModeButtonClick;
        OnToggleButtonsPanel += ToggleButtonsPanel;
        OnToggleOkButton += ToggleOkButtonInteractable;
        okButton.onClick.AddListener(OnOkButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);
        closeEditModeButton.onClick.AddListener(OnCloseEditMode);
        ToggleButtonsPanel(false);
    }

    private void OnDestroy()
    {
        UiBuildCanvas.OnIsInBuildMode -= OnIsInBuildMode;
        UiAllScreenButtonsCanvas.OnEditModeButtonClick -= OnEditModeButtonClick;
        OnToggleButtonsPanel -= ToggleButtonsPanel;
        OnToggleOkButton -= ToggleOkButtonInteractable;
        okButton.onClick.RemoveListener(OnOkButtonClicked);
        cancelButton.onClick.RemoveListener(OnCancelButtonClicked);
        closeEditModeButton.onClick.RemoveListener(OnCloseEditMode);
    }

    private void OnIsInBuildMode(bool inEditMode)
    {
        isInBuildMode = inEditMode;
        closeEditModeButton.interactable = !inEditMode;
    }

    private void OnEditModeButtonClick()
    {
        EditModeColliders.OnToggleEditModeColliders?.Invoke(true);
        closeEditModeButton.gameObject.SetActive(true);
        OnToggleEditMode?.Invoke(true);
    }


    #region Edit buttons stuff
    private void OnOkButtonClicked()
    {
        OnEditOkButtonClick?.Invoke();
        if (isInBuildMode)
        {
            OnBuildOkButtonClick?.Invoke();
            OnCloseEditMode();
        }
    }

    private void OnCancelButtonClicked()
    {
        OnEditCancelButtonClick?.Invoke(isInBuildMode);
        if (isInBuildMode)
        {
            OnBuildCancelButtonClick?.Invoke();
            OnCloseEditMode();
        }
    }

    private void OnCloseEditMode()
    {
        if (!isInBuildMode)
        {
            OnCancelButtonClicked();
        }
        ToggleButtonsPanel(false);
        UiAllScreenButtonsCanvas.OnToggleAllButtons?.Invoke(true);
        EditModeColliders.OnToggleEditModeColliders?.Invoke(false);
        closeEditModeButton.gameObject.SetActive(false);
        OnToggleEditMode?.Invoke(false);
    }

    private void ToggleButtonsPanel(bool isVisible)
    {
        mainEditButtonsPanel.SetActive(isVisible);
    }

    private void ToggleOkButtonInteractable(bool isEnabled)
    {
        okButton.interactable = isEnabled;
    }
    #endregion
}