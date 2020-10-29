using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTiki : TrapsBase
{
    //WIP
    [SerializeField] private Transform spikesParent;
    private Vector3 openRight;

    private void OpenSkipes()
    {
        spikesParent.position = openRight;
    }

}