using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemySpawner : IEnemySpawner
{
    [Inject(Id = "EnemyPool")] private GameObjectPool _enemyPool;

    // セグメントごとの敵リストを管理
    private readonly Dictionary<GameObject, List<GameObject>> _segmentEnemies = new Dictionary<GameObject, List<GameObject>>();
    private readonly List<GameObject> _spawnedEnemies = new List<GameObject>(); // 全生成敵のリスト
    [SerializeField] private float yOffset = 1.0f;

    public async UniTask SpawnEnemies(GameObject segment, int enemyCount)
    {
        List<Vector3> positions = GetSpawnPositions(segment, enemyCount);

        // 敵リストを初期化
        if (!_segmentEnemies.ContainsKey(segment))
        {
            _segmentEnemies[segment] = new List<GameObject>();
        }

        foreach (Vector3 position in positions)
        {
            GameObject enemy = await SpawnEnemy(position);
            _segmentEnemies[segment].Add(enemy); // セグメントに関連付け
            _spawnedEnemies.Add(enemy); // 全生成リストに追加
        }
    }

    public async UniTask<GameObject> SpawnEnemy(Vector3 position)
    {
        GameObject enemy = _enemyPool.Get();
        enemy.transform.position = position;
        enemy.SetActive(true);
        await UniTask.Yield();
        return enemy;
    }

    public List<GameObject> GetSpawnedEnemies()
    {
        return new List<GameObject>(_spawnedEnemies); // 生成された敵のリストを返す
    }

    public void ReturnEnemyToPool(GameObject enemy)
    {
        if (enemy != null && _spawnedEnemies.Contains(enemy))
        {
            enemy.SetActive(false);
            _spawnedEnemies.Remove(enemy); // 全生成リストから削除
            _enemyPool.Return(enemy);
        }
    }

    public void HandleSegmentRemoved(GameObject segment)
    {
        // セグメントに関連付けられた敵をプールに戻す
        if (_segmentEnemies.ContainsKey(segment))
        {
            foreach (var enemy in _segmentEnemies[segment])
            {
                ReturnEnemyToPool(enemy);
            }
            _segmentEnemies.Remove(segment); // セグメントのリストから削除
        }
    }

    private List<Vector3> GetSpawnPositions(GameObject segment, int enemyCount)
    {
        List<Vector3> spawnPositions = new List<Vector3>();

        // 子要素 "LongPlatform" を取得
        Transform childTransform = segment.transform.Find("LongPlatform");
        if (childTransform == null)
        {
            Debug.LogError($"Child 'LongPlatform' not found under segment '{segment.name}'.");
            return spawnPositions;
        }

        // 子要素の Renderer を取得
        Renderer renderer = childTransform.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError($"Renderer not found on child 'LongPlatform' under segment '{segment.name}'.");
            return spawnPositions;
        }

        // 子要素の幅を取得
        float segmentWidth = renderer.bounds.size.x;
        float step = segmentWidth / (enemyCount + 1); // 敵の間隔を計算

        for (int i = 1; i <= enemyCount; i++)
        {
            float xPos = renderer.bounds.min.x + step * i; // 敵の x 座標
            float yPos = renderer.bounds.max.y + yOffset;  // 敵の y 座標
            spawnPositions.Add(new Vector3(xPos, yPos, 0)); // 生成位置をリストに追加
        }

        return spawnPositions;
    }
}