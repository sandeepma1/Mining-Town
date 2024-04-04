using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiStartMinesCanvas : UiBasicCanvasWindowBase
{
    public static Action OnShowStartMinesMenu;
    [Header("Child Class")]
    [Space(10)]
    [SerializeField] private Button startMinesButton;
    [SerializeField] private TextMeshProUGUI startMinesButtonText;

    private void Start()
    {
        OnShowStartMinesMenu += ShowStartMinesMenu;
        startMinesButton.onClick.AddListener(OnStartMinesClicked);
    }

    private void OnDestroy()
    {
        OnShowStartMinesMenu -= ShowStartMinesMenu;
        startMinesButton.onClick.RemoveListener(OnStartMinesClicked);
    }

    private void ShowStartMinesMenu()
    {
        ToggleCanvas(true);
    }

    protected override void ToggleCanvas(bool isVisible)
    {
        base.ToggleCanvas(isVisible);
        if (isVisible)
        {
            GameEvents.PauseGame();
        }
        else
        {
            GameEvents.ResumeGame();
        }
    }

    private void OnLevelSelected(int levelNumber)
    {
        SaveLoadManager.saveData.playerStats.currentMinesLevel = levelNumber;
        startMinesButtonText.text = "Start Mining Level " + levelNumber;
        startMinesButton.interactable = true;
    }

    private void OnStartMinesClicked()
    {
        ToggleCanvas(false);
        SaveLoadManager.saveData.playerStats.currentMinesLevel = 1;
        SceneLoader.LoadLevelByName(this, Scenes.Mines);
    }
}