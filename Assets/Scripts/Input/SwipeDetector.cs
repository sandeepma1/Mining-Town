using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwipeDetector : MonoBehaviour
{
    public static Action<bool> OnToggleSwipe;
    public static Action OnSwipeLeft;
    public static Action OnSwipeRight;
    [SerializeField] private bool detectSwipeOnlyAfterRelease = false;
    [SerializeField] private float swipeThreshold = 20f;
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    private int lastTabId = -1;
    private bool enableSwipe = true;
    private bool isInBuildMenu;
    private List<Touch> touches;

    private void Awake()
    {
        OnToggleSwipe += OnToggleSwipeEventHandler;
        //UiBuildCanvas.OnToggleBuildMode += OnToggleBuildMode;
        UiNavigationManagerCanvas.OnNavigationButtonsClicked += OnNavigationButtonsClicked;
    }

    private void OnDestroy()
    {
        OnToggleSwipe -= OnToggleSwipeEventHandler;
        //UiBuildCanvas.OnToggleBuildMode -= OnToggleBuildMode;
        UiNavigationManagerCanvas.OnNavigationButtonsClicked -= OnNavigationButtonsClicked;
    }

    private void OnToggleBuildMode(bool inBuildMenu)
    {
        isInBuildMenu = inBuildMenu;
    }

    private void OnToggleSwipeEventHandler(bool isEnable)
    {
        StartCoroutine(ToggleSwipe(isEnable));
    }

    private IEnumerator ToggleSwipe(bool isEnable)
    {
        yield return new WaitForEndOfFrame();
        enableSwipe = isEnable;
    }

    private void Update()
    {
        if (!enableSwipe)
        {
            return;
        }
        touches = InputHelper.GetTouches();
        if (touches.Count != 1)
        {
            return;
        }
        foreach (Touch touch in touches)
        {
            //Refactor it
            if (lastTabId == 0 || lastTabId == 4)
            {
                //continue
            }
            else
            {
                if (ExtensionMethods.IsPointerOverUIObject())
                {
                    return;
                }
            }



            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;
            }

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                if (!detectSwipeOnlyAfterRelease)
                {
                    fingerDown = touch.position;
                    CheckSwipe();
                }
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDown = touch.position;
                CheckSwipe();
            }
        }
    }

    private void OnNavigationButtonsClicked(int tabId)
    {
        lastTabId = tabId;
    }

    private void CheckSwipe()
    {
        //Check if Vertical swipe
        if (VerticalMove() > swipeThreshold && VerticalMove() > HorizontalValMove())
        {
            //Debug.Log("Vertical");
            if (fingerDown.y - fingerUp.y > 0)//up swipe
            {
                SwipeUp();
            }
            else if (fingerDown.y - fingerUp.y < 0)//Down swipe
            {
                SwipeDown();
            }
            fingerUp = fingerDown;
        }

        //Check if Horizontal swipe
        else if (HorizontalValMove() > swipeThreshold && HorizontalValMove() > VerticalMove())
        {
            //Debug.Log("Horizontal");
            if (fingerDown.x - fingerUp.x > 0)//Right swipe
            {
                SwipeRight();
            }
            else if (fingerDown.x - fingerUp.x < 0)//Left swipe
            {
                SwipeLeft();
            }
            fingerUp = fingerDown;
        }

        //No Movement at-all
        else
        {
            //Debug.Log("No Swipe!");
        }
    }

    private float VerticalMove()
    {
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    private float HorizontalValMove()
    {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }

    private void SwipeUp()
    {
        //Debug.Log("Swipe UP");
        //Hud.SetHudText?.Invoke("Swipe UP");
    }

    private void SwipeDown()
    {
        //Debug.Log("Swipe Down");
        //Hud.SetHudText?.Invoke("Swipe Down");
    }

    private void SwipeLeft()
    {
        if (isInBuildMenu && lastTabId >= 3)
        {
            return;
        }
        lastTabId++;
        if (lastTabId >= 4)
        {
            lastTabId = 4;
        }
        //Hud.SetHudText?.Invoke("SwipeLeft " + lastTabId);
        UiNavigationManagerCanvas.OnMoveNavigateTabById?.Invoke(lastTabId);
        MainCanvasManager.OnHideFloatingUi?.Invoke();
    }

    private void SwipeRight()
    {
        if (isInBuildMenu && lastTabId <= 1)
        {
            return;
        }
        lastTabId--;
        if (lastTabId <= 0)
        {
            lastTabId = 0;
        }
        //Hud.SetHudText?.Invoke("SwipeRight " + lastTabId);
        UiNavigationManagerCanvas.OnMoveNavigateTabById?.Invoke(lastTabId);
        MainCanvasManager.OnHideFloatingUi?.Invoke();
    }
}