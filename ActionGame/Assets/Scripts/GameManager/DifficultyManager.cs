using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager
{
    public int CalculateDifficulty(float distance, float distancePerLevel)
    {
        return Mathf.FloorToInt(distance / distancePerLevel) + 1;
    }

    public float GetPlayerSpeed(int difficultyLevel, float initialSpeed, float speedIncreasePerLevel)
    {
        return initialSpeed + difficultyLevel * speedIncreasePerLevel;
    }
}
