using Zenject;
using UnityEngine;

public class GameInstaller : MonoInstaller
{
    public GameObject[] segmentPrefabs;
    public GameObject[] obstaclePrefabs;

    public override void InstallBindings()
    {
        Container.Bind<IStageGenerationStrategy>().To<BasicStageGenerationStrategy>().AsSingle();

        // セグメントプールのバインド
        Container.Bind<GameObjectPool>().WithId("SegmentPool").FromMethod(context =>
        {
            return new GameObjectPool(() => Instantiate(segmentPrefabs[Random.Range(0, segmentPrefabs.Length)]));
        }).AsTransient().WhenInjectedInto<StageGenerator>();

        // 障害物プールのバインド（obstaclePrefabsが空の場合はプールを生成しない）
        if (obstaclePrefabs != null && obstaclePrefabs.Length > 0)
        {
            Container.Bind<GameObjectPool>().WithId("ObstaclePool").FromMethod(context =>
            {
                return new GameObjectPool(() => Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)]));
            }).AsTransient().WhenInjectedInto<StageGenerator>();
        }
        else
        {
            // obstaclePrefabsが空の場合はnullをバインド
            Container.Bind<GameObjectPool>().WithId("ObstaclePool").To<GameObjectPool>().FromInstance(null).AsTransient();
        }
    }
}