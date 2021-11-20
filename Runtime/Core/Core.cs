using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class Core : MonoBehaviour
{
    private const string CORE_SCENE_NAME = "CORE";
    private const string CORE_CONFIG_NAME = "CoreSetupConfig";
    private const string APPLICATION_SETTINGS_NAME = "ApplicationSettings";

    private static bool _applicationIsFocus = false;
    private static bool _applicationIsPaused = false;
    private static bool _applicationIsQuitting = false;
    private static bool _initialized = false;

    public static readonly DependencyContainer Container = new DependencyContainer(5);
    public static readonly ProcessorUpdate Updater = new ProcessorUpdate();

    public static bool ApplicationIsFocus => _applicationIsFocus;
    public static bool ApplicationIsPaused => _applicationIsPaused;
    public static bool ApplicationIsQuitting => _applicationIsQuitting;

    public event Action OnApplicationQuitting;

    private GUIStyle guiStyle = new GUIStyle();
    void OnGUI()
    {
        guiStyle.fontSize = 60;

        float fps = 1.0f / Time.deltaTime;
        GUILayout.Label(fps.ToString(), guiStyle);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Bootstrap()
    {
        if (_initialized) return;
        SetupCore();
        SetupApplicationSettings();

        //Настройка ядра
        void SetupCore()
        {
            //Создание сцены и установка ядра
            var scene = SceneManager.CreateScene(CORE_SCENE_NAME, new CreateSceneParameters(LocalPhysicsMode.None));
            var coreOG = new GameObject("Core");
            var coreScript = coreOG.AddComponent<Core>();

            //Установка загрузчика сцен
            var bootLoaderGO = new GameObject("BootLoader");
            var bootLoaderScript = bootLoaderGO.AddComponent<BootLoader>();

            var coreInstaller = LoadCoreConfig(coreOG.transform);
            coreInstaller.Install(Container);

            SetupPlugins(coreInstaller.gameObject);

            SceneManager.MoveGameObjectToScene(coreOG, scene);
            SceneManager.MoveGameObjectToScene(bootLoaderGO, scene);

            _initialized = true;
            Debug.Log("[CORE]: Started!");
        }
    }

    private static CoreInstaller LoadCoreConfig(Transform loadContainer)
    {
        CoreInstaller coreInstaller = LoadResources<CoreInstaller>(CORE_CONFIG_NAME);

        if (coreInstaller == null)
        {
            Debug.LogError("[CORE]: CoreInstaller отсутствует!");
        }
        coreInstaller = Instantiate(coreInstaller, loadContainer);
        return coreInstaller;
    }

    private static void SetupPlugins(GameObject corePrefab)
    {
        //
    }

    private static void SetupApplicationSettings()
    {
        ApplicationSettings applicationSettings = LoadResources<ApplicationSettings>(APPLICATION_SETTINGS_NAME);

        QualitySettings.vSyncCount = applicationSettings.vSyncCount;
        Application.targetFrameRate = applicationSettings.TargetFrameRate;
        Debug.Log("vSyncCount: " + QualitySettings.vSyncCount);
        Debug.Log("frameRateLimit: " + Application.targetFrameRate);
    }

    private void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
        OnApplicationQuitting?.Invoke();
    }

    private void OnApplicationPause(bool pause)
    {
        //Если игра скрыта или частично перекрыта другим приложением, то вернет true
        _applicationIsPaused = pause;
    }

    private void OnApplicationFocus(bool focus)
    {
        //(Android) При открытой клавиатуре вызывает false
        _applicationIsFocus = focus;
    }

    private static T LoadResources<T>(string name) where T : UnityEngine.Object
    {
        return Resources.Load<T>(name) as T;
    }
}