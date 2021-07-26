using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UiBasicCanvasWindowBase : MonoBehaviour
{
    [Header("Base Class")]
    [SerializeField] private Button closeButton = null;
    [SerializeField] private Button closeButtonBackground = null;
    [SerializeField] protected Button actionButton = null;
    [SerializeField] protected GameObject actionPanel = null;
    [SerializeField] protected GameObject mainPanel = null;
    protected bool isPopup = false;

    protected virtual void Awake()
    {
        closeButton?.onClick.AddListener(CloseActionPanel);
        closeButtonBackground?.onClick.AddListener(CloseActionPanel);
        actionButton?.onClick.AddListener(OpenActionPanel);
        CloseActionPanel();
        ToggleCanvas(false);
    }

    private void OnDestroy()
    {
        closeButton?.onClick.RemoveListener(CloseActionPanel);
        closeButtonBackground?.onClick.RemoveListener(CloseActionPanel);
        actionButton?.onClick.RemoveListener(OpenActionPanel);
    }

    protected virtual void ToggleActionPanel(bool isVisible)
    {
        if(isVisible)
        {
            OpenActionPanel();
        }
        else
        {
            CloseActionPanel();
        }
    }

    protected void OpenActionPanel()
    {
        SuperBlurManager.OnToggleSuperBlur?.Invoke(true);
        if (!isPopup)
        {
            UiAllScreenButtonsCanvas.OnToggleAllButtons?.Invoke(false);
        }

        closeButtonBackground?.gameObject.SetActive(true);
        actionButton?.gameObject.SetActive(false);
        if (actionPanel)
            actionPanel.SetActive(true);
    }

    protected void CloseActionPanel()
    {
        SuperBlurManager.OnToggleSuperBlur?.Invoke(false);
        if (!isPopup)
        {
            UiAllScreenButtonsCanvas.OnToggleAllButtons?.Invoke(true);
        }

        actionButton?.gameObject.SetActive(true);
        closeButtonBackground?.gameObject.SetActive(false);
        if (actionPanel)
            actionPanel.SetActive(false);
        else
            ClosePanel();
    }

    protected void ClosePanel()
    {
        ToggleCanvas(false);
    }

    protected virtual void ToggleCanvas(bool isVisible)
    {
        StartCoroutine(ToggleCanvasDelay(isVisible));
    }

    private IEnumerator ToggleCanvasDelay(bool isVisible)
    {
        yield return new WaitForEndOfFrame();
        mainPanel.gameObject.SetActive(isVisible);

        if (actionButton != null)
        {
            actionButton.gameObject.SetActive(isVisible);
        }
        else
        {
            if (actionPanel)
                actionPanel.SetActive(isVisible);
        }
    }
}