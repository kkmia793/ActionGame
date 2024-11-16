using UnityEngine;

[CreateAssetMenu(fileName = "NewObstacleData", menuName = "Obstacle/ObstacleData")]
public class ObstacleData : ScriptableObject
{
    public ObstacleType type;       
    public GameObject prefab;      
    public float spawnInterval;    
    public Vector2[] spawnPositions; 
}