using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UiBasicCanvasWindowBase : MonoBehaviour
{
    [Header("Base Class")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button closeButtonBackground;
    [SerializeField] protected GameObject mainPanel;
    protected bool isPopup = false;

    protected virtual void Awake()
    {
        closeButton.onClick.AddListener(ClosePanel);
        closeButtonBackground.onClick.AddListener(ClosePanel);
        ToggleCanvas(false);
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(ClosePanel);
        closeButtonBackground.onClick.RemoveListener(ClosePanel);
    }

    protected void ClosePanel()
    {
        ToggleCanvas(false);
    }

    protected virtual void ToggleCanvas(bool isVisible)
    {
        SuperBlurManager.OnToggleSuperBlur?.Invoke(isVisible);
        if (!isPopup)
        {
            UiAllScreenButtonsCanvas.OnToggleAllButtons?.Invoke(!isVisible);
        }
        StartCoroutine(ToggleCanvasDelay(isVisible));
    }

    private IEnumerator ToggleCanvasDelay(bool isVisible)
    {
        yield return new WaitForEndOfFrame();
        closeButtonBackground.gameObject.SetActive(isVisible);
        mainPanel.gameObject.SetActive(isVisible);
    }
}