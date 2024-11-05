using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{
    private readonly Func<GameObject> _createFunc;
    private readonly Queue<GameObject> _pool = new Queue<GameObject>();

    public GameObjectPool(Func<GameObject> createFunc)
    {
        _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
    }

    public GameObject Get()
    {
        GameObject obj = _pool.Count > 0 ? _pool.Dequeue() : _createFunc();
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}