using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyManager : MonoBehaviour
{
    [Inject] private IEnemySpawner _enemySpawner;

    [SerializeField, Range(0f, 1f)] private float spawnProbabilityForLongPlatform = 0.7f;

    private List<EnemyCharacter> _activeEnemies = new List<EnemyCharacter>();

    private void OnEnable()
    {
        StageEventDispatcher.OnStageSegmentGenerated += HandleStageSegmentGenerated;
        StageEventDispatcher.OnStageSegmentRemoved += HandleStageSegmentRemoved;
    }

    private void OnDisable()
    {
        StageEventDispatcher.OnStageSegmentGenerated -= HandleStageSegmentGenerated;
        StageEventDispatcher.OnStageSegmentRemoved -= HandleStageSegmentRemoved;
    }

    private void HandleStageSegmentGenerated(GameObject segment)
    {
        if (segment.name.Contains("LongPlatform"))
        {
            if (Random.value <= spawnProbabilityForLongPlatform)
            {
                GenerateEnemies(segment, Random.Range(12,16)).Forget();
            }
        }
    }

    private void HandleStageSegmentRemoved(GameObject segment)
    {
        // セグメント削除時に関連する敵をプールに戻す
        _enemySpawner.HandleSegmentRemoved(segment);
    }

    public async UniTask GenerateEnemies(GameObject segment, int enemyCount)
    {
        await _enemySpawner.SpawnEnemies(segment, enemyCount);

        foreach (var enemy in _enemySpawner.GetSpawnedEnemies())
        {
            var enemyCharacter = enemy.GetComponent<EnemyCharacter>();
            if (enemyCharacter != null)
            {
                _activeEnemies.Add(enemyCharacter);
                enemyCharacter.OnDeath += HandleEnemyDeath;
            }
        }
    }

    private void HandleEnemyDeath(EnemyCharacter enemy)
    {
        enemy.OnDeath -= HandleEnemyDeath;
        _activeEnemies.Remove(enemy);

        // オブジェクトプールに戻す
        _enemySpawner.ReturnEnemyToPool(enemy.gameObject);
    }
}