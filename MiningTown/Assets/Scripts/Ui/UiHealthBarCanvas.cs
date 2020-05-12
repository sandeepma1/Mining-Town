using UnityEngine;

public class UiHealthBarCanvas : MonoBehaviour
{
    public static UiHealthBarCanvas Instance = null;
    [SerializeField] private UiHealthBar uiHealthBarPrefab;
    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Instance = this;
    }

    public UiHealthBar CreateHealthBar(Transform targetTransform)
    {
        UiHealthBar uiHealthBar = Instantiate(uiHealthBarPrefab);
        uiHealthBar.transform.SetParent(transform);
        uiHealthBar.transform.localScale = Vector3.one;
        uiHealthBar.Init(targetTransform, rectTransform);
        return uiHealthBar;
    }
}