using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class FireballSpawner : IObstacleSpawner
{
    [Inject(Id = "FireballPool")] private GameObjectPool _pool;

    
    private readonly Dictionary<GameObject, List<GameObject>> _segmentObstacles = new Dictionary<GameObject, List<GameObject>>();

    public async UniTask SpawnObstacles(ObstacleData data, GameObject segment)
    {
        if (!_segmentObstacles.ContainsKey(segment))
        {
            _segmentObstacles[segment] = new List<GameObject>();
        }

        foreach (var height in data.spawnPositions)
        {
            var fireball = _pool.Get(); 
            fireball.transform.position = new Vector3(segment.transform.position.x, height.y, 0);
            fireball.SetActive(true);

            
            Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.left * 10f; 
            }


            _segmentObstacles[segment].Add(fireball);

            await UniTask.Delay((int)(data.spawnInterval * 1000));
        }
    }

    public void ReturnObstaclesToPool(GameObject segment)
    {
        if (_segmentObstacles.ContainsKey(segment))
        {
            foreach (var fireball in _segmentObstacles[segment])
            {
                fireball.SetActive(false);
                _pool.Return(fireball);  
            }

            _segmentObstacles.Remove(segment); 
        }
    }
}