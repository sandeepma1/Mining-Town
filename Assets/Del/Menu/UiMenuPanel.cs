using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiMenuPanel : MonoBehaviour
{
    [SerializeField] private Button showHideButton;
    [SerializeField] private RectTransform mainPanel;
    [SerializeField] private Ease showEase;
    [SerializeField] private Ease hideEase;
    [SerializeField] private float hidePos = -100;
    private bool isMenuVisible;
    private const float animDuration = 0.15f;

    private void Start()
    {
        showHideButton.onClick.AddListener(OnShowHideButtonClicked);
        isMenuVisible = false;
        TogglePanel();
    }

    private void OnShowHideButtonClicked()
    {
        isMenuVisible = !isMenuVisible;
        TogglePanel();
    }

    private void TogglePanel()
    {
        if (isMenuVisible)
        {
            mainPanel.anchoredPosition = new Vector2(0, hidePos);
            mainPanel.gameObject.SetActive(true);
            mainPanel.DOAnchorPosY(0, animDuration).SetEase(showEase);
        }
        else
        {
            mainPanel.DOAnchorPosY(hidePos, animDuration).SetEase(hideEase).OnComplete(() => mainPanel.gameObject.SetActive(false));
        }
    }
}
