using UnityEngine;

public class UiHealthBarCanvas : MonoBehaviour
{
    public static UiHealthBarCanvas Instance = null;
    [SerializeField] private UiHealthBar uiHealthBarPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public UiHealthBar GetUiHealthBarPrefab()
    {
        return uiHealthBarPrefab;
    }
}