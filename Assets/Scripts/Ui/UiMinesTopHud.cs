using UnityEngine;
using TMPro;

public class UiMinesTopHud : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelNumberText;

    private void Start()
    {
        levelNumberText.text = "Level " + SaveLoadManager.saveData.playerStats.currentMinesLevel;
    }
}