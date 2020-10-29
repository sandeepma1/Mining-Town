using System.Collections;
using UnityEngine;
using DG.Tweening;

public class HomeCameraManager : MonoBehaviour
{
    private const float spaceSize = 16f;
    private const float animationDuration = 0.5f;
    Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        AdjustCamera();
        // UiNavigationManagerCanvas.OnNavigationButtonsClicked += OnNavigationButtonsClicked;
    }

    private void OnDestroy()
    {
        // UiNavigationManagerCanvas.OnNavigationButtonsClicked -= OnNavigationButtonsClicked;
    }

    //public float elevation;
    //public float cameraDistance = 2.0f;
    private void AdjustCamera()
    {
        Camera.main.orthographicSize = (float)((spaceSize) * Screen.height / Screen.width * 0.5);

        //Bounds bounds = cameraToFitGo.GetComponent<MeshFilter>().mesh.bounds;
        //Vector3 xyz = bounds.size;
        //Vector3 xyztemp = bounds.size;
        //UnityEngine.Debug.Log("xyz:" + xyz.magnitude);
        //if (xyz.magnitude < xyztemp.magnitude)
        //{
        //    xyz = xyztemp;
        //    UnityEngine.Debug.Log(xyz.magnitude);
        //    UnityEngine.Debug.Log(xyztemp.magnitude);
        //}
        ////UnityEngine.Debug.Log (xyz);
        //float distance = Mathf.Max(xyz.x, xyz.y, xyz.z);
        //distance /= (2.0f * Mathf.Tan(0.5f * Camera.main.fieldOfView * Mathf.Deg2Rad));
        //// Move camera in -z-direction; change '2.0f' to your needs
        ////Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -distance * 2.0f);
        //Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, -distance * 2.0f, Camera.main.transform.position.z);

        ////Bounds bounds = cameraToFitGo.GetComponent<MeshFilter>().mesh.bounds;
        ////Vector3 objectSizes = bounds.max - bounds.min;
        ////float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
        ////float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * Camera.main.fieldOfView); // Visible height 1 meter in front
        ////float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
        ////distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object

        ////Camera.main.transform.position = bounds.center - distance * Camera.main.transform.forward;
        ////Camera.main.transform.rotation = Quaternion.Euler(new Vector3(elevation, 0, 0));
        ////Camera.main.transform.position = bounds.center - Camera.main.transform.position.y * Camera.main.transform.forward;
    }

    private void SnapCamera(float positionId)
    {
        transform.DOMoveX(positionId * spaceSize, animationDuration);
    }

    private void OnNavigationButtonsClicked(int buttonId)
    {
        switch (buttonId)
        {
            case 0: //Mines
                SnapCamera(-2);
                break;
            case 1: //Mines
                SnapCamera(-1);
                break;
            case 2://Farm
                SnapCamera(0);
                break;
            case 3: //Forest
                SnapCamera(1);
                break;
            case 4: //Forest
                SnapCamera(2);
                break;
            default:
                break;
        }
    }
}