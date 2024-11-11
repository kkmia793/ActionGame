
using UnityEngine;
using Cysharp.Threading.Tasks;
public interface IEnemySpawner
{
    UniTask<GameObject> SpawnEnemy(Vector3 position);
    Vector3 GetSpawnPosition(GameObject segment);
}