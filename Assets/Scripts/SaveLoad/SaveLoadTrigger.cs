using System;
using MiningTown.IO;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SaveLoadTrigger : MonoBehaviour
{
    public static Action OnSaveTrigger;

    private void Awake()
    {
        if (SaveLoadManager.saveData == null)
        {
            ItemDatabase.LoadItemDatabase();
            MineralsDatabase.LoadItemDatabase();
            ForestObjectsDatabase.LoadItemDatabase();
            FishDatabase.LoadItemDatabase();
            MonsterDatabase.LoadItemDatabase();
            MiningLevelsDatabase.LoadItemDatabase();
            ForestLevelsDatabase.LoadItemDatabase();
            SourceDatabase.LoadDatabase();
            DecorationDatabase.LoadDatabase();
            ItemReceipesDatabase.LoadDatabase();
            CropsDatabase.LoadDatabase();
            SaveLoadManager.LoadOrCreateNewGame();
        }
        //SceneLoader.OnLevelLoading?.Invoke(SceneManager.GetActiveScene().name);
        SceneLoader.OnSceneChanged += OnLevelLoading;
    }

    private void OnDestroy()
    {
        SceneLoader.OnSceneChanged -= OnLevelLoading;
    }


    #region Save Game on Exit or pause
#if UNITY_EDITOR || UNITY_STANDALONE
    private void OnApplicationQuit()
    {
        TriggerSave();
    }
#endif

#if UNITY_ANDROID
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            TriggerSave();
        }
    }
#endif

    private void OnLevelLoading(Scenes levelName)
    {
        TriggerSave();
    }

    private void TriggerSave()
    {
        OnSaveTrigger?.Invoke();
    }
    #endregion
}