using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiMainCanvas : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
    }
}
