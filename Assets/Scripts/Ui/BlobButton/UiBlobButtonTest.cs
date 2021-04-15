using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiBlobButtonTest : MonoBehaviour
{
    public Button button;

    private void Start()
    {
        button.onClick.AddListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        print("button");
    }
}
