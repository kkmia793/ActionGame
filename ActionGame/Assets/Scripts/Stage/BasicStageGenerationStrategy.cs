using Cysharp.Threading.Tasks;
using UnityEngine;

public class BasicStageGenerationStrategy : IStageGenerationStrategy
{
    public async UniTask GenerateSegment(Transform parent, GameObjectPool segmentPool, Vector3 nextSpawnPosition)
    {
        GameObject segment = segmentPool.Get();
        segment.transform.position = nextSpawnPosition;
        segment.SetActive(true);
        await UniTask.Yield(); // 非同期でステージ生成を行う
    }
}