using System;
using UnityEngine;
using UnityEngine.UI;

public class UiPlayerDiedCanvas : MonoBehaviour
{
    [SerializeField] private Button respawnButton;
    [SerializeField] private GameObject mainPanel;

    private void Start()
    {
        mainPanel.SetActive(false);
        PlayerCurrencyManager.OnPlayerDied += OnPlayerDied;
        respawnButton.onClick.AddListener(RestartLevel);
    }

    private void OnDestroy()
    {
        PlayerCurrencyManager.OnPlayerDied -= OnPlayerDied;
        respawnButton.onClick.RemoveListener(RestartLevel);
    }

    private void RestartLevel()
    {
        SceneLoader.LoadLevelByName(this, Scenes.FarmHome);
    }

    private void OnPlayerDied()
    {
        GameEvents.PauseGame();
        mainPanel.SetActive(true);
    }
}