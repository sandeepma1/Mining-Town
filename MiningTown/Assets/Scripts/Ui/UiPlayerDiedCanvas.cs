using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiPlayerDiedCanvas : MonoBehaviour
{
    public static Action ShowPlayerDiedCanvas;
    [SerializeField] private Button respawnButton;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        ShowPlayerDiedCanvas += ShowCanvas;
        canvasGroup = GetComponent<CanvasGroup>();
        ToggleCanvasGroup(false);
        respawnButton.onClick.AddListener(RestartLevel);
    }

    private void OnDestroy()
    {
        ShowPlayerDiedCanvas -= ShowCanvas;
        respawnButton.onClick.RemoveListener(RestartLevel);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void ShowCanvas()
    {
        ToggleCanvasGroup(true);
    }

    private void ToggleCanvasGroup(bool isActive)
    {
        canvasGroup.blocksRaycasts = isActive;
        canvasGroup.interactable = isActive;
        if (isActive)
        {
            canvasGroup.alpha = 1;
        }
        else
        {
            canvasGroup.alpha = 0;
        }
    }
}