﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Unmask/UnmaskRaycastFilter", 2)]
public class UnmaskRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
{
    [Tooltip("Target unmask component. The ray passes through the unmasked rectangle.")]
    [SerializeField] Unmask m_TargetUnmask;


    public Unmask targetUnmask { get { return m_TargetUnmask; } set { m_TargetUnmask = value; } }

    /// <summary>
    /// Given a point and a camera is the raycast valid.
    /// </summary>
    /// <returns>Valid.</returns>
    /// <param name="sp">Screen position.</param>
    /// <param name="eventCamera">Raycast camera.</param>
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        // Skip if deactived.
        if (!isActiveAndEnabled || !m_TargetUnmask || !m_TargetUnmask.isActiveAndEnabled)
        {
            return true;
        }

        // check inside
        return !RectTransformUtility.RectangleContainsScreenPoint((m_TargetUnmask.transform as RectTransform), sp);
    }


    //################################
    // Private Members.
    //################################

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
    }
}