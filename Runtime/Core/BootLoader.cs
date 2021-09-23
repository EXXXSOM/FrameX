using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

//Отвечает за загрузку, подгрузку и выгрузку сцен
public class BootLoader : MonoBehaviour
{
    private static string _nextSceneName;
    public static string NextSceneName => _nextSceneName;

    // Start is called before the first frame update
    private List<AsyncOperation> _loadJobs = new List<AsyncOperation>();
    public static event Action<float> OnSceneLoading = delegate { };

    private void Update()
    {
        HandleLoadingProgress();
    }

    //Выгружает все загруженные коллекции сцен и загружает требуемую
    public void LoadSceneCollection(string sceneName)
    {
        _nextSceneName = sceneName;

        AddSceneCollection(sceneName);
    }

    //К активным коллекциям сцен добавляет новую
    public void AddSceneCollection(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = false;
        _loadJobs.Add(asyncOperation);
    }

    public void UnloadSceneCollection(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        _loadJobs.Add(asyncOperation);
    }

    private void HandleLoadingProgress()
    {
        if (_loadJobs.Count <= 0) return;

        float progress = 0f;
        for (int i = 0; i < _loadJobs.Count; i++)
        {
            progress += _loadJobs[i].progress;
        }

        float ratio = progress / _loadJobs.Count;

        OnSceneLoading?.Invoke(ratio);

        if (ratio >= 0.9f)
        {
            for (int i = 0; i < _loadJobs.Count; i++)
            {
                _loadJobs[i].allowSceneActivation = true;
            }
        }
    }
}
