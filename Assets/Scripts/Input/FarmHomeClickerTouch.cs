using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class FarmHomeClickerTouch : MonoBehaviour
{
    public static Action<bool> OnToggleClick;
    private Camera mainCamera;
    private RaycastHit hitInfo = new RaycastHit();
    private bool isClickable = true;

    private void Start()
    {
        OnToggleClick += OnToggleClickEventHandler;
        mainCamera = Camera.main;
    }

    private void OnDestroy()
    {
        OnToggleClick -= OnToggleClickEventHandler;
    }

    private void OnToggleClickEventHandler(bool isClick)
    {
        isClickable = isClick;
    }

    private void Update()
    {
        //Hud.SetHudText?.Invoke(isClickable + " FarmHomeClickerTouch");
        //if (!isClickable)
        //{
        //    return;
        //}
        //if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        //{
        //    //print(isClickable + " FarmHomeClickerTouch");
        //    //UiTimeRemainingCanvas.Instance.HideCanvas();
        //    //if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo))
        //    //{
        //    //    if (hitInfo.collider.gameObject.CompareTag("FarmClickable"))
        //    //    {
        //    //        //Debug.Log("Farm bed hit");
        //    //        hitInfo.collider.gameObject.GetComponent<IFarmIneractable>().InteractOnClick();
        //    //    }
        //    //}
        //}
    }
}