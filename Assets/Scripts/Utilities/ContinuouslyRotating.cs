using System.Collections;
using UnityEngine;

public class ContinuouslyRotating : MonoBehaviour
{
    private float rotationsPerMinute = 100;
    private void LateUpdate()
    {
        transform.Rotate(0, 0, rotationsPerMinute * Time.deltaTime);
    }
}