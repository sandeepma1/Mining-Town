using UnityEngine;
using UnityEngine.UI;

public class UiSettingsCanvas : UiBasicCanvasWindowBase
{
    [Header("Child Class")]
    [Space(10)]
    [SerializeField] private Button resetGameButton;
    [SerializeField] private InputField itemIdInputField;
    [SerializeField] private InputField countInputField;
    [SerializeField] private Button addItemButton;
    [SerializeField] private Button removeItemButton;

    private void Start()
    {
        UiAllScreenButtonsCanvas.OnSettingButtonClick += OnSettingButtonClick;
        resetGameButton.onClick.AddListener(OnResetGameButtonClick);
        addItemButton.onClick.AddListener(OnAddItemButtonClick);
        removeItemButton.onClick.AddListener(OnRemoveItemButtonClick);
    }

    private void OnDestroy()
    {
        UiAllScreenButtonsCanvas.OnSettingButtonClick -= OnSettingButtonClick;
        resetGameButton.onClick.RemoveListener(OnResetGameButtonClick);
        addItemButton.onClick.RemoveListener(OnAddItemButtonClick);
        removeItemButton.onClick.RemoveListener(OnRemoveItemButtonClick);
    }

    private void OnSettingButtonClick()
    {
        ToggleCanvas(true);
    }

    private void OnAddItemButtonClick()
    {
        int itemId;
        int.TryParse(itemIdInputField.text, out itemId);
        int count;
        int.TryParse(countInputField.text, out count);
        SaveLoadManager.AddUpdateItem(itemId, count);
    }

    private void OnRemoveItemButtonClick()
    {
        int itemId;
        int.TryParse(itemIdInputField.text, out itemId);
        int count;
        int.TryParse(countInputField.text, out count);
        SaveLoadManager.RemoveFarmBarnItem(itemId, count);
    }

    private void OnResetGameButtonClick()
    {
        UiCommonPopupMenu.Instance.InitYesNoDialog(GameVariables.msg_resetGame, OnYes, OnNo);
    }

    private void OnYes()
    {
        SaveLoadManager.ResetGameData();
    }

    private void OnNo()
    {
        //Should be empty
    }
}