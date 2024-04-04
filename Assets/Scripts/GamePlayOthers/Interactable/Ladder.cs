using UnityEngine;

public class Ladder : MonoBehaviour
{
    public void OnLadderInteract()
    {
        GameEvents.PauseGame();
        Joystick.OnJoystickReset?.Invoke();
        UiCommonPopupMenu.Instance.InitYesNoDialog(GameVariables.msg_gotoNextMinesLevel, OnYes, OnNo);
    }

    private void OnYes()
    {
        SaveLoadManager.saveData.playerStats.currentMinesLevel++;
        if (SaveLoadManager.saveData.playerStats.currentMinesLevel > 10) // levels completed go home
        {
            SceneLoader.LoadLevelByName(this, Scenes.FarmHome);
        }
        else // goto next level
        {
            SceneLoader.LoadLevelByName(this, Scenes.Mines);
        }
    }

    private void OnNo()
    {
        GameEvents.ResumeGame();
    }
}