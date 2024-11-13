using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemySpawner : IEnemySpawner
{
    [Inject(Id = "EnemyPool")] private GameObjectPool _enemyPool;

    private readonly List<GameObject> _spawnedEnemies = new List<GameObject>();
    [SerializeField] private float yOffset = 0.5f;

    public async UniTask<GameObject> SpawnEnemy(Vector3 position)
    {
        GameObject enemy = _enemyPool.Get();
        enemy.transform.position = position;
        enemy.SetActive(true);
        _spawnedEnemies.Add(enemy); // 生成された敵をリストに追加
        await UniTask.Yield();
        return enemy;
    }

    public async UniTask SpawnEnemies(GameObject segment, int enemyCount)
    {
        List<Vector3> positions = GetSpawnPositions(segment, enemyCount);

        foreach (Vector3 position in positions)
        {
            await SpawnEnemy(position);
        }
    }

    public List<GameObject> GetSpawnedEnemies()
    {
        return new List<GameObject>(_spawnedEnemies); // 生成された敵のリストを返す
    }

    public void ReturnEnemyToPool(GameObject enemy)
    {
        if (_spawnedEnemies.Contains(enemy))
        {
            _spawnedEnemies.Remove(enemy);
            _enemyPool.Return(enemy);
            enemy.SetActive(false);
            Debug.Log("ok");
        }
    }

    private List<Vector3> GetSpawnPositions(GameObject segment, int enemyCount)
    {
        List<Vector3> spawnPositions = new List<Vector3>();
        Renderer renderer = segment.GetComponent<Renderer>();

        if (renderer == null)
        {
            Debug.LogError("Segment does not contain a Renderer component.");
            return spawnPositions;
        }

        float segmentWidth = renderer.bounds.size.x;
        float step = segmentWidth / (enemyCount + 1);

        for (int i = 1; i <= enemyCount; i++)
        {
            float xPos = renderer.bounds.min.x + step * i;
            float yPos = renderer.bounds.max.y + yOffset;
            spawnPositions.Add(new Vector3(xPos, yPos, 0));
        }

        return spawnPositions;
    }
}