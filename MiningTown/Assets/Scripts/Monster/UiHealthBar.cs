using UnityEngine;
using UnityEngine.UI;

public class UiHealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarFillImage;
    private RectTransform targetCanvasRect;
    private RectTransform healthBar;
    private Transform objectToFollow;
    private Camera mainCamera;

    private void Start()
    {
        targetCanvasRect = transform.parent.GetComponent<RectTransform>();
        healthBar = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    public void Init(Transform targetTransform)
    {
        objectToFollow = targetTransform;
        transform.SetParent(UiHealthBarCanvas.Instance.GetTransform());
    }

    public void OnHealthChanged(float healthFill)
    {
        healthBarFillImage.fillAmount = healthFill;
    }

    private void LateUpdate()
    {
        Vector2 ViewportPosition = mainCamera.WorldToViewportPoint(objectToFollow.transform.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * targetCanvasRect.sizeDelta.x) - (targetCanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * targetCanvasRect.sizeDelta.y) - (targetCanvasRect.sizeDelta.y * 0.5f)));
        healthBar.anchoredPosition = WorldObject_ScreenPosition;
    }
}