using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//[RequireComponent(typeof(CanvasGroup))]
public class UiCommonPopupMenu : Singleton<UiCommonPopupMenu>
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private UiSimpleButton uiSimpleButtonPrefab;
    [SerializeField] private RectTransform dialogPanel;
    [SerializeField] private Transform buttonsPanel;
    private const float fadeAnimDuration = 0.10f;
    private const float moveAnimDuration = 0.15f;
    private List<UiSimpleButton> uiSimpleButtons = new List<UiSimpleButton>();
    private bool isDialogShowing = false;
    private float topPosition;
    private float middlePosition;
    private float bottomPosition;
    private float hidePosition;

    protected override void Awake()
    {
        base.Awake();
        float screenHeightParts = Screen.height / 4;
        topPosition = screenHeightParts * 3;
        middlePosition = screenHeightParts * 2;
        bottomPosition = screenHeightParts;

        hidePosition = dialogPanel.sizeDelta.y / 2 * -1;
        dialogPanel.anchoredPosition = new Vector2(0, hidePosition);
        for (int i = 0; i < 3; i++)
        {
            UiSimpleButton uiSimpleButton = Instantiate(uiSimpleButtonPrefab, buttonsPanel);
            uiSimpleButtons.Add(uiSimpleButton);
        }
    }

    #region Actual Dialogs Functions
    public void InitOkDialog(string message, UnityAction okAction,
        string okText = "Ok",
        CommonPopupScreenPosition commonPopupScreenPosition = CommonPopupScreenPosition.Middle)
    {
        if (isDialogShowing)
        {
            Debug.LogError("Popup dialog is already showing");
            return;
        }
        HideAllButtons();
        descriptionText.text = message;
        InitButton(uiSimpleButtons[0], okAction, okText);
        ShowDialogPanel(commonPopupScreenPosition);
    }

    public void InitYesNoDialog(string message, UnityAction yesAction, UnityAction noAction,
        string yesText = "Yes", string noText = "No",
        CommonPopupScreenPosition commonPopupScreenPosition = CommonPopupScreenPosition.Middle)
    {
        if (isDialogShowing)
        {
            Debug.LogError("Popup dialog is already showing");
            return;
        }
        HideAllButtons();
        descriptionText.text = message;
        InitButton(uiSimpleButtons[0], yesAction, yesText);
        InitButton(uiSimpleButtons[1], noAction, noText);
        ShowDialogPanel(commonPopupScreenPosition);
    }

    public void InitYesNoCancelDialog(string message, UnityAction yesAction, UnityAction noAction, UnityAction cancelAction,
        string yesText = "Yes", string noText = "No", string cancelText = "Cancel",
        CommonPopupScreenPosition commonPopupScreenPosition = CommonPopupScreenPosition.Middle)
    {
        if (isDialogShowing)
        {
            Debug.LogError("Popup dialog is already showing");
            return;
        }
        HideAllButtons();
        descriptionText.text = message;
        InitButton(uiSimpleButtons[0], yesAction, yesText);
        InitButton(uiSimpleButtons[1], noAction, noText);
        InitButton(uiSimpleButtons[2], cancelAction, cancelText);
        ShowDialogPanel(commonPopupScreenPosition);
    }

    private void InitButton(UiSimpleButton uiSimpleButton, UnityAction unityAction, string buttonText)
    {
        uiSimpleButton.gameObject.SetActive(true);
        uiSimpleButton.ChangeButtonText(buttonText);
        uiSimpleButton.RemoveListner();
        uiSimpleButton.AddListener(unityAction);
        uiSimpleButton.AddListener(HideDialogPanel);
    }
    #endregion

    private void HideAllButtons()
    {
        for (int i = 0; i < uiSimpleButtons.Count; i++)
        {
            uiSimpleButtons[i].gameObject.SetActive(false);
        }
    }

    public void ShowDialogPanel(CommonPopupScreenPosition commonPopupScreenPosition)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogPanel);
        isDialogShowing = true;
        transform.SetAsLastSibling();
        mainPanel.SetActive(true);
        switch (commonPopupScreenPosition)
        {
            case CommonPopupScreenPosition.Top:
                dialogPanel.DOAnchorPosY(topPosition, moveAnimDuration);
                break;
            case CommonPopupScreenPosition.Middle:
                dialogPanel.DOAnchorPosY(middlePosition, moveAnimDuration);
                break;
            case CommonPopupScreenPosition.Bottom:
                dialogPanel.DOAnchorPosY(bottomPosition, moveAnimDuration);
                break;
            default:
                break;
        }
    }

    private void HideDialogPanel()
    {
        isDialogShowing = false;
        dialogPanel.DOAnchorPosY(hidePosition, moveAnimDuration);
        mainPanel.SetActive(false);
    }
}


public enum CommonPopupScreenPosition
{
    Top,
    Middle,
    Bottom
}

// ***** Usage *****
// For Ok dialog => UiCommonPopupMenu.Instance.InitOkDialog("Ok test", OnOkClikced, "Ok, Thanks!");
// For Yes No dialog => UiCommonPopupMenu.Instance.InitYesNoDialog("Yes No test", OnYesClikced, OnNoClikced, "Save", "Discard");
// For Yes No Cancel dialog => UiCommonPopupMenu.Instance.InitYesNoCancelDialog("Yes No Cancel test", OnYesClikced, OnNoClikced, OnCancelClikced);
//Along with that, create functions to get the button actions like below.
//private void OnYesClikced(){print("Yes");}
//private void OnNoClikced(){print("No");}
//private void OnOkClikced(){print("Ok");}
//private void OnCancelClikced(){print("Cancel");}