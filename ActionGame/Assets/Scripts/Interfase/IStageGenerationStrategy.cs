using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public interface IStageGenerationStrategy
{
    UniTask GenerateSegment(Transform parent, GameObjectPool segmentPool, Vector3 nextSpawnPosition);
}

