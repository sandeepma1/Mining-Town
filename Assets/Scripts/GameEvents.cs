using System;
using UnityEngine;

public static class GameEvents
{
    public static Action OnPauseGame;
    public static Action OnResumeGame;
    private static bool isGamePaused;

    public static void PauseGame()
    {
        //Debug.Log("PauseGame ");
        isGamePaused = true;
        OnPauseGame?.Invoke();
    }

    public static void ResumeGame()
    {
        //Debug.Log("ResumeGame ");
        isGamePaused = false;
        OnResumeGame?.Invoke();
    }

    public static bool IsGamePaused()
    {
        return isGamePaused;
    }
}