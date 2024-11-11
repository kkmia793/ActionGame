using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class EnemySpawner : IEnemySpawner
{
    [Inject(Id = "EnemyPool")] private GameObjectPool _enemyPool;

    public async UniTask<GameObject> SpawnEnemy(Vector3 position)
    {
        GameObject enemy = _enemyPool.Get();
        enemy.transform.position = position;
        enemy.SetActive(true);
        await UniTask.Yield();
        return enemy;
    }

    public Vector3 GetSpawnPosition(GameObject segment)
    {
        Renderer renderer = segment.GetComponent<Renderer>();
        float xPos = Random.Range(renderer.bounds.min.x, renderer.bounds.max.x);
        float yPos = renderer.bounds.max.y; // セグメント上に配置
        return new Vector3(xPos, yPos, 0);
    }
}