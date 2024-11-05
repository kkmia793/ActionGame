using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class StageGenerator : MonoBehaviour
{
    private IStageGenerationStrategy _generationStrategy;
    private GameObjectPool _segmentPool;
    private GameObjectPool _obstaclePool;
    private Queue<GameObject> _activeSegments = new Queue<GameObject>();
    private Vector3 _nextSpawnPosition;

    [SerializeField] private Transform player;
    [SerializeField] private float spawnDistance = 30f; // プレイヤーがこの距離に達すると新しいセグメントを生成
    [SerializeField] private int maxSegments = 5; // 表示される最大セグメント数を設定

    [Inject]
    public void Construct(
        IStageGenerationStrategy generationStrategy,
        [Inject(Id = "SegmentPool")] GameObjectPool segmentPool,
        [InjectOptional(Id = "ObstaclePool")] GameObjectPool obstaclePool)
    {
        _generationStrategy = generationStrategy;
        _segmentPool = segmentPool;
        _obstaclePool = obstaclePool;
    }

    private void Start()
    {
        GenerateInitialSegments().Forget();
    }

    private async UniTaskVoid GenerateInitialSegments()
    {
        for (int i = 0; i < maxSegments; i++)
        {
            await GenerateSegment();
        }
    }

    private void Update()
    {
        if (player.position.x > _nextSpawnPosition.x - spawnDistance)
        {
            GenerateSegment().Forget();
        }
    }

    private async UniTask GenerateSegment()
    {
        GameObject newSegment = _segmentPool.Get();
        newSegment.transform.position = _nextSpawnPosition;
        newSegment.SetActive(true);
        _activeSegments.Enqueue(newSegment);

        // 全体の幅を子オブジェクトのRendererを使って計算
        float segmentWidth = CalculateTotalWidth(newSegment);
        _nextSpawnPosition += new Vector3(segmentWidth, 0, 0);

        // 障害物の生成は obstaclePool がある場合のみ実行
        if (_obstaclePool != null && Random.value > 0.5f)
        {
            GameObject newObstacle = _obstaclePool.Get();
            float obstacleOffsetX = Random.Range(1f, segmentWidth - 1f);
            newObstacle.transform.position = newSegment.transform.position + new Vector3(obstacleOffsetX, 1f, 0);
            newObstacle.SetActive(true);
        }

        if (_activeSegments.Count > maxSegments)
        {
            RemoveOldSegment();
        }

        await UniTask.Yield();
    }

    private float CalculateTotalWidth(GameObject segment)
    {
        Renderer[] renderers = segment.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return 0;

        Bounds combinedBounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }

        return combinedBounds.size.x;
    }

    private void RemoveOldSegment()
    {
        GameObject oldSegment = _activeSegments.Dequeue();
        oldSegment.SetActive(false);
        _segmentPool.Return(oldSegment);
    }
}