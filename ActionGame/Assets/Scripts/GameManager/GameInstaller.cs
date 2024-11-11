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
        

        // 敵プールのバインド
        Container.Bind<GameObjectPool>().WithId("EnemyPool").FromMethod(context =>
        {
            return new GameObjectPool(() => Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]));
        }).AsTransient().WhenInjectedInto<EnemySpawner>();

        // クラスのバインド
        Container.Bind<IEnemySpawner>().To<EnemySpawner>().AsSingle();
        Container.Bind<EnemyManager>().AsSingle();
    }

    // NullObstaclePool：障害物を生成しない場合のクラス
    private class NullObstaclePool : GameObjectPool
    {
        public NullObstaclePool() : base(() => null) { }

        public override GameObject Get() => null; // 実際には生成しない
        public override void Return(GameObject obj) { }
    }
}