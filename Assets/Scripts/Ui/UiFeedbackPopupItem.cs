using UnityEngine;
using TMPro;
using DG.Tweening;

public class UiFeedbackPopupItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private RectTransform rectTransform;

    public void Init(string messageToShow, float endPosY)
    {
        rectTransform = GetComponent<RectTransform>();
        text.text += messageToShow;
        transform.DOScaleX(1, 0.25f)
           .OnComplete(() => rectTransform.DOAnchorPosY(endPosY, 1.75f).SetEase(Ease.InSine)
           .OnComplete(() => transform.DOScaleX(0, 0.25f)
           .OnComplete(() => Destroy(gameObject))));

    }
}