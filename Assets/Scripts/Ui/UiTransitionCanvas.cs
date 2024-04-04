using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UiTransitionCanvas : Singleton<UiTransitionCanvas>
{
    public static Action OnCloseTransition;
    public static Action<Transform> OnCloseTransitionToPlayer;
    [SerializeField] private RectTransform maskToScale;
    [SerializeField] private GameObject backgroundScreen;
    private Vector3 maxScale = new Vector3(6, 6, 6);
    private RectTransform mainCanvasRect;

    protected override void Awake()
    {
        base.Awake();
        mainCanvasRect = GetComponent<RectTransform>();
        SceneManager.sceneLoaded += LoadScene;
        OnCloseTransition += CloseTransition;
    }

    private void Start()
    {
        backgroundScreen.SetActive(true);
        ToggleTransisiton(true);
    }

    private void OnDestroy()
    {
        OnCloseTransition -= CloseTransition;
        SceneManager.sceneLoaded -= LoadScene;
    }

    private void CloseTransition()
    {
        ToggleTransisiton(true);
    }

    private void ToggleTransisiton(bool isClosed)
    {
        if (PlayerMovement.Instance != null)
        {
            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(PlayerMovement.Instance.GetPlayerPosition());
            Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * mainCanvasRect.sizeDelta.x) - (mainCanvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * mainCanvasRect.sizeDelta.y) - (mainCanvasRect.sizeDelta.y * 0.5f)));
            maskToScale.anchoredPosition = WorldObject_ScreenPosition;
        }
        if (isClosed)
        {
            maskToScale.DOScale(Vector3.zero, GameVariables.transistionDuration);
        }
        else
        {
            maskToScale.DOScale(maxScale, GameVariables.transistionDuration);
        }
    }

    private void LoadScene(Scene scene, LoadSceneMode loadSceneMode)
    {
        StartCoroutine(OnLevelLoaded());
    }

    private IEnumerator OnLevelLoaded()
    {
        yield return new WaitForSeconds(GameVariables.transistionDuration);
        ToggleTransisiton(false);
    }
}