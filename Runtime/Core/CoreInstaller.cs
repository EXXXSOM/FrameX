using UnityEngine;

public class CoreInstaller : MonoBehaviour
{
    public void Install(DependencyContainer coreContainer)
    {
        GameData data = SaveSystem.LoadData();
        GameManager.LoadGameModes();
        GameManager.LoadGameData(data);
    }
}
