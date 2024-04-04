using System;
using System.Collections;
using UnityEngine;

public class MainCanvasManager : MonoBehaviour
{
    public static Action<bool> OnToggleDragPan;
    public static Action OnHideFloatingUi;

    private void Awake()
    {
        OnToggleDragPan += OnToggleDragPanEvent;
        OnHideFloatingUi += OnHideFloatingUiEvent;
    }

    private void OnDestroy()
    {
        OnToggleDragPan -= OnToggleDragPanEvent;
        OnHideFloatingUi -= OnHideFloatingUiEvent;
    }

    private void OnHideFloatingUiEvent()
    {
        UiTimeRemainingCanvas.OnHideCanvas?.Invoke();
        UiRemoveByCoinCanvas.OnHideCanvas?.Invoke();
        UiCropsCanvas.OnHideCanvas?.Invoke();
    }

    private void OnToggleDragPanEvent(bool isVisible)
    {
        StartCoroutine(ToggleDragPanClickEvents(isVisible));
    }

    private IEnumerator ToggleDragPanClickEvents(bool isVisible)
    {
        yield return new WaitForEndOfFrame();
        SwipeDetector.OnToggleSwipe?.Invoke(isVisible);
        //CameraPanDrag.OnTogglePan?.Invoke(isVisible);
    }
}