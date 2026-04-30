using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonMonoBehaviour<PoolManager>
{
    [System.Serializable]
    public struct Pool
    {
        public uint Id;
        public int PoolSize;
        public GameObject Prefab;
        public Transform ParentTransform { get; set; }
    }

    [SerializeField] private List<Pool> _pools;

    private Dictionary<uint, Queue<GameObject>> _poolDictionary;

    protected override void Awake()
    {
        base.Awake();

        _poolDictionary = new();

        for (int i = 0; i < _pools.Count; ++i)
        {
            CreatePool(_pools[i]);
        }
    }

    private void CreatePool(Pool pool)
    {
        uint poolKey = pool.Id;

        GameObject parentObject = new GameObject(pool.Prefab.name + " Pool");
        parentObject.transform.SetParent(transform);
        pool.ParentTransform = parentObject.transform;

        if (!_poolDictionary.ContainsKey(poolKey))
        {
            _poolDictionary.Add(poolKey, new Queue<GameObject>());

            for (int i = 0; i < pool.PoolSize; ++i)
            {
                GameObject poolObject = Instantiate(pool.Prefab, parentObject.transform);

                poolObject.SetActive(false);

                _poolDictionary[poolKey].Enqueue(poolObject);
            }
        }
        else
        {
            Debug.LogError(poolKey + " Duplicate Pool");
        }
    }

    public GameObject GetObject(uint id, Transform parentTransform = null, bool isChangeParent = false)
    {
        if (!_poolDictionary.ContainsKey(id))
            return null;

        GameObject obj;

        if (_poolDictionary[id].Count == 0)
        {
            Pool pool = _pools.Find(p => p.Id == id);
            obj = Instantiate(pool.Prefab);
        }
        else
        {
            obj = _poolDictionary[id].Dequeue();
        }


        if (isChangeParent)
        {
            obj.transform.SetParent(parentTransform, false);
            obj.transform.localScale = Vector3.one;
        }
        else if(parentTransform != null)
        {
            obj.transform.position = parentTransform.position;
            obj.transform.rotation = parentTransform.rotation;
        }

        obj.SetActive(true);

        return obj;
    }

    public void ReturnObject(uint id, GameObject obj)
    {
        uint poolKey = id;

        if (_poolDictionary.ContainsKey(id))
        {
            _poolDictionary[id].Enqueue(obj);
            obj.SetActive(false);
        }
        else
        {
            Debug.LogError(poolKey + " key does not exist");
        }
    }

}