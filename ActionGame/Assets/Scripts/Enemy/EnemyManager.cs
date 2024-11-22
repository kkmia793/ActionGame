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
    }

    private void OnDisable()
    {
        StageEventDispatcher.OnStageSegmentGenerated -= HandleStageSegmentGenerated;
    }

    private void HandleStageSegmentGenerated(GameObject segment)
    {
        if (segment.name.Contains("LongPlatform"))
        {
            if (Random.value <= spawnProbabilityForLongPlatform)
            {
                GenerateEnemies(segment, enemyCount:6).Forget();
            }
        }
        else
        {
            //Debug.Log($"Skipping enemy spawn for segment: {segment.name}");
        }
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