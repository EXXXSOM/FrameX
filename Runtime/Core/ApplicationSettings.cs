using UnityEngine;

[CreateAssetMenu(fileName = "ApplicationSettings", menuName = "FrameX/ApplicationSettings", order = 1)]
public class ApplicationSettings : ScriptableObject
{
    [SerializeField] private int _vSyncCount = 0; //Disable
    [SerializeField] private int _targetFrameRate = -1; //No limite

    public int vSyncCount => _vSyncCount;
    public int TargetFrameRate => _targetFrameRate;
}
