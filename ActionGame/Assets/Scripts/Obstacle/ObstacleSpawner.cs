using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class ObstacleSpawner : MonoBehaviour
{
    [Inject(Id = "ObstaclePool")] private GameObjectPool _obstaclePool;

    public async UniTask<GameObject> SpawnObstacle(Vector3 position)
    {
        GameObject obstacle = _obstaclePool.Get();
        obstacle.transform.position = position;
        obstacle.SetActive(true);
        await UniTask.Yield();
        return obstacle;
    }

    public Vector3 GetSpawnPosition(GameObject segment)
    {
        Renderer renderer = segment.GetComponent<Renderer>();
        float xPos = Random.Range(renderer.bounds.min.x, renderer.bounds.max.x);
        float yPos = renderer.bounds.max.y; // セグメント上に配置
        return new Vector3(xPos, yPos, 0);
    }
}