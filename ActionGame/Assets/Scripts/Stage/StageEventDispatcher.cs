using UnityEngine;
using System;

public class StageEventDispatcher : MonoBehaviour
{
    public static event Action<GameObject> OnStageSegmentGenerated;
    public static event Action<GameObject> OnStageSegmentRemoved;

    public static void NotifyStageSegmentGenerated(GameObject segment)
    {
        OnStageSegmentGenerated?.Invoke(segment);
    }

    public static void NotifyStageSegmentRemoved(GameObject segment)
    {
        OnStageSegmentRemoved?.Invoke(segment);
    }
}