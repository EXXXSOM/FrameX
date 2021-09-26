using System.Collections;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    public static SceneSetup ActiveSceneSetup;

    [SerializeField] private SetupConfig _sceneSetupConfig = null;

    private void Awake()
    {
        if (_sceneSetupConfig != null)
        {
            _sceneSetupConfig.OnAwake();
        }

        Bootstrap();
    }

    private void Start()
    {
        if (_sceneSetupConfig != null)
        {
            _sceneSetupConfig.OnStart();
        }
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
                ActiveSceneSetup = this;
            }
        }
    }
}
