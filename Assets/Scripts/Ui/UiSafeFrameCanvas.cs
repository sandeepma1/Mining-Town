using UnityEngine;
using System.Collections;

public class UiSafeFrameCanvas : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(GetSetSafeFrame());
    }

    private IEnumerator GetSetSafeFrame()
    {
        yield return new WaitForEndOfFrame();
        Rect safeRect = Screen.safeArea;
        GetComponent<RectTransform>().rect.Set(safeRect.x, safeRect.y, safeRect.width, safeRect.height);
    }
}