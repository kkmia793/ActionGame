using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class IceObstacleSpawner : IObstacleSpawner
{
    [Inject(Id = "IceObstaclePool")] private GameObjectPool _pool;

    public async UniTask SpawnObstacles(ObstacleData data, GameObject segment)
    {
        Renderer renderer = segment.GetComponent<Renderer>();
        if (renderer == null) return;

        float xMin = renderer.bounds.min.x;
        float xMax = renderer.bounds.max.x;
        float yPos = renderer.bounds.max.y;

        var leftObstacle = _pool.Get();
        leftObstacle.transform.position = new Vector3(xMin, yPos, 0);
        leftObstacle.SetActive(true);

        var rightObstacle = _pool.Get();
        rightObstacle.transform.position = new Vector3(xMax, yPos, 0);
        rightObstacle.SetActive(true);

        await UniTask.Yield();
    }
}