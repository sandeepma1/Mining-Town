using UnityEngine;
using UnityEngine.UI;

public class UiForestMenuCanvas : UiBasicCanvasWindowBase
{
    [Header("Child class")]
    [Space(10)]
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button goHomeButton;
    [SerializeField] private Toggle godModeToggle;

    private void Start()
    {
        settingsButton.onClick.AddListener(OnSettingButtonClick);
        goHomeButton.onClick.AddListener(OnGoHomeButtonClick);
        godModeToggle.onValueChanged.AddListener(OnGodMode);
        godModeToggle.isOn = SaveLoadManager.saveData.playerStats.isInGodMode;
        print("isInGodMode: " + SaveLoadManager.saveData.playerStats.isInGodMode);
    }

    private void OnDestroy()
    {
        goHomeButton.onClick.RemoveListener(OnGoHomeButtonClick);
        settingsButton.onClick.RemoveListener(OnSettingButtonClick);
        godModeToggle.onValueChanged.AddListener(OnGodMode);
    }

    private void OnGodMode(bool flag)
    {
        PlayerHealth.OnToggleGodMode?.Invoke(flag);
    }

    private void OnSettingButtonClick()
    {
        ToggleCanvas(true);
    }

    private void OnGoHomeButtonClick()
    {
        UiCommonPopupMenu.Instance.InitYesNoDialog(GameVariables.msg_gotoHomeLevel, OnYes, OnNo);
    }

    private void OnYes()
    {
        SaveLoadManager.saveData.playerStats.currentForestLevel = 1;
        SceneLoader.LoadLevelByName(this, Scenes.FarmHome);
    }

    private void OnNo()
    {
        //Should be empty
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
}
