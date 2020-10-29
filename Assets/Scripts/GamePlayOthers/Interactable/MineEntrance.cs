using UnityEngine;

public class MineEntrance : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            UiStartMinesCanvas.OnShowStartMinesMenu?.Invoke();
        }
    }
}
