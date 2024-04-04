using UnityEngine;

public class UiHealthBarCanvas : MonoBehaviour
{
    public static UiHealthBarCanvas Instance = null;
    [SerializeField] private UiHealthBar uiHealthBarPrefab;
    private Canvas mainCanvas;

    private void Awake()
    {
        Instance = this;
        mainCanvas = GetComponent<Canvas>();
    }

    public UiHealthBar CreateHealthBar(Transform targetTransform)
    {
        UiHealthBar uiHealthBar = Instantiate(uiHealthBarPrefab, transform);
        uiHealthBar.Init(targetTransform, mainCanvas);
        return uiHealthBar;
    }
}