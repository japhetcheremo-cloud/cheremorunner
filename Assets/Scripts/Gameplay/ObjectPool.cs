using UnityEngine;
using System.Collections.Generic;

namespace CheremoRunner.Gameplay
{
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }

        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        [SerializeField] private List<Pool> pools = new List<Pool>();
        private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<string, Pool> poolTemplates = new Dictionary<string, Pool>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }
        }

        public void InitializePool(string tag, GameObject prefab, int size)
        {
            if (poolDictionary.ContainsKey(tag)) return;

            Queue<GameObject> objectPool = new Queue<GameObject>();
            Pool pool = new Pool { tag = tag, prefab = prefab, size = size };
            poolTemplates[tag] = pool;

            for (int i = 0; i < size; i++)
            {
                GameObject obj = Instantiate(prefab, transform);
                obj.name = tag + "_" + i;
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary[tag] = objectPool;
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("ObjectPool: Tag not found: " + tag);
                return null;
            }

            Queue<GameObject> queue = poolDictionary[tag];

            GameObject objectToSpawn;

            if (queue.Count > 0)
            {
                objectToSpawn = queue.Dequeue();
            }
            else
            {
                // Expand pool
                if (poolTemplates.ContainsKey(tag))
                {
                    objectToSpawn = Instantiate(poolTemplates[tag].prefab, transform);
                    objectToSpawn.name = tag + "_expanded";
                }
                else
                {
                    Debug.LogWarning("ObjectPool: No template for tag: " + tag);
                    return null;
                }
            }

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            IPoolable poolable = objectToSpawn.GetComponent<IPoolable>();
            poolable?.OnSpawnFromPool();

            return objectToSpawn;
        }

        public void ReturnToPool(string tag, GameObject obj)
        {
            if (obj == null) return;

            IPoolable poolable = obj.GetComponent<IPoolable>();
            poolable?.OnReturnToPool();

            obj.SetActive(false);

            if (!poolDictionary.ContainsKey(tag))
            {
                poolDictionary[tag] = new Queue<GameObject>();
            }

            poolDictionary[tag].Enqueue(obj);
        }

        public void ReturnAllActive(string tag)
        {
            if (!poolDictionary.ContainsKey(tag)) return;
            // Find all active objects with this tag pattern
            foreach (Transform child in transform)
            {
                if (child.name.StartsWith(tag) && child.gameObject.activeSelf)
                {
                    ReturnToPool(tag, child.gameObject);
                }
            }
        }
    }

    public interface IPoolable
    {
        void OnSpawnFromPool();
        void OnReturnToPool();
    }
}
