using System.Collections.Generic;
using System.Diagnostics;


namespace DarlingEngine.Net
{
    public delegate void Function(params object[] data);
    public class ServerMessageEventManager
    {
        private readonly Dictionary<ulong, Function> responseEvents = new();

        private readonly Dictionary<string, Function> pushEvents = new();

        public void RegisterResponseHandler(ulong messageId, Function handler)
        {
            if (!responseEvents.ContainsKey(messageId))
            {
                responseEvents.Add(messageId, handler);
            }
            else
            {
                UnityEngine.Debug.LogError("register message id is duplicate: " + messageId);
            }
        }

        public void RemoveResponseHandler(ulong messageId)
        {
            if (responseEvents.ContainsKey(messageId))
            {
                responseEvents.Remove(messageId);
            }
            else
            {
                UnityEngine.Debug.LogError("handler not exist when remove message id: " + messageId);
            }
        }

        public void RegisterPushHandler(string pushRouter, Function handler)
        {
            if (!pushEvents.ContainsKey(pushRouter))
            {
                pushEvents.Add(pushRouter, handler);
            }
            else
            {
                UnityEngine.Debug.LogError("register push router is duplicate: " + pushRouter);
            }
        }

        public void RemovePushHandler(string pushRouter)
        {
            if (pushEvents.ContainsKey(pushRouter))
            {
                pushEvents.Remove(pushRouter);
            }
            else
            {
                UnityEngine.Debug.LogError("handler not exist when remove push handler: " + pushRouter);
            }
        }

        public void Clear()
        {
            responseEvents.Clear();
            pushEvents.Clear();
        }

        public void OnResponseMessage(ulong messageId, params object[] data)
        {
            if (responseEvents.TryGetValue(messageId, out var handler))
            {
                handler?.Invoke(data);
                responseEvents.Remove(messageId);
            }
            else
            {
                UnityEngine.Debug.LogError($"handler not exist when handle message id: {messageId}, maybe duplicate message id");
            }
        }

        public void OnPushMessage(string pushRouter, params object[] data)
        {
            if (pushEvents.TryGetValue(pushRouter, out var handler))
            {
                handler?.Invoke(data);
            }
            else
            {
                UnityEngine.Debug.LogError("handler not exist when received push message: " + pushRouter);
            }
        }
    }
}