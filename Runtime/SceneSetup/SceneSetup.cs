using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-10200)]
public class SceneSetup : MonoBehaviour
{
    private static SceneSetup _activeSceneSetup;
    public readonly DependencyContainer Container = new DependencyContainer(20);
    public readonly ProcessorUpdate Engine = new ProcessorUpdate();
    //ProcessorRoutine

    [SerializeField] private SceneSetupConfig _sceneSetupConfig = null;

    public static SceneSetup ActiveSceneSetup => _activeSceneSetup;
    public static DependencyContainer ActiveSceneDependencyContainer => ActiveSceneSetup.Container;

    private void Awake()
    {
        Bootstrap();

        if (_activeSceneSetup == null)
            _activeSceneSetup = this;

        if (_sceneSetupConfig != null)
            _sceneSetupConfig.OnAwake(this);
    }

    private void Start()
    {
        if (_sceneSetupConfig != null)
            _sceneSetupConfig.OnStart();
    }

    public void Bootstrap()
    {
        StartCoroutine(CoWaitForSceneLoad());

        IEnumerator CoWaitForSceneLoad()
        {
            while (!gameObject.scene.isLoaded)
                yield return 0;

            Debug.Log("Scene loaded!");
            if (gameObject.scene.name == BootLoader.NextActiveSceneName)
            {
                _activeSceneSetup = this;
            }
        }
    }

    private void OnDisable()
    {
        Engine.Dispose();
        //ProcessorRoutine.Dispose();

        if (_sceneSetupConfig != null)
            _sceneSetupConfig.OnDispose();
    }
}
