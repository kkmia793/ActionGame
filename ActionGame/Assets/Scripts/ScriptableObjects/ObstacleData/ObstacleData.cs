using UnityEngine;

[CreateAssetMenu(fileName = "NewObstacleData", menuName = "Obstacle/ObstacleData")]
public class ObstacleData : ScriptableObject
{
    public ObstacleType type;              // 障害物の種類
    public GameObject prefab;              // 障害物のプレハブ
    public float spawnInterval;            // 生成間隔
    public Vector2[] spawnPositions;       // 生成位置
    public int baseSpawnCount = 1;         // 基本の生成数（初期値）
    public int maxSpawnCount = 10;         // 最大生成数（上限値）

    [HideInInspector] public int currentSpawnCount;  // 動的な生成数
}