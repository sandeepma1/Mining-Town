using UnityEngine;

public class DragCamera : MonoBehaviour
{
    private bool bDragging;
    private Vector3 oldPos;
    private Vector3 panOrigin;
    private Vector3 dragPosition;
    private Vector3 xPosition;
    public float panSpeed = 4;
    public float zSpeed = 4;
    public float zPos = 4;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bDragging = true;
            oldPos = transform.position;
            panOrigin = mainCamera.ScreenToViewportPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 pos = mainCamera.ScreenToViewportPoint(Input.mousePosition) - panOrigin;
            xPosition = oldPos + -pos * panSpeed;


            dragPosition = pos - panOrigin;

            Hud.SetHudText?.Invoke(dragPosition + "");
            if (dragPosition.y == 0)
            {
                return;
            }
            transform.position = new Vector3(xPosition.x, transform.position.y, transform.position.z + dragPosition.y);
        }

        if (Input.GetMouseButtonUp(0))
        {
            bDragging = false;
        }
    }
}
