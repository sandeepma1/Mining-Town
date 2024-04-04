using UnityEngine;

public class ForestEntrance : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            UiStartForestCanvas.OnShowStartForestMenu?.Invoke();
        }
    }
}