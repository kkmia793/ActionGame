using Zenject;
using UnityEngine;

public class GameInstaller : MonoInstaller
{
    public GameObject[] segmentPrefabs;
    public GameObject[] obstaclePrefabs;
    public GameObject[] enemyPrefabs;

    public override void InstallBindings()
    {
        Container.Bind<IStageGenerationStrategy>().To<BasicStageGenerationStrategy>().AsSingle();

        BindGameObjectPoolFactory("SegmentPool", segmentPrefabs);
        BindGameObjectPoolFactory("ObstaclePool", obstaclePrefabs);
        BindGameObjectPoolFactory("EnemyPool", enemyPrefabs);

        Container.Bind<ObstacleSpawner>().AsSingle();
        Container.Bind<ObstacleManager>().AsSingle();
        Container.Bind<IEnemySpawner>().To<EnemySpawner>().AsSingle();
        Container.Bind<EnemyManager>().AsSingle();
    }

    private void BindGameObjectPoolFactory(string poolId, GameObject[] prefabs)
    {
        Container.Bind<GameObjectPool>().WithId(poolId)
            .FromMethod(_ => new GameObjectPool(() =>
            {
                var obj = Instantiate(prefabs[Random.Range(0, prefabs.Length)]);
                Container.Inject(obj); // 依存性注入
                return obj;
            }, Container))
            .AsCached();
    }
}