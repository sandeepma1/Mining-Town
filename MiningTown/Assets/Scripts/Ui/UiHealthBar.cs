using UnityEngine;
using UnityEngine.UI;

public class UiHealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarFillImage;
    private RectTransform healthBar;
    private Transform objectToFollow;
    private Camera mainCamera;
    private CanvasGroup canvasGroup;
    private RectTransform targetCanvasRect;

    public void Init(Transform targetTransform, RectTransform targetCanvasRect)
    {
        healthBar = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        this.targetCanvasRect = targetCanvasRect;
        mainCamera = Camera.main;
        objectToFollow = targetTransform;
        SetToTargetPosition();
    }

    public void OnHealthChanged(float healthFill)
    {
        healthBarFillImage.fillAmount = healthFill;
        canvasGroup.alpha = 1;
    }

    private void LateUpdate()
    {
        SetToTargetPosition();
    }

    private void SetToTargetPosition()
    {
        Vector2 viewportPosition = mainCamera.WorldToViewportPoint(objectToFollow.transform.position);
        Vector2 screenPosition = new Vector2(
        ((viewportPosition.x * targetCanvasRect.sizeDelta.x) - (targetCanvasRect.sizeDelta.x * 0.5f)),
        ((viewportPosition.y * targetCanvasRect.sizeDelta.y) - (targetCanvasRect.sizeDelta.y * 0.5f)));
        healthBar.anchoredPosition = screenPosition;
    }
}