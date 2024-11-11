using Zenject;
using UnityEngine;

public class GameInstaller : MonoInstaller
{
    public GameObject[] segmentPrefabs; // ステージセグメント
    public GameObject[] obstaclePrefabs; // 障害物プレハブ
    public GameObject[] enemyPrefabs;    // 敵プレハブ

    public override void InstallBindings()
    {
        // ステージ生成戦略のバインド
        Container.Bind<IStageGenerationStrategy>().To<BasicStageGenerationStrategy>().AsSingle();

        // セグメントプールのバインド
        Container.Bind<GameObjectPool>().WithId("SegmentPool").FromMethod(context =>
        {
            return new GameObjectPool(() => Instantiate(segmentPrefabs[Random.Range(0, segmentPrefabs.Length)]));
        }).AsTransient().WhenInjectedInto<StageGenerator>();

        // 障害物プールのバインド
        if (obstaclePrefabs != null && obstaclePrefabs.Length > 0)
        {
            Container.Bind<GameObjectPool>().WithId("ObstaclePool").FromMethod(context =>
            {
                return new GameObjectPool(() => Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)]));
            }).AsTransient().WhenInjectedInto<ObstacleSpawner>();
        }

        // 敵プールのバインド
        Container.Bind<GameObjectPool>().WithId("EnemyPool").FromMethod(context =>
        {
            return new GameObjectPool(() => Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]));
        }).AsTransient().WhenInjectedInto<EnemySpawner>();

        // クラスのバインド
        Container.Bind<ObstacleSpawner>().AsSingle();
        Container.Bind<ObstacleManager>().AsSingle();
        Container.Bind<IEnemySpawner>().To<EnemySpawner>().AsSingle();
        Container.Bind<EnemyManager>().AsSingle();
    }
}