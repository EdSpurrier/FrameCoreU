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
            bool found = false;

            foreach (PoolObject poolObject in poolObjects)
            {
                if (prefab == poolObject.prefab)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                PoolObject poolObject = new PoolObject();
                poolObject.prefab = prefab;
                poolObjects.Add(poolObject);
            }
        }

        public bool ObjectInPool(Transform prefab)
        {
            foreach (PoolObject poolObject in poolObjects)
            {
                if (prefab == poolObject.prefab)
                    return true;
            }

            return false;
        }

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
                Debug.LogError("PoolCore [ERROR] >> Tried to create pooled object from a null prefab.");
                return;
            }

            if (poolObject.objects == null)
                poolObject.objects = new List<GameObject>();

            Transform newObj = Instantiate(obj, Vector3.zero, Quaternion.identity);
            newObj.SetParent(transform);
            newObj.gameObject.SetActive(false);

            poolObject.objects.Add(newObj.gameObject);
        }

        public GameObject SpawnObject(Transform obj, Vector3 position, Quaternion rotation)
        {
            foreach (PoolObject poolObject in poolObjects)
            {
                if (poolObject.prefab != obj)
                    continue;

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

                GameObject spawnedObject = poolObject.objects[0];
                poolObject.objects.RemoveAt(0);

                spawnedObject.transform.SetParent(null);
                spawnedObject.transform.position = position;
                spawnedObject.transform.rotation = rotation;
                spawnedObject.SetActive(true);

                return spawnedObject;
            }

            Debug.LogError("PoolCore [ERROR] >> No object found in pool: " + obj.name);
            Debug.Break();
            return null;
        }
    }
}