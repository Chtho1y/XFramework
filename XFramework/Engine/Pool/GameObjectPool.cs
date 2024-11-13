using System;
using System.Collections;
using System.Collections.Generic;
using XEngine.Engine;
using UnityEngine;


namespace XEngine.Engine
{
    public class GameObjectPool : ObjectPool<GameObject>
    {
        private GameObject prefab;
        private static readonly int MAX_POOL_SIZE = 10000;
        private static readonly int MAX_DESTROY_PER_FRAME = 10;
        private static Queue<GameObject> destructionQueue = new();
        private bool isDestroying = false;

        public GameObjectPool(GameObject prefab, int initSize)
            : base(initSize, null, CreateGameObject)
        {
            this.prefab = prefab;
        }

        private static GameObject CreateGameObject(object[] args)
        {
            var newPrefab = (GameObject)args[0];
            var newInstance = UnityEngine.Object.Instantiate(newPrefab);
            newInstance.SetActive(false);
            return newInstance;
        }

        protected override void RecycleAction(GameObject item)
        {
            item.SetActive(false);
            if (inUse.Count + stack.Count >= MAX_POOL_SIZE) destructionQueue.Enqueue(item);
            else base.Recycle(item);
            if (destructionQueue.Count > 0 && !isDestroying)
                GameManager.Instance.StartCoroutine(DestroyExtraObjects());
        }

        private IEnumerator DestroyExtraObjects()
        {
            isDestroying = true;
            var count = 0;
            while (destructionQueue.Count > 0 && count < MAX_DESTROY_PER_FRAME)
            {
                UnityEngine.Object.Destroy(destructionQueue.Dequeue());
                count++;
                if (count == MAX_DESTROY_PER_FRAME)
                {
                    yield return null;
                    count = 0;
                }
            }
            isDestroying = false;
        }

        public GameObject Get(Transform parentTransform = null)
        {
            var gameObject = base.Get(new object[] { prefab });
            gameObject.transform.SetParent(parentTransform, false);
            gameObject.SetActive(true);
            return gameObject;
        }

        public new void Recycle(GameObject obj)
        {
            if (obj == null) return; //throw new ArgumentNullException(nameof(obj));
            base.Recycle(obj);
        }

        public new void RecycleAll()
        {
            List<GameObject> toRecycle = new(inUse);
            foreach (var obj in toRecycle) Recycle(obj);
        }
    }
}