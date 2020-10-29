using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        InvokeRepeating("Shake", 0, 2);
    }

    private void Shake()
    {
        transform.DOPunchRotation(new Vector3(0, 50, 0), 0.5f);
    }
}
