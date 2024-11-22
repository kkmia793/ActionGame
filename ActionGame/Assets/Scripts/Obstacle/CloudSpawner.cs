using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class CloudSpawner : IObstacleSpawner
{
    [Inject(Id = "CloudPool")] private GameObjectPool _pool;

    private readonly Dictionary<GameObject, List<GameObject>> _segmentObstacles = new Dictionary<GameObject, List<GameObject>>();

    public async UniTask SpawnObstacles(ObstacleData data, GameObject segment)
    {
        if (!_segmentObstacles.ContainsKey(segment))
        {
            _segmentObstacles[segment] = new List<GameObject>();
        }

        foreach (var position in data.spawnPositions)
        {
            var cloud = _pool.Get();
            cloud.transform.position = segment.transform.position + (Vector3)position;
            cloud.SetActive(true);
            _segmentObstacles[segment].Add(cloud);
            await UniTask.Yield();
        }
    }

    public void ReturnObstaclesToPool(GameObject segment)
    {
        if (_segmentObstacles.ContainsKey(segment))
        {
            foreach (var obstacle in _segmentObstacles[segment])
            {
                obstacle.SetActive(false);
                _pool.Return(obstacle);
            }
            _segmentObstacles.Remove(segment);
        }
    }
}