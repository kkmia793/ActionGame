using Zenject;

public class EnemyInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GameObjectPool>().WithId("EnemyPool").FromComponentInHierarchy().AsSingle();
    }
}