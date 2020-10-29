using UnityEngine;

public class GridTileQuad : MonoBehaviour
{
    private Renderer gridRenderer;
    private bool isColliding;
    public bool IsColliding
    {
        get
        {
            return isColliding;
        }
        private set
        {
            isColliding = value;
            gridRenderer.material.color = isColliding ? ColorConstants.RedTile : ColorConstants.GreenTile;
        }
    }

    private void Start()
    {
        gridRenderer = GetComponent<Renderer>();
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    IsColliding = true;
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    IsColliding = false;
    //}

    private void OnCollisionStay(Collision collision)
    {
        IsColliding = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        IsColliding = false;
    }
}
