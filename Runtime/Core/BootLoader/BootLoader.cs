using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    enum State
    {
        InvalidSceneName,
        NotAdded,
        IsLoading,
        IsLoaded
    }

    private const string RESOURCES_FOLDER = "SceneCollections";
    private static string _nextActiveSceneName;

    //Количество ссыланий на сцену
    //int = sceneHashCode, int = количество ссылок на сцену
    private static Dictionary<string, SceneData> _sceneReferences = new Dictionary<string, SceneData>(5);
    private static List<SceneCollectionObject> _loadedSceneCollections = new List<SceneCollectionObject>(3);
    private static List<AsyncOperation> _loadJobs = new List<AsyncOperation>();

    public static Action OnStartLoading;
    public static Action OnEndLoading;
    //public static Action<string> OnSceneUnload = delegate { };
    public static Action<float> OnSceneLoading = delegate { };
    public static string NextActiveSceneName => _nextActiveSceneName;

    private static Scene _bootScene;

    private void Update()
    {
        HandleLoadingProgress();
    }

    #region LOAD_SCENE_COLLECTION
    public static void LoadSceneCollection(SceneCollectionObject sceneCollector)
    {
        UnloadAllCollectionsExcept();
        AddSceneCollection(sceneCollector);
    }

    public static void LoadSceneCollection(SceneCollectionObject sceneCollector, Scene bootScene)
    {
        _bootScene = bootScene;
        OnEndLoading += UnloadBootScene;

        LoadSceneCollection(sceneCollector);
    }

    public static void LoadSceneCollection(string nameSceneCollection)
    {
        SceneCollectionObject sceneCollectionObject = GetSceneCollection(nameSceneCollection);

        if (sceneCollectionObject != null)
        {
            LoadSceneCollection(sceneCollectionObject);
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogError("[BootLoader]: Нужная SceneCollectionObject не найдена!");
        }
#endif
    }

    public static void LoadSceneCollectionWithSaveRepeatingScenes(SceneCollectionObject sceneCollector)
    {
        SaveLoadedScene(sceneCollector);
        UnloadAllCollectionsExcept();
        AddSceneCollection(sceneCollector);
    }
    #endregion

    private static void UnloadAllCollectionsExcept()
    {
        for (int i = 0; i < _loadedSceneCollections.Count; i++)
        {
            UnloadSceneCollector(_loadedSceneCollections[i]);
        }

        UnloadUnusedAssets();
    }

    public static void AddSceneCollection(SceneCollectionObject sceneCollector)
    {
        if (_loadedSceneCollections.Count == 0)
        {
            OnStartLoading?.Invoke();
            _nextActiveSceneName = sceneCollector.MainSceneName;
        }

        //Загружает сцены требуемой коллекции
        for (int i = 0; i < sceneCollector.scenes.Length; i++)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(sceneCollector.scenes[i].ScenePath);
            Add(sceneName);
        }

        _loadedSceneCollections.Add(sceneCollector);
    }

    public static void AddSceneCollection(string nameSceneCollection)
    {
        SceneCollectionObject sceneCollectionObject = GetSceneCollection(nameSceneCollection);

        if (sceneCollectionObject != null)
        {
            AddSceneCollection(sceneCollectionObject);
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogError("[BootLoader]: Нужная SceneCollectionObject не найдена!");
        }
#endif
    }

    public static void UnloadSceneCollector(SceneCollectionObject sceneCollector)
    {
        for (int i = 0; i < sceneCollector.scenes.Length; i++)
        {
            string sceneMane = System.IO.Path.GetFileNameWithoutExtension(sceneCollector.scenes[i].ScenePath);
            Remove(sceneMane);
        }
        _loadedSceneCollections.Remove(sceneCollector);

        if (_loadedSceneCollections.Count <= 0)
            _nextActiveSceneName = null;
    }

    private static void Remove(string sceneName)
    {
        if (_sceneReferences.ContainsKey(sceneName))
        {
            if (_sceneReferences[sceneName].CountReference <= 1)
            {
                if (_sceneReferences[sceneName].DontUnload == false)
                {
                    //OnSceneUnload.Invoke(sceneName);
                    AsyncOperation removingScene = SceneManager.UnloadSceneAsync(sceneName);
                    removingScene.allowSceneActivation = false;
                    _loadJobs.Add(removingScene);
                    _sceneReferences.Remove(sceneName);
                }
                else
                {
                    _sceneReferences[sceneName].DontUnload = false;
                }
            }
            else
            {
                _sceneReferences[sceneName].CountReference--;
            }
        }
    }

    private static void Add(string sceneName)
    {
        if (_sceneReferences.ContainsKey(sceneName))
        {
            if (_sceneReferences[sceneName].CountReference > 0)
            {
                _sceneReferences[sceneName].CountReference++;
            }
        }
        else
        {
            var state = CheckSceneState(sceneName);
            if (state == State.NotAdded)
            {
                AsyncOperation loadingScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                loadingScene.allowSceneActivation = false;
                _loadJobs.Add(loadingScene);
                _sceneReferences.Add(sceneName, new SceneData(1));
            }
        }
    }

    private static void HandleLoadingProgress()
    {
        if (_loadJobs.Count <= 0) return;

        float progress = 0f;
        for (int i = 0; i < _loadJobs.Count; i++)
        {
            //Debug.Log(_loadJobs[i].progress + " / " + i);
            progress += _loadJobs[i].progress;
        }

        float ratio = (float)Math.Round(progress, 3) / _loadJobs.Count;

        OnSceneLoading(ratio);

        //Debug.Log("Ratio: " + ratio + " / Count: " + _loadJobs.Count);
        if (ratio >= 0.9f)
        {
            //Debug.Log("Zashel");
            for (var i = 0; i < _loadJobs.Count; i++)
            {
                _loadJobs[i].allowSceneActivation = true;
            }
        }

        if (ratio == 1)
        {
            _loadJobs.Clear();
            OnEndLoading?.Invoke();
        }
    }

    private static void UnloadUnusedAssets()
    {
        AsyncOperation unloadUnusedAssetsOperation = Resources.UnloadUnusedAssets();
        unloadUnusedAssetsOperation.allowSceneActivation = false;
        _loadJobs.Add(unloadUnusedAssetsOperation);
    }

    private static void UnloadBootScene()
    {
        if (_bootScene != default)
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(_bootScene);
            asyncOperation.allowSceneActivation = false;
            _bootScene = default;
        }
        OnEndLoading -= UnloadBootScene;
    }

    private static void SaveLoadedScene(SceneCollectionObject sceneCollector)
    {
        for (int i = 0; i < sceneCollector.scenes.Length; i++)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(sceneCollector.scenes[i].ScenePath);
            if (_sceneReferences.ContainsKey(sceneName))
            {
                _sceneReferences[sceneName].DontUnload = true;
            }
        }
    }

    private static State CheckSceneState(string sceneName)
    {
        if (String.IsNullOrEmpty(sceneName))
            return State.InvalidSceneName;

        if (SceneManager.GetSceneByName(sceneName).isLoaded)
            return State.IsLoaded;

        if (SceneManager.GetSceneByName(sceneName).name != default)
            return State.IsLoading;
        else
            return State.NotAdded;
    }

    public static SceneCollectionObject GetSceneCollection(string nameSceneCollection)
    {
        return Resources.Load<SceneCollectionObject>(RESOURCES_FOLDER + "/" + nameSceneCollection);
    }

    public static string[] GetLoadedSceneCollectionNames()
    {
        string[] sceneColectionNames = new string[_loadedSceneCollections.Count];

        for (int i = 0; i < _loadedSceneCollections.Count; i++)
        {
            sceneColectionNames[i] = _loadedSceneCollections[i].name;
        }

        return sceneColectionNames;
    }

    private class SceneData
    {
        public bool DontUnload;
        public int CountReference;

        public SceneData(int countReference)
        {
            CountReference = countReference;
            DontUnload = false;
        }
    }
}
