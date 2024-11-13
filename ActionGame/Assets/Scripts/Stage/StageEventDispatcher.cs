using UnityEngine;
using System;

public class StageEventDispatcher : MonoBehaviour
{
    public static event Action<GameObject> OnStageSegmentGenerated;

    public static void NotifyStageSegmentGenerated(GameObject segment)
    {
        OnStageSegmentGenerated?.Invoke(segment);
    }
}