using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class ObstacleManager : MonoBehaviour
{
    [Inject] private Dictionary<ObstacleType, IObstacleSpawner> _spawners;
    [Inject] private List<ObstacleData> _obstacleDataList;

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
        if (segment.name.Contains("ObstaclePlatform"))
        {
            var obstacleType = GetRandomObstacleType();
            var obstacleData = GetObstacleData(obstacleType);
            _spawners[obstacleType].SpawnObstacles(obstacleData, segment).Forget();
        }
    }

    private void HandleStageSegmentRemoved(GameObject segment)
    {
        if (segment.name.Contains("ObstaclePlatform"))
        {
            foreach (var spawner in _spawners.Values)
            {
                spawner.ReturnObstaclesToPool(segment);
            }
        }
    }

    private ObstacleType GetRandomObstacleType()
    {
        var values = System.Enum.GetValues(typeof(ObstacleType));
        return (ObstacleType)values.GetValue(Random.Range(0, values.Length));
    }

    private ObstacleData GetObstacleData(ObstacleType type)
    {
        return _obstacleDataList.Find(data => data.type == type);
    }
}