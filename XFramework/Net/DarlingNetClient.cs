using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using Newtonsoft.Json;
using UnityEngine;
using XLua;

namespace DarlingEngine.Net
{
    public enum NetWorkState
    {
        START,
        CONNECTING,
        CONNECTED,
        DISCONNECTED,
        ERROR
    }
    
    public class DarlingNetClient
    {
        // 网络变化事件
        private event Action<NetWorkState> NetWorkStateChangedEvent;

        // 服务器消息事件
        private readonly ServerMessageEventManager serverMessageEventManager = new();

        private NetWorkState currentNetWorkState = NetWorkState.START;

        private Socket socket;

        private bool disposed = false;

        public DarlingNetClient()
        {

        }

        // Lua 用来判断是否需要发起重连
        public bool NeedReconnect
        {
            get
            {
                return currentNetWorkState switch
                {
                    NetWorkState.DISCONNECTED or NetWorkState.ERROR => true,
                    NetWorkState.START or NetWorkState.CONNECTING or NetWorkState.CONNECTED => false,
                    _ => false,
                };
            }
        }

        private bool NetworkAvailable()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;
            }
            return true;
        }

        // 被 Lua 调用来初始化
        public void StartConnect(string ip, int port)
        {
            disposed = false;

            if (!NetworkAvailable())
            {
                NetWorkChanged(NetWorkState.DISCONNECTED);
                return;
            }

            NetWorkChanged(NetWorkState.CONNECTING);

            IPAddress ipAddress = IPAddress.Parse(ip);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ie = new(ipAddress, port);

            socket.BeginConnect(ie, OnSocketConnected, socket);
        }

        private void OnSocketConnected(IAsyncResult result)
        {
            try
            {
                socket.EndConnect(result);
                NetWorkChanged(NetWorkState.CONNECTED);
                StartIOLoop();
            }
            catch (SocketException)
            {
                NetWorkChanged(NetWorkState.ERROR);
                Dispose();
            }
        }

        public void Disconnect()
        {
            Dispose();
            NetWorkChanged(NetWorkState.DISCONNECTED);
        }

        private void NetWorkChanged(NetWorkState state)
        {
            currentNetWorkState = state;
            NetWorkStateChangedEvent?.Invoke(state);
        }

        public void Dispose()
        {
            if (disposed)
                return;
            readState = ReadState.CLOSED;
            StopHeartbeatLoop();
            if (socket != null)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                finally
                {
                    socket.Close();
                    socket = null;
                }
            }
            receivedMessages.Clear();

            serverMessageEventManager.Clear();

            disposed = true;
        }

        public void StartIOLoop()
        {
            readState = ReadState.READ_HEAD;
            messageBufferOffset = 0;
            ReadLoop();
            SendHandshake(); // 建立连接后由客户端主动发起握手
        }

        #region IOLoop
        class StateObject
        {
            public const int BufferSize = 1024;
            internal byte[] buffer = new byte[BufferSize];
        }
        private readonly StateObject stateObject = new();
        private enum ReadState
        {
            READ_HEAD,
            READ_BODY,
            CLOSED,
        }
        private ReadState readState;
        private readonly byte[] headBuffer = new byte[PacketProtocol.HEADER_LENGTH];
        private byte[] messageBuffer;

        private int messageBufferOffset;

        private int bodyLength = 0;
        private void ReadLoop()
        {
            socket.BeginReceive(stateObject.buffer, 0, stateObject.buffer.Length, SocketFlags.None, OnReceived, stateObject);
        }
        private void OnReceived(IAsyncResult asyncReceive)
        {
            if (readState == ReadState.CLOSED)
                return;
            StateObject state = (StateObject)asyncReceive.AsyncState;

            try
            {
                int newReceivedLength = socket.EndReceive(asyncReceive);

                if (newReceivedLength > 0)
                {
                    ProcessBytes(state.buffer, 0, newReceivedLength);
                    //Receive next message
                    if (readState != ReadState.CLOSED) ReadLoop();
                }
                else
                {
                    Disconnect();
                }

            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("OnReceived error: " + e.ToString());
                NetWorkChanged(NetWorkState.ERROR);
                Dispose();
            }
        }

        // 处理 bytes 的 [offset, limit) 区间的数据, limit 是收到的数据总长度
        private void ProcessBytes(byte[] bytes, int offset, int limit)
        {
            if (readState == ReadState.READ_HEAD)
            {
                ReadHead(bytes, offset, limit);
            }
            else if (readState == ReadState.READ_BODY)
            {
                ReadBody(bytes, offset, limit);
            }
        }

        private void ReadHead(byte[] bytes, int offset, int limit)
        {
            int length = limit - offset;
            int numToReachCompleteHead = PacketProtocol.HEADER_LENGTH - messageBufferOffset;

            if (length >= numToReachCompleteHead)
            {
                // 数据长度达到了完整的 head 长度, 把 Head部分数据读出来
                Array.Copy(bytes, offset, headBuffer, messageBufferOffset, numToReachCompleteHead);

                // head 的后三个 byte 表示 body 数据长度
                bodyLength = (headBuffer[1] << 16) + (headBuffer[2] << 8) + headBuffer[3];

                // 准备好读取 message 的空间, message = head + body
                messageBuffer = new byte[PacketProtocol.HEADER_LENGTH + bodyLength];
                Array.Copy(headBuffer, 0, messageBuffer, 0, PacketProtocol.HEADER_LENGTH);
                offset += numToReachCompleteHead;
                messageBufferOffset = PacketProtocol.HEADER_LENGTH;
                readState = ReadState.READ_BODY;

                if (offset <= limit) ProcessBytes(bytes, offset, limit);
            }
            else
            {
                // 数据长度不够得到完整的 Head, 先把数据拷贝到 headBuffer 中
                Array.Copy(bytes, offset, headBuffer, messageBufferOffset, length);
                messageBufferOffset += length;
            }
        }

        private void ReadBody(byte[] bytes, int offset, int limit)
        {
            int length = bodyLength + PacketProtocol.HEADER_LENGTH - messageBufferOffset; // 距离完整body还需要多少byte
            if ((offset + length) <= limit)
            {
                // 收到的数据足够拼接出来 body 部分

                Array.Copy(bytes, offset, messageBuffer, messageBufferOffset, length);
                offset += length;

                // 处理完整的 message
                HandleRawMessageBytes(messageBuffer);

                // 重置状态
                messageBufferOffset = 0;
                bodyLength = 0;

                if (readState != ReadState.CLOSED)
                    readState = ReadState.READ_HEAD;
                if (offset < limit)
                    ProcessBytes(bytes, offset, limit);
            }
            else
            {
                Array.Copy(bytes, offset, messageBuffer, messageBufferOffset, limit - offset);
                messageBufferOffset += limit - offset;
                readState = ReadState.READ_BODY;
            }
        }

        private void SendBytes(byte[] data)
        {
            try
            {
                _ = socket.Send(data);
            }
            catch (SocketException)
            {
                // An error occurred when attempting to access the socket.
                NetWorkChanged(NetWorkState.ERROR);
                Dispose();
            }
            catch (ObjectDisposedException)
            {
                // The Socket has been closed.
                NetWorkChanged(NetWorkState.ERROR);
                Dispose();
            }
        }

        private MessageProtocol messageProtocol;
        private void SendMessage(MessageType messageType, ulong messageId, string route, byte[] data)
        {
            var newMessage = new Message(messageType, messageId, route, data);
            newMessage.Encrypt();
            var bytes = messageProtocol.Encode(newMessage);
            SendBytes(PacketProtocol.Encode(new Packet(PacketType.DATA, bytes)));
        }

        public void SendRequest(ulong messageId, string route, byte[] data, Function responseCallback)
        {
            serverMessageEventManager.RegisterResponseHandler(messageId, responseCallback);
            SendMessage(MessageType.REQUEST, messageId, route, data);
        }

        public void SendNotify(string route, byte[] data)
        {
            SendMessage(MessageType.NOTIFY, 0, route, data);
        }

        public void SubscribeServerPush(string route, Function pushCallback)
        {
            serverMessageEventManager.RegisterPushHandler(route, pushCallback);
        }

        #endregion

        #region 消息处理
        private void HandleRawMessageBytes(byte[] data)
        {
            Packet packet = PacketProtocol.Decode(data);
            switch (packet.Type)
            {
                case PacketType.HANDSHAKE:
                    OnReceivedHandshake(packet);
                    break;
                case PacketType.HANDSHAKE_ACK:
                    // 服务器不应该发送这个消息，如果收到了，说明有问题
                    UnityEngine.Debug.LogError("unexpected packet type received: " + packet.Type);
                    break;
                case PacketType.HEARTBEAT:
                    OnReceivedHeartbeat(packet);
                    break;
                case PacketType.DATA:
                    OnReceivedDataMessage(packet);
                    break;
                case PacketType.KICK:
                    OnReceivedKick(packet);
                    break;
                default:
                    UnityEngine.Debug.LogError("unexpected packet type received: " + packet.Type);
                    break;
            }
        }

        // 被服务器踢下线
        private void OnReceivedKick(Packet packet)
        {
            UnityEngine.Debug.Log("received kick message from server, disconnect");
            Disconnect();
        }

        private readonly Queue<Message> receivedMessages = new Queue<Message>();
        // 处理 Data 类型的数据
        private void OnReceivedDataMessage(Packet packet)
        {
            var message = messageProtocol.Decode(packet.Body);
            if (message == null)
            {
                UnityEngine.Debug.LogError("data packet message format error");
                return;
            }
            if (message.Type != MessageType.RESPONSE && message.Type != MessageType.PUSH)
            {
                UnityEngine.Debug.LogError("received unexpected message type from server: " + message.Type);
            }

            message.Decrypt();

            receivedMessages.Enqueue(message);
        }

        // 消息处理循环
        public void Update(float dt)
        {
            //检测网络连接
            if (!NetworkAvailable())
            {
                Disconnect();
                return;
            }

            if (receivedMessages == null) return;
            if (readState == ReadState.CLOSED) return;
            while (receivedMessages.Count > 0)
            {
                var message = receivedMessages.Dequeue();
                try
                {
                    if (message.Type == MessageType.RESPONSE)
                    {
                        HandleResponseMessage(message);
                    }
                    else if (message.Type == MessageType.PUSH)
                    {
                        HandlePushMessage(message);
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("unexpected message type in queue: " + message.Type);
                    }
                }
                catch (Exception e)
                {
                    // 回调函数中可能会抛出异常，这里捕获一下，避免影响后续消息的处理
                    UnityEngine.Debug.LogError("handle message error: " + e);
                }
            }
        }

        private void HandleResponseMessage(Message message)
        {
            serverMessageEventManager.OnResponseMessage(message.Id, message);
        }

        private void HandlePushMessage(Message message)
        {
            serverMessageEventManager.OnPushMessage(message.Route, message);
        }

        #endregion

        #region 握手
        // 当前客户端发出的 handshake 没有携带额外信息，将来可以带上客户端的版本号之类的，服务器拒绝老版本的握手并提示升级客户端
        private readonly byte[] handshakeBytes = PacketProtocol.Encode(new Packet(PacketType.HANDSHAKE, null));
        private readonly byte[] handshakeAckBytes = PacketProtocol.Encode(new Packet(PacketType.HANDSHAKE_ACK, null));
        private void SendHandshake()
        {
            SendBytes(handshakeBytes);
        }
        private void SendHandshakeAck()
        {
            SendBytes(handshakeAckBytes);
        }
        [Serializable]
        public struct Sys
        {
            public int heartbeat;
            public ulong servertime;
            public Dictionary<string, ushort> router;
        }
        [Serializable]
        public struct HandshakeAppendData
        {
            public int code;
            public Sys sys;
        }

        private Function OnHandshakeFinished;
        public void SetOnHandshakeFinished(Function hook)
        {
            OnHandshakeFinished = hook;
        }
        // 收到服务器握手消息
        private void OnReceivedHandshake(Packet handshakePacket)
        {
            // TODO: decode json in handshakePacket.Body
            var jsonStr = Encoding.UTF8.GetString(handshakePacket.Body);
            if (jsonStr == null || jsonStr.Length == 0)
            {
                UnityEngine.Debug.LogError("invalid handshake packet body, json string is empty");
                return;
            }
            HandshakeAppendData data = JsonConvert.DeserializeObject<HandshakeAppendData>(jsonStr);
            if (data.code != 200)
            {
                UnityEngine.Debug.LogError("server handshake failed, code: " + data.code);
                return;
            }
            int heartBeatIntervalSeconds = data.sys.heartbeat;
            SetHeartbeatIntervalSeconds(heartBeatIntervalSeconds);
            ulong serverTime = data.sys.servertime; // 服务器时间，暂时未使用
            var routerCompressDict = data.sys.router;
            messageProtocol = new MessageProtocol(routerCompressDict);

            SendHandshakeAck(); // 发送握手ACK回应, 完成握手

            StartHeartbeatLoop(); // 开始心跳

            OnHandshakeFinished?.Invoke();
        }
        #endregion

        #region 心跳

        private readonly byte[] heartbeatBytes = PacketProtocol.Encode(new Packet(PacketType.HEARTBEAT, null));
        private const int DEFAULT_HEARTBEAT_INTERVAL_SECONDS = 10;
        private int heartbeatIntervalMilliSeconds = DEFAULT_HEARTBEAT_INTERVAL_SECONDS * 1000;
        private DateTime lastServerHeartbeatAt;
        private DateTime lastSentHeartbeatAt;
        private void SetHeartbeatIntervalSeconds(int intervalSeconds)
        {
            if (intervalSeconds < DEFAULT_HEARTBEAT_INTERVAL_SECONDS)
            {
                Debug.LogWarning("heartbeat interval seconds too small, use default value: " + DEFAULT_HEARTBEAT_INTERVAL_SECONDS);
                return;
            }
            heartbeatIntervalMilliSeconds = intervalSeconds * 1000;
        }
        private void SendHeartbeat(object source, ElapsedEventArgs e)
        {
            TimeSpan span = DateTime.Now - lastSentHeartbeatAt;
            if ((int)span.TotalMilliseconds > heartbeatIntervalMilliSeconds * 2)
            {
                // 心跳超时，断开连接
                Debug.LogWarning("Heartbeat timeout, disconnect");
                Disconnect();
                return;
            }

            SendBytes(heartbeatBytes);
            lastSentHeartbeatAt = DateTime.Now;
        }

        // 收到服务器心跳
        private void OnReceivedHeartbeat(Packet packet)
        {
            UnityEngine.Debug.Log("received server heartbeat");
            lastServerHeartbeatAt = DateTime.Now;
        }
        private Timer heartbeatTimer;
        private void StartHeartbeatLoop()
        {
            if (heartbeatIntervalMilliSeconds < 1000) return;

            //start hearbeat
            heartbeatTimer = new Timer
            {
                Interval = heartbeatIntervalMilliSeconds
            };
            heartbeatTimer.Elapsed += new ElapsedEventHandler(SendHeartbeat);
            heartbeatTimer.Enabled = true;

            lastSentHeartbeatAt = DateTime.Now;
        }

        public void StopHeartbeatLoop()
        {
            if (heartbeatTimer != null)
            {
                heartbeatTimer.Enabled = false;
                heartbeatTimer.Dispose();
            }
        }

        #endregion
    }
}