using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using Zenject;

public class ObstacleSpawner : MonoBehaviour, IObstacleSpawner
{
    private readonly GameObjectPool _pool;

    [Inject]
    public ObstacleSpawner([Inject(Id = "ObstaclePool")] GameObjectPool pool)
    {
        _pool = pool;
    }

    public async UniTask SpawnObstacles(ObstacleData data, GameObject segment)
    {
        switch (data.type)
        {
            case ObstacleType.IceObstacle:
                await SpawnIceObstacles(segment, data);
                break;

            case ObstacleType.Fireball:
                await SpawnFireballs(segment, data);
                break;

            case ObstacleType.Cloud:
                SpawnClouds(segment, data);
                break;
        }
    }

    private async UniTask SpawnIceObstacles(GameObject segment, ObstacleData data)
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

    private async UniTask SpawnFireballs(GameObject segment, ObstacleData data)
    {
        foreach (var height in data.spawnPositions)
        {
            var fireball = _pool.Get();
            fireball.transform.position = new Vector3(segment.transform.position.x, height.y, 0);
            fireball.SetActive(true);

            Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.left * 10f;

            await UniTask.Delay((int)(data.spawnInterval * 1000));
        }
    }

    private void SpawnClouds(GameObject segment, ObstacleData data)
    {
        foreach (var position in data.spawnPositions)
        {
            var cloud = _pool.Get();
            cloud.transform.position = new Vector3(segment.transform.position.x + position.x, position.y, 0);
            cloud.SetActive(true);
        }
    }
}