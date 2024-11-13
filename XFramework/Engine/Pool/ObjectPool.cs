using System;
using System.Collections.Generic;


namespace XEngine.Engine
{
    public class ObjectPool<T>
    {
        public readonly HashSet<T> inUse;
        public readonly Stack<T> stack;
        private readonly Func<object[], T> ctor;
        private readonly Action<T> onRecycle;
        private readonly object lockObj = new();
        private int size;

        public ObjectPool(int initSize = 10, Action<T> onRecycle = null, Func<object[], T> ctor = null)
        {
            stack = new Stack<T>(initSize);
            inUse = new HashSet<T>();
            size = initSize;
            this.onRecycle = onRecycle;
            this.ctor = ctor;
        }

        public T Get(params object[] args)
        {
            lock (lockObj)
            {
                T item;
                if (stack.Count == 0)
                {
                    item = ctor != null ? ctor(args) : Activator.CreateInstance<T>();
                    size++;
                }
                else item = stack.Pop();
                inUse.Add(item);
                return item;
            }
        }

        public bool Recycle(T item)
        {
            lock (lockObj)
            {
                if (item == null) return false;//throw new ArgumentNullException(nameof(item));
                if (!inUse.Remove(item)) return false;// throw new InvalidOperationException("Attempting to recycle an item that is not in use.");
                RecycleAction(item);
                onRecycle?.Invoke(item);
                if (stack.Count < size)
                    stack.Push(item);
                return true;
            }
        }

        public void RecycleAll()
        {
            lock (lockObj)
            {
                T[] items = new T[inUse.Count];
                inUse.CopyTo(items);
                foreach (var item in items)
                {
                    onRecycle?.Invoke(item);
                    if (stack.Count < size)
                        stack.Push(item);
                }
                inUse.Clear();
            }
        }

        protected virtual void RecycleAction(T item)
        {

        }
        public override string ToString()
        {
            lock (lockObj) return $"ObjPool: item=[{typeof(T)}], inUse=[{inUse.Count}], inPool=[{stack.Count}/{size}]";
        }
    }
}