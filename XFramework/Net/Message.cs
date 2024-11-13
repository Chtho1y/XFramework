
using System.Collections.Generic;
using System;
using System.Text;
using System.Diagnostics;

namespace DarlingEngine.Net
{
    public enum MessageType
    {
        REQUEST = 0,
        NOTIFY = 1,
        RESPONSE = 2,
        PUSH = 3
    }

    public class Message
    {
        private MessageType type;
        private string route;
        private ulong id;
        private readonly byte[] data;

        public MessageType Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Route
        {
            get { return route; }
            set { route = value; }
        }

        public ulong Id
        {
            get { return id; }
            set { id = value; }
        }

        public byte[] Data
        {
            get { return data; }
        }

        public Message(MessageType type, ulong id, string route, byte[] data)
        {
            this.type = type;
            this.id = id;
            this.route = route;
            this.data = data;
        }

        #region 加解密Data部分
        private static readonly byte[] secretBytes = Encoding.UTF8.GetBytes("dCbuvN@XSEVG!lKYsRWDU%$g");

        private static byte[] Xor(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= secretBytes[i % secretBytes.Length];
            }
            return data;
        }

        public void Encrypt()
        {
            Xor(data);
        }

        public void Decrypt()
        {
            Xor(data);
        }
        #endregion
    }

    public class MessageProtocol
    {
        // 路径压缩表, 在握手阶段由服务器下发
        private Dictionary<string, ushort> routeCompressDict = new();
        private Dictionary<ushort, string> routeDecompressDict = new();

        public const int MSG_ROUTE_LENGTH_LIMIT = 255;

        public const int MSG_ROUTE_MASK = 0x01;
        public const int MSG_TYPE_MASK = 0x07;

        public MessageProtocol(Dictionary<string, ushort> dict)
        {
            if (dict != null)
            {
                routeCompressDict = dict;
                foreach (var item in dict)
                {
                    if (routeDecompressDict.ContainsKey(item.Value))
                    {
                        UnityEngine.Debug.LogError("duplicate route id in handshake: " + item.Value);
                        continue;
                    }
                    routeDecompressDict.Add(item.Value, item.Key);
                }
            }
        }

        private void WriteMessageId(byte[] bytes, ref int index, ulong messageId)
        {
            ulong n = messageId;
            do
            {
                ulong tmp = n % 128;
                ulong next = n >> 7;
                if (next != 0)
                {
                    tmp = tmp + 128;
                }
                bytes[index++] = Convert.ToByte(tmp);
                n = next;
            } while (n != 0);
        }

        private void WriteCompressedRoute(byte[] bytes, ref int index, ushort routeId)
        {
            bytes[index++] = (byte)(routeId >> 8 & 0xff);
            bytes[index++] = (byte)(routeId & 0xff);
        }

        private void WriteRoute(byte[] bytes, ref int index, int routeLength, byte[] routeBytes)
        {
            bytes[index++] = (byte)(routeLength & 0xff);
            Array.Copy(routeBytes, 0, bytes, index, routeLength);
            index += routeLength;
        }

        public byte[] Encode(Message msg)
        {
            if (msg.Type == MessageType.PUSH || msg.Type == MessageType.RESPONSE)
            {
                // 不要在客户端编码PUSH和RESPONSE类型的消息，这是服务端的消息格式
                throw new Exception("client shouldnt encode message type: " + msg.Type);
            }
            byte[] routeBytes = Encoding.UTF8.GetBytes(msg.Route);
            int routeLength = routeBytes.Length;
            if (routeLength > MSG_ROUTE_LENGTH_LIMIT)
            {
                throw new Exception("route is too long");
            }

            // Encode head
            byte flag = 0;
            // 最多的时候 flag(1 byte) + messageId(5 bytes) + routeLength(1 byte) + route(255 bytes)
            byte[] head = new byte[1 + 5 + 1 + routeLength];
            int offset = 1;

            // encode message id if exist
            if (msg.Type == MessageType.REQUEST)
            {
                // Request: flag,message id,route
                flag |= ((byte)MessageType.REQUEST) << 1;
                WriteMessageId(head, ref offset, msg.Id);

            }
            else if (msg.Type == MessageType.PUSH)
            {
                // Push: flag, route
                flag |= ((byte)MessageType.PUSH) << 1;
            }
            else
            {
                throw new Exception("client shouldnt encode message type: " + msg.Type);
            }

            // encode route
            if (routeCompressDict.ContainsKey(msg.Route))
            {
                // route compressed
                flag |= MSG_ROUTE_MASK;
                ushort routeId = routeCompressDict[msg.Route];

                WriteCompressedRoute(head, ref offset, routeId);
            }
            else
            {
                // route not compressed
                WriteRoute(head, ref offset, routeLength, routeBytes);
            }

            head[0] = flag;

            var bodyLength = 0;
            if (msg.Data != null)
            {
                bodyLength = msg.Data.Length;
            }
            byte[] result = new byte[offset + bodyLength];
            Array.Copy(head, 0, result, 0, offset);

            if (msg.Data != null && msg.Data.Length > 0)
            {
                Array.Copy(msg.Data, 0, result, offset, bodyLength);
            }
            return result;
        }

        private void ReadMessageId(byte[] buffer, ref int index, out ulong messageId)
        {
            ulong id = 0;
            int i = 0;
            do
            {
                ulong b = Convert.ToUInt32(buffer[index++]);
                id = id + (b & 0x7F) << (7 * i);
                i++;
                if (b < 128) break;
            } while (index < buffer.Length);

            messageId = id;
        }

        private void ReadCompressedRoute(byte[] buffer, ref int index, out ushort routeId)
        {
            ushort id = (ushort)((buffer[index++] << 8) | buffer[index++]);
            routeId = id;
        }

        private void ReadRoute(byte[] buffer, ref int index, out string route)
        {
            byte length = buffer[index++];
            byte[] bytes = new byte[length];
            Array.Copy(buffer, index, bytes, 0, length);
            route = Encoding.UTF8.GetString(bytes);
            index += length;
        }

        public Message Decode(byte[] buffer)
        {
            byte flag = buffer[0];

            var messageType = (MessageType)((flag >> 1) & MSG_TYPE_MASK);
            if (messageType == MessageType.NOTIFY || messageType == MessageType.REQUEST)
            {
                // 这些是不应该出现的来自服务器的消息类型
                throw new Exception("received unexpected message type from server: " + messageType);
            }

            int offset = 1;
            ulong messageId = 0;
            string route = "";

            if (messageType == MessageType.RESPONSE)
            {
                // Response: Flag + MessageId + Route
                ReadMessageId(buffer, ref offset, out messageId);
                if (messageId <= 0)
                {
                    UnityEngine.Debug.LogError("invalid messageId: " + messageId);
                    return null;
                }
            }
            else if (messageType == MessageType.PUSH)
            {
                // Push: Flag + Route
                if ((flag & MSG_ROUTE_MASK) == 1)
                {
                    // compressed route
                    ReadCompressedRoute(buffer, ref offset, out ushort routeId);
                    if (routeDecompressDict.ContainsKey(routeId))
                    {
                        route = routeDecompressDict[routeId];
                    }
                    else
                    {
                        throw new Exception("received unexpected routeId from server:" + routeId);
                    }
                }
                else
                {
                    // uncompressed route
                    ReadRoute(buffer, ref offset, out route);
                }
            }
            else
            {
                throw new Exception("received unexpected message type from server: " + messageType);
            }

            // body
            var bodyLength = buffer.Length - offset;
            byte[] data = new byte[bodyLength];

            Array.Copy(buffer, offset, data, 0, bodyLength);

            return new Message(messageType, messageId, route, data);
        }
    }


}