using UnityEngine;

public class Warper : MonoBehaviour
{
    [SerializeField] private Scenes sceneToLoad;
    [SerializeField] private Vector3 positionToStart;

    private void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            WarpTo();
        }
    }

    private void WarpTo()
    {
        SaveLoadManager.saveData.playerStats.playerPosition = positionToStart;
        SceneLoader.LoadLevelByName(this, sceneToLoad);
    }
}