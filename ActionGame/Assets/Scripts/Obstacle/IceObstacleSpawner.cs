using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class IceObstacleSpawner : IObstacleSpawner
{
    [Inject(Id = "IceObstaclePool")] private GameObjectPool _pool;

    private readonly Dictionary<GameObject, List<GameObject>> _segmentObstacles = new Dictionary<GameObject, List<GameObject>>();

    public async UniTask SpawnObstacles(ObstacleData data, GameObject segment)
    {
        Renderer renderer = segment.GetComponent<Renderer>();
        if (renderer == null) return;

        float xMin = renderer.bounds.min.x;
        float xMax = renderer.bounds.max.x;
        float yPos = renderer.bounds.max.y;

        if (!_segmentObstacles.ContainsKey(segment))
        {
            _segmentObstacles[segment] = new List<GameObject>();
        }

        var leftObstacle = _pool.Get();
        leftObstacle.transform.position = new Vector3(xMin, yPos, 0);
        leftObstacle.SetActive(true);
        _segmentObstacles[segment].Add(leftObstacle);

        var rightObstacle = _pool.Get();
        rightObstacle.transform.position = new Vector3(xMax, yPos, 0);
        rightObstacle.SetActive(true);
        _segmentObstacles[segment].Add(rightObstacle);

        await UniTask.Yield();
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