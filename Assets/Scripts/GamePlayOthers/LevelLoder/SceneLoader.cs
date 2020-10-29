using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static Action<Scenes> OnSceneChanged;
    private static Scenes lastLoadedScene;

    public static void LoadLevelByName(MonoBehaviour monoBehaviour, Scenes scenes)
    {
        UiTransitionCanvas.OnCloseTransition?.Invoke();
        monoBehaviour.StartCoroutine(LoadLevelAfterEndOfFrame(scenes));
    }

    private static IEnumerator LoadLevelAfterEndOfFrame(Scenes currentScene)
    {
        yield return new WaitForSeconds(GameVariables.transistionDuration);
        OnSceneChanged?.Invoke(currentScene);
        if (lastLoadedScene != Scenes.Loading)
        {
            SceneManager.UnloadSceneAsync(lastLoadedScene.ToString());
        }
        SceneManager.LoadSceneAsync(currentScene.ToString(), LoadSceneMode.Additive);
        switch (lastLoadedScene)
        {
            case Scenes.Loading:
                break;
            case Scenes.FarmHome:
                break;
            case Scenes.Mines:
                break;
            case Scenes.Forest:
                break;
            case Scenes.Town:
                break;
            default:
                break;
        }
        lastLoadedScene = currentScene;

    }
}