using UnityEngine;

[CreateAssetMenu(fileName = "SceneCollectionObject", menuName = "FrameX/SceneCollection/SceneCollectionObject", order = 1)]
public class SceneCollectionObject : ScriptableObject
{
    public string MainSceneName;
    [SerializeField] public SceneReference[] scenes;
}