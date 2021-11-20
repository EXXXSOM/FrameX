using UnityEngine;

public abstract class SceneSetupConfig : MonoBehaviour
{
    //Устанавливает все зависимости сцены
    public abstract void OnAwake(SceneSetup callScene);
    public virtual void OnStart() { }

    //При выгрузке сцены выгружает ненужные объекты
    public virtual void OnDispose() { }
}
