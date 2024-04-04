using UnityEngine;

public class ToonWater : MonoBehaviour
{
    private void OnValidate()
    {
        //SetCameraDepthTextureMode();
    }

    private void Awake()
    {
        SetCameraDepthTextureMode();
    }

    private void SetCameraDepthTextureMode()
    {
        //unable to build
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }
}