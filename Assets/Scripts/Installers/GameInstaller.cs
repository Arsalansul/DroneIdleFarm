using UI;
using UnityEngine;
using Zenject;

public class MainSceneMonoInstaller : MonoInstaller
{
    [SerializeField] private UiManager uiManager;
    [SerializeField] private PoolManager poolManager;

    public override void InstallBindings()
    {
        poolManager.InitializePools();
        
        Container.Bind<UiManager>().FromInstance(uiManager).AsSingle();
        Container.Bind<PoolManager>().FromInstance(poolManager).AsSingle();
    }
}