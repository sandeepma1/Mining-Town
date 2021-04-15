using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiBlobButton : Button
{
    private Transform buttonPanel;
    private Vector3 downSize = new Vector3(0.75f, 0.75f, 0.75f);
    private const float animDuration = 0.25f;
    private const Ease ease = Ease.OutElastic;

    protected override void Start()
    {
        buttonPanel = transform.GetChild(0).transform;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        buttonPanel.DOScale(downSize, animDuration);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        buttonPanel.DOScale(Vector3.one, animDuration * 3).SetEase(ease);
    }
}