using UnityEngine;

public class FarmGroundClicker : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (!ExtensionMethods.IsPointerOverUIObject())
        {
            MainCanvasManager.OnHideFloatingUi?.Invoke();
        }
    }
}
