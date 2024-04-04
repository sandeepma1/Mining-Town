using System.Collections.Generic;
using UnityEngine;

public class BreakableManager : MonoBehaviour
{
    [SerializeField] private Ladder ladder;
    private List<BreakableBase> breakables = new List<BreakableBase>();
    private int ladderSpawnCutoff = 0;
    private bool isLadderMarked = false;

    public void AddBreakable(BreakableBase breakable)
    {
        breakables.Add(breakable);
        breakable.OnBreakableDied += BreakableDied;
    }

    public void CalculaterLadderSpawn(float min, float max)
    {
        float ladderSpawnCutoffPercent = UnityEngine.Random.Range(min, max);
        ladderSpawnCutoff = (int)(breakables.Count - (breakables.Count * ladderSpawnCutoffPercent));
    }

    public void BreakableDied(BreakableBase breakableBase, bool isLadder)
    {
        SelectionCircle.OnSetToThisParent?.Invoke(null);
        breakableBase.OnBreakableDied -= BreakableDied;
        if (isLadder)
        {
            GenerateLadder(breakableBase.transform.localPosition);
        }
        breakables.Remove(breakableBase);
        //LevelGenerator.OnBakeNavMesh?.Invoke();
        if (breakables.Count <= ladderSpawnCutoff && !isLadderMarked)
        {
            isLadderMarked = true;
            MarkLadder();
        }
    }

    private void GenerateLadder(Vector3 position)
    {
        ladder.transform.localPosition = position;
    }

    private void MarkLadder()
    {
        int ladderIndex = UnityEngine.Random.Range(0, breakables.Count);
        breakables[ladderIndex].isLadder = true;
        //breakables[ladderIndex].breakableMesh.GetComponent<Renderer>().material.color = Color.green;
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