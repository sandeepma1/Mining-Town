using System;
using UnityEngine;
using UnityEngine.UI;

public class UiTabButton : MonoBehaviour
{
    public Action<int> OnTabButtonClicked;
    private int tabId;
    private Button tabButton;
    private Image tabButtonImage;
    private RectTransform buttonRect;
    private const float bigButtonSize = 110;
    private const float defaultButtonSize = 100;

    public void Init(int id)
    {
        tabButton = GetComponent<Button>();
        buttonRect = tabButton.GetComponent<RectTransform>();
        tabButtonImage = tabButton.GetComponent<Image>();
        tabButton.onClick.AddListener(TabButtonClicked);
        tabId = id;
        buttonRect.sizeDelta = new Vector2(buttonRect.sizeDelta.x, defaultButtonSize);
    }

    public void ToggleButtonPressed(Color color, bool isSelected)
    {
        tabButtonImage.color = color;
        if (isSelected)
        {
            buttonRect.sizeDelta = new Vector2(buttonRect.sizeDelta.x, bigButtonSize);
        }
        else
        {
            buttonRect.sizeDelta = new Vector2(buttonRect.sizeDelta.x, defaultButtonSize);
        }
    }

    private void OnDestroy()
    {
        tabButton.onClick.RemoveListener(TabButtonClicked);
    }

    private void TabButtonClicked()
    {
        OnTabButtonClicked?.Invoke(tabId);
    }
}