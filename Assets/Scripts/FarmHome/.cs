using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditModeColliders : MonoBehaviour
{
    [SerializeField] private Transform colliders;
    [SerializeField] private Transform colliders;

    private void Start()
    {
        foreach (Transform child in colliders)
        {
            child.GetComponent<Renderer>().enabled = false;//.gameObject.SetActive(false);
        }
    }
}
