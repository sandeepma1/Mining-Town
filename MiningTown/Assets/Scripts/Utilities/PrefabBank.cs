using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabBank : MonoBehaviour
{
    public static PrefabBank Instance = null;
    [SerializeField] private HealthBar healthBarPrefab;
    private Camera mainCamera;
    private MainCameraManager mainCameraManager;

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
        mainCameraManager = mainCamera.GetComponent<MainCameraManager>();
    }

    public HealthBar GetHealthBarPrefab()
    {
        return healthBarPrefab;
    }

    public Camera GetMainCamera()
    {
        return mainCamera;
    }

    public MainCameraManager GetMainCameraManager()
    {
        return mainCameraManager;
    }
}
