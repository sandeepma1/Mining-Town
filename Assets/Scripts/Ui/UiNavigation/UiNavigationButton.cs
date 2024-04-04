using UnityEngine;
using UnityEngine.UI;
using System;

public class UiNavigationButton : MonoBehaviour
{
    public Action<int> OnButtonPressed;
    [HideInInspector] public int id;
    private Button button;
    private Image buttonImage;

    private void Start()
    {
        button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        button.onClick.AddListener(OnButtonPress);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonPress);
    }

    private void OnButtonPress()
    {
        OnButtonPressed?.Invoke(id);
    }

    public void ToggleSelection(bool flag)
    {
        if (flag) //Selected
        {
            buttonImage.color = ColorConstants.SelectedColor;
        }
        else //Deselected
        {
            buttonImage.color = ColorConstants.DeselectedColor;
        }
    }

    public void IsInteractable(bool flag)
    {
        button.interactable = flag;
    }
}