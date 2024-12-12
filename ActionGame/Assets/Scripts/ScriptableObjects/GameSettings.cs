using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/GameSettings")]
public class GameSettings : ScriptableObject
{
    public float FallThreshold = -10f;
    public float SpeedThreshold = 1f;
    public float DistancePerLevel = 50f;
    public float SpeedIncreasePerLevel = 0.5f;
    public float InitialSpeed = 15f;
    public float MaxSpeed = 25f;
}