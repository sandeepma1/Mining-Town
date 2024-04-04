using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UiLevelItem : MonoBehaviour
{
    public Action<int> OnButtonClick;
    [SerializeField] private TextMeshProUGUI buttonText;
    private Button button;
    private int levelNumber;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClicked);
    }

    public void Init(int levelNumber)
    {
        this.levelNumber = levelNumber;
        buttonText.text = "Level " + levelNumber;
    }

    private void OnButtonClicked()
    {
        OnButtonClick?.Invoke(levelNumber);
    }
}
