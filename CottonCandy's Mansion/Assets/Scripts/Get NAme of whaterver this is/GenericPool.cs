using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class GenericPool<T> where T : Component
{
    private GameObject _prefab;
    private Transform _prefabParent;
    private Stack<T> _pool;
    private HashSet<T> _activeInstances;

    public GenericPool(GameObject prefab, Transform prefabParent, int initialSize = 0)
    {
        _prefab = prefab;
        _prefabParent = prefabParent;
        _pool = new(initialSize);
        _activeInstances = new();

        for(int i = 0 ; i < initialSize ; i++)
        {
            CreateInstance();
        }
    }

    private T CreateInstance()
    {
        GameObject obj = Object.Instantiate(_prefab,_prefabParent);
        T component = obj.GetComponent<T>();
        obj.SetActive(false);
        _pool.Push(component);
        return component;
    }

    public T Get()
    {
        T instance;

        if(_pool.Count > 0)
        {
            instance = _pool.Pop();
        }
        else
        {
            instance = CreateInstance();
        }

        instance.gameObject.SetActive(true);
        _activeInstances.Add(instance);
        return instance;
    }

    public void Return(T instance)
    {
        if(instance == null) return;

        instance.gameObject.SetActive(false);
        _activeInstances.Remove(instance);
        _pool.Push(instance);
    }

    public void ReturnAll()
    {
        foreach(T instance in _activeInstances)
        {
            instance.gameObject.SetActive(false);
            _pool.Push(instance);
        }
        _activeInstances.Clear();;
    }

    public int ActiveCount => _activeInstances.Count;
    public int PooledCount => _pool.Count;
}
