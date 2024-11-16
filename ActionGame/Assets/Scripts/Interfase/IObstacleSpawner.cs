using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IObstacleSpawner
{
    UniTask SpawnObstacles(ObstacleData data, GameObject segment);
}