using UnityEngine;
using System;

public class EditModeColliders : MonoBehaviour
{
    public static Action<bool> OnToggleEditModeColliders;
    [SerializeField] private GameObject gridRenderer;
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Collider[] colliders;

    private void Start()
    {
        OnToggleEditModeColliders += ToggleEditModeColliders;
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
        foreach (Renderer child in renderers)
        {
            child.enabled = false;//.gameObject.SetActive(false);
        }
        ToggleEditModeColliders(false);
    }

    private void OnDestroy()
    {
        OnToggleEditModeColliders -= ToggleEditModeColliders;
    }

    private void ToggleEditModeColliders(bool isEnabled)
    {
        foreach (Collider child in colliders)
        {
            child.enabled = isEnabled;
        }
        gridRenderer.gameObject.SetActive(isEnabled);
    }
}
