using System.Collections.Generic;
using Interfaces;
using Managers;
using UnityEngine;

namespace Pools
{
    public class MonoPool<T> : MonoSingleton<MonoPool<T>> where T : MonoBehaviour , IPoolable
    {
        private Stack<T> _available;
        [SerializeField] private T prefab;
        [SerializeField] protected Transform parent;
        [SerializeField] private int initialPoolSize = 10;
        private int _poolSize;

        private void Awake()
        {
            _available = new Stack<T>();
            _poolSize = initialPoolSize;
            AddItemsToPool();
        }

        
        public virtual T Get()
        {
            if (_available.Count == 0)
            {
                AddItemsToPool();
            }
            var obj = _available.Pop();
            obj.gameObject.SetActive(true);
            obj.Reset();
            return obj;
        }
        
        
        
        public virtual void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            _available.Push(obj);
        }
        
        
        private void AddItemsToPool()
        {
            for (int i = 0; i < _poolSize; i++)
            {
                var obj = Instantiate(prefab, parent,true);
                obj.gameObject.SetActive(false);
                _available.Push(obj);
            }
        }
    }
}