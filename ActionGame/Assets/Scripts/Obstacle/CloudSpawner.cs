using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class CloudSpawner : IObstacleSpawner
{
    [Inject(Id = "CloudPool")] private GameObjectPool _pool;

    public async UniTask SpawnObstacles(ObstacleData data, GameObject segment)
    {
        foreach (var position in data.spawnPositions)
        {
            var cloud = _pool.Get();
            cloud.transform.position = segment.transform.position + (Vector3)position;
            cloud.SetActive(true);
            await UniTask.Yield();
        }
    }
}