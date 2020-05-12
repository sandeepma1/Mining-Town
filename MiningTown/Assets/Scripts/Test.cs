using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform a;
    public Transform b;
    public Transform result;
    public Vector3 dir;
    public float distance = 10;

    void Update()
    {
        dir = (a.transform.position - b.transform.position).normalized;

        result.transform.position = a.transform.position + dir * distance;

        //Debug.DrawLine(a.transform.position, b.transform.position + dir * distance, Color.red, Mathf.Infinity);
    }
}
