using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class StageGenerator : MonoBehaviour
{
    private IStageGenerationStrategy _generationStrategy;
    private GameObjectPool _segmentPool;
    private Queue<GameObject> _activeSegments = new Queue<GameObject>();
    private Vector3 _nextSpawnPosition;

    [SerializeField] private Transform player;
    [SerializeField] private float spawnDistance = 30f; // プレイヤーがこの距離に達すると新しいセグメントを生成
    [SerializeField] private int maxSegments = 5;       // 表示される最大セグメント数を設定
    [SerializeField] private GameObject initialPlatformPrefab; // 最初に生成する初期プラットフォーム

    private bool initialPlatformGenerated = false; // InitialPlatformを生成済みかどうか

    [Inject]
    public void Construct(
        IStageGenerationStrategy generationStrategy,
        [Inject(Id = "SegmentPool")] GameObjectPool segmentPool)
    {
        _generationStrategy = generationStrategy;
        _segmentPool = segmentPool;
    }

    private void Start()
    {
        GenerateInitialSegments().Forget();
    }

    private async UniTaskVoid GenerateInitialSegments()
    {
        if (!initialPlatformGenerated)
        {
            GenerateInitialPlatform();
        }

        for (int i = 1; i < maxSegments; i++) // InitialPlatform以外を生成
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

    private void GenerateInitialPlatform()
    {
        GameObject initialPlatform = Instantiate(initialPlatformPrefab, _nextSpawnPosition, Quaternion.identity);
        initialPlatform.SetActive(true);
        _activeSegments.Enqueue(initialPlatform);

        float segmentWidth = CalculateTotalWidth(initialPlatform);
        _nextSpawnPosition += new Vector3(segmentWidth, 0, 0);

        initialPlatformGenerated = true; // 初期プラットフォームを生成済みに設定
    }

    private async UniTask GenerateSegment()
    {
        GameObject newSegment = _segmentPool.Get();
        newSegment.transform.position = _nextSpawnPosition;
        newSegment.SetActive(true);
        _activeSegments.Enqueue(newSegment);
        
        StageEventDispatcher.NotifyStageSegmentGenerated(newSegment);

        float segmentWidth = CalculateTotalWidth(newSegment);
        _nextSpawnPosition += new Vector3(segmentWidth, 0, 0);

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