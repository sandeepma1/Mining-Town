using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UiNavigationManagerCanvas : MonoBehaviour
{
    public static Action<bool> OnToggleCanvas;
    public static Action<int> OnMoveNavigateTabById;
    public static Action<int> OnNavigationButtonsClicked;
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private RectTransform mainPanel;
    [SerializeField] private UiNavigationButton[] uiNavigationButtons;
    [SerializeField] private RectTransform[] navigationCanvasesRect;
    [SerializeField] private int lastSelectedTabId = -1;
    private int screenWidth;
    private float panelHidePosY;
    private const float animDuration = 0.2f;

    private void Awake()
    {
        // UiBuildCanvas.OnToggleBuildMode += ToggleBuildMode;
        OnToggleCanvas += ToggleCanvas;
        OnMoveNavigateTabById += OnNavigateNextButtonPressed;
    }

    //As the rects are controlled by layout groups, its better to get rect dimensions in a co-routine 
    private void Start()
    {
        panelHidePosY = mainPanel.sizeDelta.y;
        screenWidth = (int)canvasScaler.referenceResolution.y;
        for (int i = 0; i < uiNavigationButtons.Length; i++)
        {
            uiNavigationButtons[i].id = i;
            uiNavigationButtons[i].OnButtonPressed += ToggleCanvacesById;
        }
        KeepCanvasToLeft(0);
        KeepCanvasToLeft(1);
        //lastSelectedTabId = 2;
        ToggleCanvacesById(2);
        KeepCanvasToRight(3);
        KeepCanvasToRight(4);
    }

    private void OnDestroy()
    {
        //UiBuildCanvas.OnToggleBuildMode -= ToggleBuildMode;
        OnToggleCanvas -= ToggleCanvas;
        OnMoveNavigateTabById -= OnNavigateNextButtonPressed;
        for (int i = 0; i < uiNavigationButtons.Length; i++)
        {
            uiNavigationButtons[i].OnButtonPressed -= ToggleCanvacesById;
        }
    }

    private void ToggleBuildMode(bool isBuildMode)
    {
        uiNavigationButtons[0].IsInteractable(!isBuildMode);
        uiNavigationButtons[4].IsInteractable(!isBuildMode);
    }

    private void OnNavigateNextButtonPressed(int id)
    {
        ToggleCanvacesById(id);
    }

    private void ToggleCanvacesById(int id)
    {
        if (lastSelectedTabId == id)
        {
            return;
        }
        OnNavigationButtonsClicked?.Invoke(id);

        //Disables last tab
        if (lastSelectedTabId >= 0)
        {
            uiNavigationButtons[lastSelectedTabId].ToggleSelection(false);
        }

        //enables currect tab
        uiNavigationButtons[id].ToggleSelection(true);
        ToggleAnimateCanvasGroupById(true, id);
        lastSelectedTabId = id;
    }

    private void ToggleAnimateCanvasGroupById(bool flag, int tabId)
    {
        AnimateCanvasToCenter(tabId);
        if (flag && lastSelectedTabId >= 0)
        {
            if (tabId > lastSelectedTabId)
            {
                AnimateCanvasToLeft(lastSelectedTabId);
            }
            else
            {
                AnimateCanvasToRight(lastSelectedTabId);
            }
        }
    }

    private void AnimateCanvasToCenter(int canvasId)
    {
        navigationCanvasesRect[canvasId].DOAnchorPosX(0, 0.5f);
    }

    private void KeepCanvasToLeft(int canvasId)
    {
        navigationCanvasesRect[canvasId].anchoredPosition = new Vector2(screenWidth * -1, 0);
    }

    private void AnimateCanvasToLeft(int canvasId)
    {
        navigationCanvasesRect[canvasId].DOAnchorPosX(screenWidth * -1, 0.5f);
    }

    private void KeepCanvasToRight(int canvasId)
    {
        navigationCanvasesRect[canvasId].anchoredPosition = new Vector2(screenWidth, 0);
    }

    private void AnimateCanvasToRight(int canvasId)
    {
        navigationCanvasesRect[canvasId].DOAnchorPosX(screenWidth, 0.5f);
    }

    private void ToggleCanvas(bool isVisible)
    {
        if (isVisible)
        {
            mainPanel.DOAnchorPosY(0, animDuration);
        }
        else
        {
            mainPanel.DOAnchorPosY(-panelHidePosY, animDuration);
        }
    }
}