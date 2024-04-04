using UnityEngine;

public class ForestExit : MonoBehaviour
{
    public void OnForestExitInteract()
    {
        GameEvents.PauseGame();
        Joystick.OnJoystickReset?.Invoke();
        UiCommonPopupMenu.Instance.InitYesNoDialog(GameVariables.msg_gotoNextForestLevel, OnYes, OnNo);
    }

    private void OnYes()
    {
        SaveLoadManager.saveData.playerStats.currentForestLevel++;
        if (SaveLoadManager.saveData.playerStats.currentForestLevel > 10) // levels completed go home
        {
            SceneLoader.LoadLevelByName(this, Scenes.FarmHome);
        }
        else // goto next level
        {
            SceneLoader.LoadLevelByName(this, Scenes.Forest);
        }
    }

    private void OnNo()
    {
        GameEvents.ResumeGame();
    }
}