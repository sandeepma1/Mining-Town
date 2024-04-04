using System;
using System.Collections.Generic;
using UnityEngine;

public class MovableUnitBase : MonoBehaviour
{
    private Vector3 initialPosition;
    private const string gridMoverPath = "FarmHome/GridMover/GridTiles";
    private GridMover gridMover;
    protected BoxCollider boxCollider;
    private Camera mainCamera;
    private bool isInEditMode;
    private bool isColliding;
    protected bool isInBuildMenu;
    private List<Touch> touches = new List<Touch>();
    private bool isAnyBuildSelected;
    private Vector3 normalColliderSize;
    private Vector3 editModeColliderSize;
    private float offsetX;
    private float offsetZ;

    protected virtual void Awake()
    {
        initialPosition = transform.position;
        boxCollider = GetComponent<BoxCollider>();
        normalColliderSize = boxCollider.size;
        editModeColliderSize = new Vector3(boxCollider.size.x, 5, boxCollider.size.z);
        UiBuildingEditModeCanvas.OnEditOkButtonClick += OnEditOkButtonClick;
        UiBuildingEditModeCanvas.OnEditCancelButtonClick += OnEditCancelButtonClick;
        UiBuildingEditModeCanvas.OnToggleButtonsPanel += OnToggleButtonsPanel;
        mainCamera = Camera.main;
    }

    protected virtual void OnDestroy()
    {
        UiBuildingEditModeCanvas.OnEditOkButtonClick -= OnEditOkButtonClick;
        UiBuildingEditModeCanvas.OnEditCancelButtonClick -= OnEditCancelButtonClick;
        UiBuildingEditModeCanvas.OnToggleButtonsPanel -= OnToggleButtonsPanel;
    }

    private void OnToggleButtonsPanel(bool isBuildSelected)
    {
        isAnyBuildSelected = isBuildSelected;
    }

    private void OnMouseUpAsButton()
    {
        if (ExtensionMethods.IsPointerOverUIObject() || isInEditMode || isAnyBuildSelected)
        {
            return;
        }
        EnableEditMode();
    }

    private void OnMouseDrag()
    {
        if (!isInEditMode || ExtensionMethods.IsPointerOverUIObject())
        {
            return;
        }
        touches = InputHelper.GetTouches();
        if (touches.Count != 1)
        {
            return;
        }
        float distance_to_screen = mainCamera.WorldToScreenPoint(gameObject.transform.position).z;
        Vector3 pos_move = mainCamera.ScreenToWorldPoint(new Vector3(touches[0].position.x, touches[0].position.y, distance_to_screen));

        transform.position = new Vector3(Mathf.RoundToInt(pos_move.x) + offsetX, transform.position.y, Mathf.RoundToInt(pos_move.z) + offsetZ);

        DetectEdge(touches[0].position);
        gridMover.DetectCollision();
    }

    private void DetectEdge(Vector2 pos)
    {
        if (!GameVariables.edgeScreenRect.Contains(pos))
        {
            PlayerFollowCameraManager.OnTouchEdge?.Invoke(pos);
        }
    }

    private void IsColliding(bool colliding)
    {
        isColliding = colliding;
    }

    protected void EnableEditMode()
    {
        isInEditMode = true;
        UiBuildingEditModeCanvas.OnToggleButtonsPanel?.Invoke(isInEditMode);
        boxCollider.isTrigger = true;
        initialPosition = transform.position;

        //Instantiate Grid mover 
        string gridSizeName = boxCollider.size.x + "x" + boxCollider.size.z;
        gridMover = Instantiate(Resources.Load<GridMover>(gridMoverPath + gridSizeName) as GridMover, transform);
        gridMover.transform.localPosition = Vector3.zero;
        gridMover.OnCollision += IsColliding;

        //boxCollider increase size
        boxCollider.size = editModeColliderSize;

        if (normalColliderSize.x % 2 == 0)
        {
            offsetX = 0.5f;
        }
        else
        {
            offsetX = 0;
        }
        if (normalColliderSize.z % 2 == 0)
        {
            offsetZ = 0.5f;
        }
        else
        {
            offsetZ = 0;
        }
    }

    private void DisabledEditMode()
    {
        if (!isInEditMode)
        {
            return;
        }
        isInEditMode = false;
        UiBuildingEditModeCanvas.OnToggleButtonsPanel?.Invoke(isInEditMode);
        boxCollider.isTrigger = false;
        if (gridMover != null)
        {
            gridMover.OnCollision -= IsColliding;
            Destroy(gridMover.gameObject);
        }
        SavePositionData();
        //boxCollider decrease size
        boxCollider.size = normalColliderSize;
    }

    protected virtual void SavePositionData()
    {
        //This will be empty, a trigger to save building data
    }

    //private void OnToggleBuildMode(bool inBuildMode)
    //{
    //    isInBuildMenu = inBuildMode;
    //    //boxCollider.isTrigger = inBuildMode;
    //}


    #region UI Edit Buttons
    private void OnEditCancelButtonClick(bool destroyBuilding)
    {
        if (destroyBuilding && isInBuildMenu)
        {
            Destroy(this.gameObject);
            return;
        }
        isInBuildMenu = false;
        if (!isInEditMode)
        {
            return;
        }
        transform.position = initialPosition;
        DisabledEditMode();
    }

    private void OnEditOkButtonClick()
    {
        if (isInBuildMenu)
        {
            AddThisNewBuilding();
        }
        if (!isInEditMode)
        {
            return;
        }
        DisabledEditMode();
        if (isColliding)
        {
            print("isColliding, should never print");
            transform.position = initialPosition;
        }
        else
        {
            isColliding = false;
        }
    }

    protected virtual void AddThisNewBuilding()
    {
        //This will be empty
    }
    #endregion
}