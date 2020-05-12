using System;
using System.Collections.Generic;
using UnityEngine;

public class BreakableManager : MonoBehaviour
{
    private List<BreakableBase> breakables = new List<BreakableBase>();

    public void AddBreakable(BreakableBase breakable, int xPos, int yPos)
    {
        breakable.transform.localPosition = new Vector3(xPos, 0, yPos);
        breakable.breakableMesh.transform.localEulerAngles = new Vector3(0, UnityEngine.Random.Range(0, 180), 0);
        //ClosestObject.AddToRederer?.Invoke(breakable.breakableMesh);
        breakables.Add(breakable);
        breakable.OnBreakableDied += BreakableDied;
    }

    public void BreakableDied(BreakableBase breakableBase)
    {
        SelectionCircle.SetToThisParent?.Invoke(null);
        breakableBase.OnBreakableDied -= BreakableDied;
        breakables.Remove(breakableBase);
        //LevelGenerator.OnBakeNavMesh?.Invoke();
    }

    public BreakableBase GetNearestBreakableFromPosition(Vector3 currentPos)
    {
        BreakableBase nearestBreakable = null;
        float minDist = Mathf.Infinity;
        for (int i = 0; i < breakables.Count; i++)
        {
            float dist = Vector3.Distance(breakables[i].transform.position, currentPos);
            if (dist < minDist)
            {
                nearestBreakable = breakables[i];
                minDist = dist;
            }
        }
        return nearestBreakable;
    }
}