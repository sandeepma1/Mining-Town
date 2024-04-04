using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiToggleItem : MonoBehaviour
{
    public Action<int> OnToggleValueChanged;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Image boxImage;
    private Button button;
    private int index;

    public void Init(int id)
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SelectButton);
        index = id;
    }

    private void OnDestroy()
    {
        button.onClick.AddListener(SelectButton);
    }

    public void DeselectButton()
    {
        boxImage.color = ColorConstants.DeselectedColor;
        buttonText.color = ColorConstants.DeselectedColor;
    }

    private void SelectButton()
    {
        boxImage.color = ColorConstants.SelectedColor;
        buttonText.color = ColorConstants.SelectedColor;
        OnToggleValueChanged?.Invoke(index);
    }
}