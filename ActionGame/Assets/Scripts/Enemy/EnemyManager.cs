using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class EnemyManager : MonoBehaviour
{
    [Inject] private IEnemySpawner _enemySpawner;

    private List<GameObject> _activeEnemies = new List<GameObject>();

    public async UniTask GenerateEnemies(GameObject segment, int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = _enemySpawner.GetSpawnPosition(segment);
            GameObject enemy = await _enemySpawner.SpawnEnemy(spawnPosition);
            _activeEnemies.Add(enemy);
        }
    }

    public void UpdateEnemyStates()
    {
        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = _activeEnemies[i];
            if (!enemy.activeInHierarchy) // 敵が倒されたまたは画面外
            {
                _activeEnemies.RemoveAt(i);
                enemy.SetActive(false); // オブジェクトプールに戻す
            }
        }
    }

    public void ClearAllEnemies()
    {
        foreach (var enemy in _activeEnemies)
        {
            enemy.SetActive(false); // オブジェクトプールに戻す
        }
        _activeEnemies.Clear();
    }
}