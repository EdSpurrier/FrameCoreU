using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace FrameCoreU.Pooling
{
    public class PoolCore : MonoBehaviour
    {
        [System.Serializable]
        public class PoolObject
        {
            [HideLabel]
            [HorizontalGroup("Split", 0.50f)]
            public Transform prefab;

            [HideLabel]
            [HorizontalGroup("Split", 0.50f)]
            [SuffixLabel("Amount", Overlay = true)]
            public int amount = 50;

            [FoldoutGroup("Pool Object")]
            public List<GameObject> objects = new();

            [FoldoutGroup("Pool Object")]
            public float lazyLoadTime = 0f;

            [FoldoutGroup("Pool Object")]
            public bool boosting = false;
        }

        [Title("Object Pool")]
        public List<PoolObject> poolObjects = new();

        [Title("Settings")]
        [HideLabel]
        [HorizontalGroup("Split", 0.50f)]
        [SuffixLabel("Minimum Amount", Overlay = true)]
        public int minimumPoolAmount = 5;

        [Title("")]
        [HideLabel]
        [HorizontalGroup("Split", 0.50f)]
        [SuffixLabel("Lazy Load Time", Overlay = true)]
        public float lazyLoadTime = 2f;

        [Title("System")]
        public float poolTime = 0f;

        public void AddToPool(Transform prefab)
        {
            if (ObjectInPool(prefab))
                return;

            poolObjects.Add(new PoolObject { prefab = prefab });
        }

        public bool ObjectInPool(Transform prefab) => FindEntry(prefab) != null;

        private void Awake()
        {
            CreateObjects();
        }

        private void Update()
        {
            poolTime += Time.deltaTime;
            BoostPoolCycle();
        }

        private void CreateObjects()
        {
            foreach (PoolObject poolObject in poolObjects)
            {
                if (poolObject.objects == null)
                    poolObject.objects = new List<GameObject>();

                for (int i = 0; i < poolObject.amount; i++)
                {
                    CreateSpawnableObject(poolObject.prefab, poolObject);
                }

                poolObject.boosting = false;
            }
        }

        public void BoostPoolCycle()
        {
            foreach (PoolObject poolObject in poolObjects)
            {
                if (!poolObject.boosting)
                    continue;

                if (poolObject.lazyLoadTime <= 0)
                {
                    CreateSpawnableObject(poolObject.prefab, poolObject);

                    if (poolObject.objects.Count >= poolObject.amount)
                    {
                        poolObject.boosting = false;
                    }
                    else
                    {
                        poolObject.lazyLoadTime = lazyLoadTime;
                    }
                }
                else
                {
                    poolObject.lazyLoadTime -= Time.deltaTime;
                }
            }
        }

        public void CreateSpawnableObject(Transform obj, PoolObject poolObject)
        {
            if (obj == null)
            {
                Debug.LogError("PoolCore [ERROR] >> Tried to create a pooled object from a null prefab (an empty entry in the pool list?).");
                return;
            }

            if (poolObject.objects == null)
                poolObject.objects = new List<GameObject>();

            Transform newObj = Instantiate(obj, Vector3.zero, Quaternion.identity);
            newObj.SetParent(transform);
            newObj.gameObject.SetActive(false);

            if (!newObj.TryGetComponent(out PooledObject pooled))
                pooled = newObj.gameObject.AddComponent<PooledObject>();
            pooled.Bind(this, poolObject);

            poolObject.objects.Add(newObj.gameObject);
        }

        public GameObject SpawnObject(Transform obj, Vector3 position, Quaternion rotation)
        {
            PoolObject poolObject = FindEntry(obj);
            if (poolObject == null)
            {
                Debug.LogError("PoolCore [ERROR] >> Prefab isn't in the pool - add it to the Pool's list: " + (obj != null ? obj.name : "null"));
                return null;
            }

            // Anything destroyed while sitting in the pool (e.g. by a scene script) can't be reused
            poolObject.objects.RemoveAll(pooledObject => pooledObject == null);

            if (poolObject.objects.Count < minimumPoolAmount)
            {
                CreateSpawnableObject(poolObject.prefab, poolObject);
                poolObject.boosting = true;
            }

            if (poolObject.objects.Count == 0)
            {
                Debug.LogError("PoolCore [ERROR] >> Pool exists but has no available objects: " + obj.name);
                return null;
            }

            // Take the most recently returned object first - it's the one most likely to still be warm
            int last = poolObject.objects.Count - 1;
            GameObject spawnedObject = poolObject.objects[last];
            poolObject.objects.RemoveAt(last);

            spawnedObject.transform.SetParent(null);
            spawnedObject.transform.SetPositionAndRotation(position, rotation);

            if (spawnedObject.TryGetComponent(out PooledObject pooled))
                pooled.MarkSpawned();

            spawnedObject.SetActive(true);
            NotifyPoolables(spawnedObject, spawned: true);

            return spawnedObject;
        }

        // Returns a spawned object to its pool entry. Objects that didn't come from a pool are destroyed instead,
        // so this is always safe to call.
        public void Despawn(GameObject obj)
        {
            if (obj == null)
                return;

            if (!obj.TryGetComponent(out PooledObject pooled) || pooled.Pool == null)
            {
                Destroy(obj);
                return;
            }

            if (pooled.Pool != this)
            {
                pooled.Pool.Despawn(obj);
                return;
            }

            // Already back in the pool - despawning twice would add it to the list twice
            if (!pooled.IsSpawned)
                return;

            pooled.MarkDespawned();
            NotifyPoolables(obj, spawned: false);

            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pooled.Entry.objects.Add(obj);
        }

        private readonly List<IPoolable> _poolables = new();

        private void NotifyPoolables(GameObject obj, bool spawned)
        {
            obj.GetComponentsInChildren(true, _poolables);

            foreach (IPoolable poolable in _poolables)
            {
                if (spawned) poolable.OnSpawned();
                else poolable.OnDespawned();
            }

            _poolables.Clear();
        }

        private PoolObject FindEntry(Transform prefab)
        {
            foreach (PoolObject poolObject in poolObjects)
            {
                if (poolObject.prefab == prefab)
                    return poolObject;
            }

            return null;
        }
    }
}