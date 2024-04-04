using System;
using UnityEngine;

public class GridMover : MonoBehaviour
{
    [SerializeField] private GridTileQuad[] gridTileQuads;
    public Action<bool> OnCollision;
    private bool isColliding;

    public void DetectCollision()
    {
        for (int i = 0; i < gridTileQuads.Length; i++)
        {
            if (gridTileQuads[i].IsColliding)
            {
                isColliding = true;
                break;
            }
            else
            {
                isColliding = false;
            }
        }
        IsColliding(isColliding);
    }

    private void IsColliding(bool flag)
    {
        UiBuildingEditModeCanvas.OnToggleOkButton?.Invoke(!flag);
        OnCollision?.Invoke(flag);
    }
}