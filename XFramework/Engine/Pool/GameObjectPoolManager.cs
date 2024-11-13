using System.Collections.Generic;
using UnityEngine;


namespace XEngine.Engine
{
    public static class GameObjectPoolManager
    {
        private static Dictionary<string, GameObjectPool> pools = new();

        public static bool Init(string key, GameObject objPrefab, int initSize = 10)
        {
            if (!pools.ContainsKey(key))
            {
                var pool = new GameObjectPool(objPrefab, initSize);
                pools[key] = pool;
                return true;
            }
            return false;
        }

        public static GameObject GetObj(string key, Transform parentTransform = null,
        bool resetTransform = true, bool resetRectTransform = false)
        {
            if (pools.TryGetValue(key, out var pool))
            {
                var obj = pool.Get(parentTransform);
                if (resetTransform)
                {
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localRotation = Quaternion.identity;
                    obj.transform.localScale = Vector3.one;
                }
                if (resetRectTransform)
                {
                    var rectTransform = obj.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.localPosition = Vector3.zero;
                        rectTransform.localRotation = Quaternion.identity;
                        rectTransform.localScale = Vector3.one;
                        rectTransform.anchoredPosition = Vector2.zero;
                        rectTransform.sizeDelta = Vector2.zero;
                    }
                }
                return obj;
            }
            return null;
        }

        public static bool RecycleObj(string key, GameObject obj, Transform parent = null)
        {
            if (pools.TryGetValue(key, out var pool))
            {
                pool.Recycle(obj);
                if (parent != null) obj.transform.SetParent(parent);
                return true;
            }
            return false;
        }

        public static bool RecycleAllObjs(string key)
        {
            if (pools.TryGetValue(key, out var pool))
            {
                pool.RecycleAll();
                return true;
            }
            return false;
        }
    }
}