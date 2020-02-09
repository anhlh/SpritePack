using System.Collections.Generic;
using UnityEngine;

namespace OriTool.GoPool
{
    public class GOPoolManager : MonoBehaviour
    {
        public class GameObjectItem<T> where T : Component
        {
            private readonly GameObject _prefab;
            private readonly Queue<T> _queue;
            private readonly Transform _parent;

            public GameObjectItem(GameObject prefab, Transform parent)
            {
                _prefab = prefab;
                _parent = parent;
                _queue = new Queue<T>();
            }

            public void CreateCopy(int num)
            {
                if (_instance == null) return;
                for (var i = 0; i < num; i++)
                {
                    var copy = Instantiate(_prefab, _parent);
                    copy.name = _prefab.name;
                    var comp = copy.GetComponent<T>();
                    _queue?.Enqueue(comp);
                }
            }

            public void Enqueue(T copy)
            {
                copy.transform.SetParent(_parent, false);
                _queue?.Enqueue(copy);
            }

            public T Dequeue()
            {
                if (_queue.Count == 0) CreateCopy(1);
                var o = _queue.Dequeue();
                o.transform.localScale = _prefab.transform.localScale;
                return o;
            }

            public void ClearCache()
            {
                while (_queue?.Count != 0)
                {
                    var obj = _queue?.Dequeue();
                    if (obj != null) Destroy(obj.gameObject);
                }
            }
        }

        private readonly Dictionary<string, object> _dict = new Dictionary<string, object>();
        private static GOPoolManager _instance;
        private const string DefaultString = "";

        private static GOPoolManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var o = new GameObject("PoolManager");
                o.gameObject.SetActive(false);
                _instance = o.AddComponent<GOPoolManager>();
                return _instance;
            }
        }

        /// <summary>
        /// Init
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <param name="numCache"></param>
        /// <param name="idForDifferencePrefabWithSameType"></param>
        public static void Init<T>(T prefab, int numCache,
            string idForDifferencePrefabWithSameType = DefaultString) where T : Component
        {
            if (!Instance.__IsInit<T>(idForDifferencePrefabWithSameType))
                Instance.__Init<T>(prefab.gameObject, numCache, idForDifferencePrefabWithSameType);
        }

        /// <summary>
        /// Check cache init or not
        /// </summary>
        /// <param name="idForDifferencePrefabWithSameType"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsInit<T>(string idForDifferencePrefabWithSameType = DefaultString) where T : Component =>
            Instance.__IsInit<T>(idForDifferencePrefabWithSameType);

        /// <summary>
        /// Get object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idForDifferencePrefabWithSameType"></param>
        /// <param name="showLogError"></param>
        /// <returns></returns>
        public static T Get<T>(string idForDifferencePrefabWithSameType = DefaultString, bool showLogError = true)
            where T : Component =>
            Instance.__Get<T>(idForDifferencePrefabWithSameType, showLogError);

        /// <summary>
        /// Get object and change parent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="idForDifferencePrefabWithSameType"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns></returns>
        public static T Get<T>(Transform parent, string idForDifferencePrefabWithSameType = DefaultString,
            bool worldPositionStays = false) where T : Component =>
            Instance.GetAndChangeParent<T>(parent, idForDifferencePrefabWithSameType, worldPositionStays);

        /// <summary>
        /// Return to cache
        /// </summary>
        /// <param name="copy"></param>
        /// <param name="idForDifferencePrefabWithSameType"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Put<T>(T copy, string idForDifferencePrefabWithSameType = DefaultString)
            where T : Component => Instance.__Put(copy, idForDifferencePrefabWithSameType);

        /// <summary>
        /// Clear all cache
        /// </summary>
        public static void ClearAllCache()
        {
            if (_instance == null) return;
            Destroy(_instance.gameObject);
        }

        /// <summary>
        /// Clear cache with type
        /// </summary>
        /// <param name="idForDifferencePrefabWithSameType"></param>
        /// <typeparam name="T"></typeparam>
        public void ClearCacheType<T>(string idForDifferencePrefabWithSameType = DefaultString) where T : Component
        {
            var key = typeof(T) + ":" + idForDifferencePrefabWithSameType;
            var objItem = GetObjectItem<T>(key);
            if (objItem == null)
            {
                Debug.LogError("Cache was not inited! Call InitCache first " + key);
                return;
            }

            objItem.ClearCache();
            _dict.Remove(key);
        }

        private void __Init<T>(GameObject prefab, int numCache,
            string idForDifferencePrefabWithSameType = DefaultString)
            where T : Component
        {
            var key = typeof(T) + ":" + idForDifferencePrefabWithSameType;
            var parent = new GameObject(key);
            parent.transform.parent = this.transform;
            parent.SetActive(false);

            var objItem = GetObjectItem<T>(key, prefab, parent.transform);
            objItem.CreateCopy(numCache);
            _dict[key] = objItem;
        }

        private bool __IsInit<T>(string idForDifferencePrefabWithSameType = DefaultString) where T : Component
        {
            var key = typeof(T) + ":" + idForDifferencePrefabWithSameType;
            var objItem = GetObjectItem<T>(key);
            return objItem != null;
        }

        private T __Get<T>(string idForDifferencePrefabWithSameType = DefaultString, bool showLogError = true)
            where T : Component
        {
            var key = typeof(T) + ":" + idForDifferencePrefabWithSameType;
            var objItem = GetObjectItem<T>(key);
            if (objItem != null) return objItem.Dequeue();
            if (showLogError) Debug.LogError("Cache was not inited! Call InitCache first " + key);
            return null;
        }

        private T GetAndChangeParent<T>(Transform parent, string idForDifferencePrefabWithSameType = DefaultString,
            bool worldPositionStays = false) where T : Component
        {
            var key = typeof(T) + ":" + idForDifferencePrefabWithSameType;
            var objItem = GetObjectItem<T>(key);
            if (objItem == null)
            {
                Debug.LogError("Cache was not inited! Call InitCache first");
                return null;
            }

            var item = objItem.Dequeue();
            item.transform.SetParent(parent, worldPositionStays);
            if (worldPositionStays) return item;

            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            return item;
        }

        private GameObjectItem<T> GetObjectItem<T>(string key, GameObject clone, Transform parent) where T : Component
        {
            GameObjectItem<T> objItem;
            if (_dict.ContainsKey(key))
                objItem = _dict[key] as GameObjectItem<T>;
            else
                objItem = new GameObjectItem<T>(clone, parent);

            return objItem;
        }

        private GameObjectItem<T> GetObjectItem<T>(string key) where T : Component
        {
            GameObjectItem<T> objItem = null;
            if (_dict.ContainsKey(key)) objItem = _dict[key] as GameObjectItem<T>;
            return objItem;
        }

        private bool __Put<T>(T copy, string idForDifferencePrefabWithSameType = DefaultString) where T : Component
        {
            var key = typeof(T) + ":" + idForDifferencePrefabWithSameType;
            var objItem = GetObjectItem<T>(key);
            if (objItem == null)
            {
                Debug.LogError("Cache was not inited! Call InitCache first " + key);
                return false;
            }

            objItem.Enqueue(copy);

            return true;
        }

        private void OnDestroy() => _instance = null;
    }
}