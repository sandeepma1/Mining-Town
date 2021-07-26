using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UiBasicCanvasWindowBase : MonoBehaviour
{
    [Header("Base Class")]
    [SerializeField] private Button closeButton = null;
    [SerializeField] private Button closeButtonBackground = null;
    [SerializeField] protected Button actionButton = null;
    [SerializeField] protected GameObject mainPanel = null;
    protected bool isPopup = false;

    protected virtual void Awake()
    {
        closeButton?.onClick.AddListener(CloseMainPanel);
        closeButtonBackground?.onClick.AddListener(CloseMainPanel);
        actionButton?.onClick.AddListener(OpenMainPanel);
        CloseMainPanel();
        ToggleCanvas(false);
    }

    private void OnDestroy()
    {
        closeButton?.onClick.RemoveListener(CloseMainPanel);
        closeButtonBackground?.onClick.RemoveListener(CloseMainPanel);
        actionButton?.onClick.RemoveListener(OpenMainPanel);
    }

    protected virtual void ToggleMainPanel(bool isVisible)
    {
        if(isVisible)
        {
            OpenMainPanel();
        }
        else
        {
            CloseMainPanel();
        }
    }

    protected void OpenMainPanel()
    {
        SuperBlurManager.OnToggleSuperBlur?.Invoke(true);
        if (!isPopup)
        {
            UiAllScreenButtonsCanvas.OnToggleAllButtons?.Invoke(false);
        }

        closeButtonBackground?.gameObject.SetActive(true);
        actionButton?.gameObject.SetActive(false);
        mainPanel.SetActive(true);
    }

    protected void CloseMainPanel()
    {
        SuperBlurManager.OnToggleSuperBlur?.Invoke(false);
        if (!isPopup)
        {
            UiAllScreenButtonsCanvas.OnToggleAllButtons?.Invoke(true);
        }

        closeButtonBackground?.gameObject.SetActive(false);
        actionButton?.gameObject.SetActive(true);
        mainPanel.SetActive(false);
    }

    protected virtual void ToggleCanvas(bool isVisible)
    {
        StartCoroutine(ToggleCanvasDelay(isVisible));
    }

    private IEnumerator ToggleCanvasDelay(bool isVisible)
    {
        yield return new WaitForEndOfFrame();

        if (actionButton != null)
        {
            actionButton.gameObject.SetActive(isVisible);
        }
        else
        {
            mainPanel.gameObject.SetActive(isVisible);
        }
    }
}