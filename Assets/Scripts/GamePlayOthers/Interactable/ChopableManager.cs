using System.Collections.Generic;
using UnityEngine;

public class ChopableManager : MonoBehaviour
{
    private List<ChopableBase> chopableBases = new List<ChopableBase>();

    public void AddChopable(ChopableBase chopableBase)
    {
        chopableBases.Add(chopableBase);
        chopableBase.OnChopableDied += OnChopableDied;
    }

    public void OnChopableDied(ChopableBase chopableBase)
    {
        SelectionCircle.OnSetToThisParent?.Invoke(null);
        chopableBase.OnChopableDied -= OnChopableDied;
        chopableBases.Remove(chopableBase);
    }
}