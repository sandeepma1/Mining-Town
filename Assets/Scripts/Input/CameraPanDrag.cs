using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class CameraPanDrag : MonoBehaviour
{
    [SerializeField] private float PanSpeed = 20f;
    private Camera cam;
    private Vector2 lastPanPosition;
    private int panFingerId; // Touch mode only
    private Vector3 panPos;
    private List<Touch> touches = new List<Touch>();

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        HandleTouch();
    }

    private void HandleTouch()
    {
        touches = InputHelper.GetTouches();
        if (touches.Count != 1 || ExtensionMethods.IsPointerOverUIObject())
        {
            return;
        }
        if (touches[0].phase == TouchPhase.Began)
        {
            lastPanPosition = touches[0].position;
            panFingerId = touches[0].fingerId;
        }
        else if (touches[0].fingerId == panFingerId && touches[0].phase == TouchPhase.Moved)
        {
            Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - touches[0].position);
            panPos = offset * PanSpeed;
            lastPanPosition = touches[0].position;
            transform.position = transform.position + new Vector3(panPos.x, 0, panPos.y);
        }
    }
}