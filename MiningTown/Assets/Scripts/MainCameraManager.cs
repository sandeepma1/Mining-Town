using System;
using UnityEngine;

public class MainCameraManager : MonoBehaviour
{
    public static Action<float> OnCameraReadyRotationX; //Use this to set the healthbar rotations to always look at camera
    [SerializeField] private float cameraSmooth = 0.1f;
    [SerializeField] private bool cameraClamp = false;
    [SerializeField] private float minX, maxX = 0;
    [SerializeField] private float minZ, maxZ = 0;
    [SerializeField] private float zOffset = 3;
    private Transform player;
    private const float cameraHeight = 10;

    private void Awake()
    {
        LevelGenerator.OnLevelSizeLoaded += OnLevelSizeLoaded;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = player.transform.position;
    }

    private void OnDestroy()
    {
        LevelGenerator.OnLevelSizeLoaded -= OnLevelSizeLoaded;
    }

    private void OnLevelSizeLoaded(int width, int height)
    {
        Camera.main.orthographicSize = (float)((width + 1) * Screen.height / Screen.width * 0.5);
        OnCameraReadyRotationX?.Invoke(transform.eulerAngles.x);
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.position, cameraSmooth);
        transform.position = new Vector3(0, cameraHeight, player.position.z);

        if (cameraClamp)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), cameraHeight, Mathf.Clamp(transform.position.z + zOffset, minZ, maxZ));
        }
    }
}