using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    public static SceneSetup ActiveSceneSetup;

    [SerializeField] private SetupConfig _sceneSetupConfig = null;

    private void Awake()
    {
        if (BootLoader.NextSceneName == gameObject.scene.name)
            ActiveSceneSetup = this;

        if (_sceneSetupConfig != null)
        {
            _sceneSetupConfig.OnAwake();
            _sceneSetupConfig.OnStart();
        }
    }
}
