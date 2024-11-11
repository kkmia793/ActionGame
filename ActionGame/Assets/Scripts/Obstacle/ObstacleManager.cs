using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class ObstacleManager : MonoBehaviour
{
    [Inject] private ObstacleSpawner _obstacleSpawner;

    private List<GameObject> _activeObstacles = new List<GameObject>();

    /// <summary>
    /// 指定されたセグメントに障害物を生成し、リストに追加。
    /// </summary>
    public async UniTask GenerateObstacles(GameObject segment, int obstacleCount)
    {
        for (int i = 0; i < obstacleCount; i++)
        {
            Vector3 spawnPosition = _obstacleSpawner.GetSpawnPosition(segment);
            GameObject obstacle = await _obstacleSpawner.SpawnObstacle(spawnPosition);
            _activeObstacles.Add(obstacle);
        }
    }

    /// <summary>
    /// 障害物の状態を監視し、非アクティブな障害物をリストから削除。
    /// </summary>
    public void UpdateObstacleStates()
    {
        for (int i = _activeObstacles.Count - 1; i >= 0; i--)
        {
            if (!_activeObstacles[i].activeInHierarchy)
            {
                _activeObstacles.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// すべての障害物を非アクティブにし、リストをクリア。
    /// </summary>
    public void ClearAllObstacles()
    {
        foreach (var obstacle in _activeObstacles)
        {
            obstacle.SetActive(false);
        }
        _activeObstacles.Clear();
    }
}