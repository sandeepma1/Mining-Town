using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainScene : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1);
        SaveLoadManager.saveData.playerStats.playerPosition = GameVariables.playerHomePosition;
        SceneLoader.LoadLevelByName(this, Scenes.FarmHome);
        //SceneManager.LoadSceneAsync(this, levelName, LoadSceneMode.Additive);
    }
}