using System;
using UnityEngine;

public class SelectionCircle : MonoBehaviour
{
    public static Action<Transform> SetToThisParent;
    [SerializeField] private Transform transforToRotate;
    private float rotationsPerMinute = 100;
    private bool hasParent;
    private void Awake()
    {
        SetToThisParent += SetToThisParentEH;
    }

    private void OnDestroy()
    {
        SetToThisParent -= SetToThisParentEH;
    }

    private void SetToThisParentEH(Transform parent)
    {
        if (parent == null)
        {
            hasParent = false;
            transform.SetParent(null);
            transforToRotate.gameObject.SetActive(false);
        }
        else
        {
            hasParent = true;
            transforToRotate.gameObject.SetActive(true);
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
        }
    }

    private void LateUpdate()
    {
        if (hasParent)
        {
            transforToRotate.Rotate(0, 0, rotationsPerMinute * Time.deltaTime);
        }
    }
}