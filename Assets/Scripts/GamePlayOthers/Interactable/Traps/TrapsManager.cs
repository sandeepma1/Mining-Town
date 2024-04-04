using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapsManager : MonoBehaviour
{
    private List<TrapsBase> breakables = new List<TrapsBase>();

    public void AddTrap(TrapsBase traps, int xPos, int yPos)
    {
        traps.transform.localPosition = new Vector3(xPos, traps.transform.localPosition.y, yPos);
        //ClosestObject.AddToRederer?.Invoke(breakable.breakableMesh);
        breakables.Add(traps);
        //traps.OnBreakableDied += BreakableDied;
    }

    //public void BreakableDied(TrapsBase traps)
    //{
    //    SelectionCircle.SetToThisParent?.Invoke(null);
    //    traps.OnBreakableDied -= BreakableDied;
    //    breakables.Remove(traps);
    //    //LevelGenerator.OnBakeNavMesh?.Invoke();
    //}
}