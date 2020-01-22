using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    private static Text debugText;

    private void Awake()
    {
        debugText = transform.GetComponentInChildren<Text>();
        PrintDebugText(".");
    }

    public static void PrintDebugText(string text)
    {
        debugText.text = text;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            SceneManager.LoadScene(0);
        }
    }
}