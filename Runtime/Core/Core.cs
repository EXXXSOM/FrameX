using UnityEngine;
using UnityEngine.SceneManagement;

public class Core : MonoBehaviour
{
    private static bool _applicationIsFocus = false;
    private static bool _applicationIsPaused = false;
    private static bool _applicationIsQuitting = false;
    private static bool _initialized = false;
    private const string CORE_SCENE_NAME = "CORE";


    public static readonly ProcessorUpdate Updater = new ProcessorUpdate();
    public static bool ApplicationIsFocus => _applicationIsFocus;
    public static bool ApplicationIsPaused => _applicationIsPaused;
    public static bool ApplicationIsQuitting => _applicationIsQuitting;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Bootstrap()
    {
        if (_initialized) return;
        SetupCore();
        SetupManagers();
        SetupApplicationSettings();
        SetupPlugins();

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

            SceneManager.MoveGameObjectToScene(coreOG, scene);
            SceneManager.MoveGameObjectToScene(bootLoaderGO, scene);

            _initialized = true;
            Debug.Log("[CORE]: Started!");
        }
    }

    private static void SetupPlugins()
    {
        //Pluggable[] pluginsz = Resources.LoadAll<Pluggable>("Plugins");
    }

    private static void SetupApplicationSettings()
    {
    }

    private static void SetupManagers()
    {
    }

    private void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
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
}
