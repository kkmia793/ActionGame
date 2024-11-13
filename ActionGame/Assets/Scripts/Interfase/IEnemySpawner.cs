using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemySpawner
{
    UniTask<GameObject> SpawnEnemy(Vector3 position);
    UniTask SpawnEnemies(GameObject segment, int enemyCount);
    List<GameObject> GetSpawnedEnemies(); // 生成された敵のリストを取得
    void ReturnEnemyToPool(GameObject enemy); // 敵をプールに戻す
}