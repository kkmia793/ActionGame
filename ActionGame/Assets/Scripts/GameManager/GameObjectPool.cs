using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameObjectPool
{
    private readonly Func<GameObject> _createFunc;
    private readonly Queue<GameObject> _pool = new Queue<GameObject>();
    private readonly DiContainer _container;

    public GameObjectPool(Func<GameObject> createFunc, DiContainer container)
    {
        _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
        _container = container ?? throw new ArgumentNullException(nameof(container));
    }

    public virtual GameObject Get()
    {
        GameObject obj = _pool.Count > 0 ? _pool.Dequeue() : _createFunc();
        _container.Inject(obj); // 依存性注入
        obj.SetActive(true);
        return obj;
    }

    public virtual void Return(GameObject obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}