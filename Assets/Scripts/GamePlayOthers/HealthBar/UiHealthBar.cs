using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiHealthBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthMinusText;
    [SerializeField] private Image healthBarFillImage;
    private RectTransform healthBar;
    private RectTransform healthMinusTextRect;
    private Transform objectToFollow;
    private Camera mainCamera;
    private CanvasGroup canvasGroup;
    private Sequence sequence;
    private const float upPosition = 100f;
    private const float animDurtaion = 0.75f;
    private Canvas canvas;

    public void Init(Transform targetTransform, Canvas canvas)
    {
        this.canvas = canvas;
        sequence = DOTween.Sequence();
        healthMinusTextRect = healthMinusText.GetComponent<RectTransform>(); ;
        healthBar = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        mainCamera = Camera.main;
        objectToFollow = targetTransform;
        SetToTargetPosition();
    }

    public void OnHealthChanged(int hitpoint, float currentHealth, float maxHealth)
    {
        healthBarFillImage.fillAmount = currentHealth / maxHealth;
        if (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha = 1;
        }
        //sequence.Kill(); This should work but not working
        DOTween.Kill(healthMinusTextRect);
        DOTween.Kill(healthMinusText);
        ShowHealthReductionAnimations(hitpoint);
    }

    private void ShowHealthReductionAnimations(int hitpoint)
    {
        healthMinusText.text = "-" + hitpoint;
        healthMinusText.color = Color.white;
        healthMinusTextRect.anchoredPosition = Vector2.zero;
        sequence.Append(healthMinusTextRect.DOLocalMoveY(upPosition, animDurtaion));
        sequence.Append(healthMinusText.DOColor(Color.clear, animDurtaion * 2));
    }

    private void LateUpdate()
    {
        SetToTargetPosition();
    }

    private void SetToTargetPosition()
    {
        healthBar.anchoredPosition = objectToFollow.WorldSpaceToUiSpace(canvas);
    }
}