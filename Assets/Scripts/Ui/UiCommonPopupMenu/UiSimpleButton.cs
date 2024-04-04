using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiSimpleButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    public void ChangeButtonText(string textToChange)
    {
        text.text = textToChange;
    }

    public void RemoveListner()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
        button.onClick.RemoveAllListeners();
    }

    public void AddListener(UnityAction unityAction)
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
        button.onClick.AddListener(unityAction);
    }
}