using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraRotaterAuto : MonoBehaviour
{
    public static Action<bool> OnCameraFacingFrame;
    public static Action<bool> OnCameraFacingLeftTemple;
    [SerializeField] private Transform designCamera;
    [SerializeField] private float minZoom = -220;
    [SerializeField] private float maxZoom = -100;
    [SerializeField] private float scrollScale = 2;
    [SerializeField] private float scrollDuration = 0.1f;

    private bool isDragging = false;
    private Vector2 oldTouchPosition;
    private const float snapDuation = 0.35f;
    private const float maxRotationAngle = 5;
    private const float leftAngle = 270;
    private const float dragSpeed = 0.15f;
    private Vector3 leftAngleVector = new Vector3(0, leftAngle, 0);
    private float zoomPosition = 0;

    private void Start()
    {
        UiDragDetector.OnDragEvent += OnDragEvent;
        UiDragDetector.OnBeginDragEvent += OnBeginDragEvent;
    }

    private void OnDestroy()
    {
        UiDragDetector.OnDragEvent -= OnDragEvent;
        UiDragDetector.OnBeginDragEvent -= OnBeginDragEvent;
    }

    private void OnPanSliderMoved(float panSliderValue)
    {
        transform.DOMoveY(panSliderValue, 0.1f);
    }

    #region Drag controller
    private void OnBeginDragEvent(Vector2 pos)
    {
        oldTouchPosition = pos;
    }

    private void OnDragEvent(bool flag) //true is dragging and false is end
    {
        isDragging = flag;
        if (!flag) //Drag ended
        {
            OnDragEndCheckEvents();
        }
    }

    private void Update()
    {
        if (isDragging)
        {
#if UNITY_ANDROID || UNITY_IOS
            List<Touch> touches = InputHelper.GetTouches();
            if (touches.Count == 1)
            {
                if (touches[0].phase == TouchPhase.Moved)
                {
                    Drag(touches[0].position);
                }
            }
#endif
#if UNITY_STANDALONE || UNITY_EDITOR
            Drag(Input.mousePosition);
#endif
        }
        if (Input.mouseScrollDelta.y == 0)
        {
            return;
        }
        zoomPosition = designCamera.localPosition.z + (Input.mouseScrollDelta.y * scrollScale);
        if (zoomPosition < maxZoom && zoomPosition > minZoom)
        {
            designCamera.DOLocalMoveZ(zoomPosition, scrollDuration);
            //Hud.SetDebugText?.Invoke(designCamera.position.z.ToString());
        }
    }

    private void Drag(Vector2 position)
    {
        Vector2 dir = position - oldTouchPosition;
        oldTouchPosition = position;
        transform.Rotate(0, dir.x * dragSpeed, 0);
        CheckRotationAngles();
    }
    #endregion


    #region Rotation check snap system
    private void CheckRotationAngles()
    {
        //Front facing
        OnCameraFacingFrame?.Invoke(transform.eulerAngles.y < maxRotationAngle
            || transform.eulerAngles.y > (360 - maxRotationAngle));

        //Left Facing
        OnCameraFacingLeftTemple?.Invoke(transform.eulerAngles.y < (leftAngle + maxRotationAngle)
            && transform.eulerAngles.y > (leftAngle - maxRotationAngle));
    }

    private void OnDragEndCheckEvents()
    {
        //Front facing
        if (transform.eulerAngles.y < maxRotationAngle || transform.eulerAngles.y > (360 - maxRotationAngle)) // Facing towards camera
        {
            transform.DOLocalRotate(Vector3.zero, snapDuation);
        }
        //Left Facing
        if (transform.eulerAngles.y < (leftAngle + maxRotationAngle) && transform.eulerAngles.y > (leftAngle - maxRotationAngle)) // Facing towards right temple
        {
            transform.DOLocalRotate(new Vector3(0, leftAngle, 0), snapDuation);
        }
    }
    #endregion

}