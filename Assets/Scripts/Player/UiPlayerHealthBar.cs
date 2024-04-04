using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiPlayerHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private TextMeshProUGUI healthMinusText;
    [SerializeField] private TextMeshProUGUI currentHealthCountText;
    [SerializeField] private Image healthBarFillImage;
    private RectTransform healthMinusTextRect;
    private Sequence sequence;
    private const float upPosition = 100f;
    private const float animDurtaion = 0.75f;
    private int lastHealth;

    public void Init()
    {
        sequence = DOTween.Sequence();
        healthMinusTextRect = healthMinusText.GetComponent<RectTransform>();
        lastHealth = PlayerCurrencyManager.Health;
    }

    public void OnHealthChanged()
    {
        if (PlayerCurrencyManager.Health >= SaveLoadManager.saveData.playerStats.maxHealth)
        {
            ToggleHideHealthBar(false);
        }
        else
        {
            ToggleHideHealthBar(true);
        }
        currentHealthCountText.text = PlayerCurrencyManager.Health.ToString("F0");
        healthBarFillImage.fillAmount = (float)PlayerCurrencyManager.Health / (float)SaveLoadManager.saveData.playerStats.maxHealth;
        //sequence.Kill(); This should work but not working
        DOTween.Kill(healthMinusTextRect);
        DOTween.Kill(healthMinusText);
        ShowHealthReductionAnimations(lastHealth - PlayerCurrencyManager.Health);
        lastHealth = PlayerCurrencyManager.Health;
    }

    private void ShowHealthReductionAnimations(int damage)
    {
        if (damage <= 0)
        {
            return;
        }
        healthMinusText.text = "-" + damage;
        healthMinusText.color = Color.white;
        healthMinusTextRect.anchoredPosition = Vector2.zero;
        sequence.Append(healthMinusTextRect.DOLocalMoveY(upPosition, animDurtaion));
        sequence.Append(healthMinusText.DOColor(Color.clear, animDurtaion * 2));
    }

    private void ToggleHideHealthBar(bool isVisible)
    {
        mainPanel.SetActive(isVisible);
    }
}