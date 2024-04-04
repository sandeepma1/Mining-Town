using System;
using UnityEngine;

public class SelectionCircle : MonoBehaviour
{
    public static Action<Transform> OnSetToThisParent;
    [SerializeField] private Transform transforToRotate;
    private float rotationsPerMinute = 100;
    private Vector3 resetPosition = new Vector3(0, 0.01f, 0);

    private void Awake()
    {
        OnSetToThisParent += SetToThisParent;
    }

    private void OnDestroy()
    {
        OnSetToThisParent -= SetToThisParent;
    }

    private void SetToThisParent(Transform parent)
    {
        if (parent == null)
        {
            transforToRotate.gameObject.SetActive(false);
        }
        else
        {
            transforToRotate.gameObject.SetActive(true);
            transform.position = parent.position;
        }
    }

    private void LateUpdate()
    {
        if (transforToRotate.gameObject.activeSelf)
        {
            transforToRotate.Rotate(0, 0, rotationsPerMinute * Time.deltaTime);
        }
    }
}