using UnityEngine;

public class FarmBarnHouse : MonoBehaviour
{
    private void OnMouseUpAsButton()
    {
        if (!ExtensionMethods.IsPointerOverUIObject())
        {
            MainCanvasManager.OnHideFloatingUi?.Invoke();
            UiFarmBarnInventory.OnShowBarnInventory?.Invoke();
        }
    }
}