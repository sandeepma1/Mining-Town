using System;
using UnityEngine;
using UnityEngine.UI;

public class UiStartForestCanvas : UiBasicCanvasWindowBase
{
    public static Action OnShowStartForestMenu;
    [Header("Child Class")]
    [Space(10)]
    [SerializeField] private Button startForestButton;

    private void Start()
    {
        OnShowStartForestMenu += ShowStartForestMenu;
        startForestButton.onClick.AddListener(OnStartForestClicked);
    }

    private void OnDestroy()
    {
        OnShowStartForestMenu -= ShowStartForestMenu;
        startForestButton.onClick.RemoveListener(OnStartForestClicked);
    }

    private void ShowStartForestMenu()
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

    private void OnStartForestClicked()
    {
        ToggleCanvas(false);
        SaveLoadManager.saveData.playerStats.currentForestLevel = 1;
        SceneLoader.LoadLevelByName(this, Scenes.Forest);
    }
}