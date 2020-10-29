using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderingManager : MonoBehaviour
{
    [ColorUsageAttribute(true, true)] public Color outdoorColour;
    [ColorUsageAttribute(true, true)] public Color indoorColour;
    private Scenes currentScene;

    private void Start()
    {
        SceneLoader.OnSceneChanged += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneLoader.OnSceneChanged -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scenes scene)
    {
        currentScene = scene;
        switch (currentScene)
        {
            case Scenes.Loading:
                break;
            case Scenes.Forest:
            case Scenes.Town:
            case Scenes.FarmHome:
                RenderSettings.ambientLight = outdoorColour;
                break;
            case Scenes.Mines:
                RenderSettings.ambientLight = indoorColour;
                break;
            default:
                break;
        }
    }
}
