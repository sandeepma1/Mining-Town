using System;
using UnityEngine;

[RequireComponent(typeof(SuperBlur.SuperBlurFast))]
public class SuperBlurManager : MonoBehaviour
{
    public static Action<bool> OnToggleSuperBlur;
    private SuperBlur.SuperBlurFast superBlurFast;

    private void Start()
    {
        superBlurFast = GetComponent<SuperBlur.SuperBlurFast>();
        ToggleSuperBlur(false);
        OnToggleSuperBlur += ToggleSuperBlur;
    }

    private void OnDestroy()
    {
        OnToggleSuperBlur -= ToggleSuperBlur;
    }

    private void ToggleSuperBlur(bool isEnabled)
    {
        superBlurFast.enabled = isEnabled;
    }
}