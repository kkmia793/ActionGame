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
    }

    private void OnDisable()
    {
        StageEventDispatcher.OnStageSegmentGenerated -= HandleStageSegmentGenerated;
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