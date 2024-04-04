using System;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class GrassManager : MonoBehaviour
{
    public static Action<Transform> OnAddGrass;
    public static Action<Vector3Int> OnStepOnGrass;
    private Dictionary<Vector3Int, Transform> grasses = new Dictionary<Vector3Int, Transform>();
    public Vector3 punch = new Vector3(0, 0, 25);
    public float duration = 3;
    public int vibrate = 4;
    public float elasticty = 1;

    private void Awake()
    {
        SceneLoader.OnSceneChanged += OnSceneChanged;
    }

    private void Start()
    {
        OnStepOnGrass += StepOnGrass;
        OnAddGrass += AddGrass;
        grasses = new Dictionary<Vector3Int, Transform>();
    }

    private void OnDestroy()
    {
        SceneLoader.OnSceneChanged -= OnSceneChanged;
        OnStepOnGrass -= StepOnGrass;
        OnAddGrass -= AddGrass;
    }

    private void OnSceneChanged(Scenes scene)
    {
        grasses = new Dictionary<Vector3Int, Transform>();
    }

    private void AddGrass(Transform grassTransform)
    {
        Vector3Int position = new Vector3Int((int)grassTransform.position.x, (int)grassTransform.position.y, (int)grassTransform.position.z);

        if (!grasses.ContainsKey(position))
        {
            grasses.Add(position, grassTransform);
        }
    }

    private void StepOnGrass(Vector3Int pos)
    {
        if (grasses.ContainsKey(pos))
        {
            grasses[pos].DOKill();
            grasses[pos].DOPunchRotation(punch, duration, vibrate, elasticty);
        }
    }
}