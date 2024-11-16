using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class FireballSpawner : IObstacleSpawner
{
    [Inject(Id = "FireballPool")] private GameObjectPool _pool;

    public async UniTask SpawnObstacles(ObstacleData data, GameObject segment)
    {
        foreach (var height in data.spawnPositions)
        {
            var fireball = _pool.Get();
            fireball.transform.position = new Vector3(segment.transform.position.x, height.y, 0);
            fireball.SetActive(true);

            Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.left * 10f;

            await UniTask.Delay((int)(data.spawnInterval * 1000));
        }
    }
}