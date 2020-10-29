using System;
using System.Collections.Generic;
using UnityEngine;

public class ClosestObject : MonoBehaviour
{
    public static Action<Renderer> AddToRederer;
    [SerializeField] private float searchDistance = 3;
    [SerializeField] private bool colorInvisibleObjects = false;
    private List<Renderer> objects = new List<Renderer>();
    private CullingGroup cullGroup = null;
    private List<BoundingSphere> bounds = new List<BoundingSphere>();

    private void Awake()
    {
        AddToRederer += AddObjectRenderer;
        MiningLevelGenerator.OnLevelGenerated += OnLevelGenerated;
        CreateCullGroup();
    }

    private void OnDestroy()
    {
        cullGroup.onStateChanged -= StateChanged;
        AddToRederer -= AddObjectRenderer;
        cullGroup.Dispose();
        cullGroup = null;
    }

    private void CreateCullGroup()
    {
        cullGroup = new CullingGroup();
        cullGroup.targetCamera = Camera.main;
        cullGroup.SetDistanceReferencePoint(transform);
        cullGroup.SetBoundingDistances(new float[] { searchDistance, float.PositiveInfinity });
    }

    private void OnLevelGenerated()
    {
        if (cullGroup == null)
        {
            CreateCullGroup();
        }
        cullGroup.SetBoundingSpheres(bounds.ToArray());
        cullGroup.SetBoundingSphereCount(objects.Count);
        cullGroup.onStateChanged += StateChanged;
    }

    public void AddObjectRenderer(Renderer renderer)
    {
        objects.Add(renderer);
        BoundingSphere b = new BoundingSphere();
        b.position = renderer.transform.position;
        b.radius = renderer.GetComponent<MeshFilter>().mesh.bounds.extents.x;
        bounds.Add(b);
    }

    private void StateChanged(CullingGroupEvent e)
    {
        if (colorInvisibleObjects == true && e.isVisible == false)
        {
            objects[e.index].material.color = Color.gray;
            return;
        }

        // if we are in distance band index 0, that is between 0 to searchDistance
        if (e.currentDistance == 0)
        {
            objects[e.index].material.color = Color.green;
        }
        else // too far, set color to red
        {
            objects[e.index].material.color = Color.red;
        }
    }
}
