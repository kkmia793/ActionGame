using Zenject;
using UnityEngine;
using System.Collections.Generic;

public class GameInstaller : MonoInstaller
{
    public GameObject[] segmentPrefabs;
    public GameObject[] obstaclePrefabs;
    public GameObject[] enemyPrefabs;
    
    public List<ObstacleData> obstacleDataList;
    
    // 障害物ごとのプレハブ
       public GameObject iceObstaclePrefab;
       public GameObject fireballPrefab;
       public GameObject cloudPrefab;

    public override void InstallBindings()
    {
        Container.Bind<IStageGenerationStrategy>().To<BasicStageGenerationStrategy>().AsSingle();

        BindGameObjectPoolFactory("SegmentPool", segmentPrefabs);
        //BindGameObjectPoolFactory("ObstaclePool", obstaclePrefabs);
        BindGameObjectPoolFactory("EnemyPool", enemyPrefabs);
        
        // 障害物のプールを個別に設定
        BindGameObjectPoolFactory("IceObstaclePool", new GameObject[] { iceObstaclePrefab });
        BindGameObjectPoolFactory("FireballPool", new GameObject[] { fireballPrefab });
        BindGameObjectPoolFactory("CloudPool", new GameObject[] { cloudPrefab });

        // 各障害物に対応するスポナーをバインド
        Container.Bind<IObstacleSpawner>().WithId(ObstacleType.IceObstacle).To<IceObstacleSpawner>().AsTransient();
        Container.Bind<IObstacleSpawner>().WithId(ObstacleType.Fireball).To<FireballSpawner>().AsTransient();
        Container.Bind<IObstacleSpawner>().WithId(ObstacleType.Cloud).To<CloudSpawner>().AsTransient();

        // Dictionaryを作成してObstacleManagerに渡す
        Container.Bind<Dictionary<ObstacleType, IObstacleSpawner>>()
            .FromMethod(_ => new Dictionary<ObstacleType, IObstacleSpawner>
            {
                { ObstacleType.IceObstacle, Container.ResolveId<IObstacleSpawner>(ObstacleType.IceObstacle) },
                { ObstacleType.Fireball, Container.ResolveId<IObstacleSpawner>(ObstacleType.Fireball) },
                { ObstacleType.Cloud, Container.ResolveId<IObstacleSpawner>(ObstacleType.Cloud) }
            })
            .AsSingle();
        
        // 障害物のプールとスポナーを設定
        //BindObstaclePoolsAndSpawners();
        
        Container.Bind<List<ObstacleData>>()
            .FromInstance(obstacleDataList) // Inspectorから設定されるobstacleDataListを使用
            .AsSingle();
        
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
    
    //private void BindObstaclePoolsAndSpawners()
    //{
        //foreach (var obstacleData in obstacleDataList)
        //{
            //Container.Bind<GameObjectPool>().WithId(obstacleData.type)
                //.FromMethod(_ => new GameObjectPool(() => Instantiate(obstacleData.prefab), Container))
                //.AsCached();

            // スポナーを設定
            //Container.Bind<IObstacleSpawner>().WithId(obstacleData.type).To<ObstacleSpawner>().AsTransient();
        //}
    //}
}