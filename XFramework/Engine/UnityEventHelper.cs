using System.Collections.Generic;
using UnityEngine;


namespace XEngine.Engine
{
    public delegate void Function(params object[] data);

    public static class UnityEventHelper
    {
        private static readonly Dictionary<object, Dictionary<string, Function>> objectEvents = new();

        private static readonly Dictionary<string, Function> globalEvents = new();

        public static void RegisterEvent(object owner, string eventName, Function handler)
        {
            if (!objectEvents.TryGetValue(owner, out var eventsOfOwner))
            {
                eventsOfOwner = new Dictionary<string, Function>();
                objectEvents.Add(owner, eventsOfOwner);
            }
            if (!eventsOfOwner.TryGetValue(eventName, out var _))
            {
                eventsOfOwner.Add(eventName, handler);
            }
            else
            {
                Debug.LogError("register event name is duplicate: " + eventName + "  " + owner);
            }
        }

        public static void RemoveEvent(object owner, string eventName)
        {
            if (objectEvents.TryGetValue(owner, out var value))
            {
                if (value.TryGetValue(eventName, out var _))
                {
                    value.Remove(eventName);
                }
                else
                {
                    Debug.LogError("handler not exist when remove event: " + eventName + "  " + owner);
                }
            }
        }

        // 清理 owner 注册的的所有 event handler
        public static void RemoveEventsOn(object owner)
        {
            if (objectEvents.TryGetValue(owner, out var _))
            {
                objectEvents.Remove(owner);
            }
        }

        // 触发 owner.eventName 的 handler
        internal static void SendEvent(object owner, string eventName, params object[] data)
        {
            if (objectEvents.TryGetValue(owner, out var eventsOnOwner) && eventsOnOwner.TryGetValue(eventName, out var handler))
            {
                handler?.Invoke(data);
            }
        }

        public static void Clear()
        {
            objectEvents.Clear();
            globalEvents.Clear();
        }


        public static void RegisterGlobalEvent(string eventName, Function handler)
        {
            if (!globalEvents.TryGetValue(eventName, out var _))
            {
                globalEvents.Add(eventName, handler);
            }
            else
            {
                Debug.LogError("register event name is duplicate: " + eventName);
            }
        }

        public static void RemoveGlobalEvent(string eventName)
        {
            if (globalEvents.TryGetValue(eventName, out var _))
            {
                globalEvents.Remove(eventName);
            }
            else
            {
                Debug.LogError("handler not exist when remove event: " + eventName);
            }
        }

        public static void SendGlobalEvent(string eventName, params object[] data)
        {
            if (globalEvents.TryGetValue(eventName, out var handler))
            {
                handler?.Invoke(data);
            }
        }
    }
}