using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiFeedbackPopupCanvas : MonoBehaviour
{
    public static Action<string> OnShowFeedbackPopup;
    [SerializeField] private UiFeedbackPopupItem uiFeedbackPopupItemPrefab;
    private float spawnYPos;
    private float halfHeight;
    private void Start()
    {
        halfHeight = Screen.height / 2;
        spawnYPos = halfHeight - (halfHeight / 3);
        OnShowFeedbackPopup += ShowFeedbackPopup;
    }

    private void OnDestroy()
    {
        OnShowFeedbackPopup -= ShowFeedbackPopup;
    }

    private void ShowFeedbackPopup(string messageToShow)
    {
        UiFeedbackPopupItem uiFeedbackPopupItem = Instantiate(uiFeedbackPopupItemPrefab, transform);
        uiFeedbackPopupItem.Init(messageToShow, halfHeight);
        uiFeedbackPopupItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, spawnYPos);
    }
}
