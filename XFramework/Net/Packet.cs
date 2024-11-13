using System;


namespace DarlingEngine.Net
{
     public enum PacketType
     {
        HANDSHAKE = 1,
        HANDSHAKE_ACK = 2,
        HEARTBEAT = 3,
        DATA = 4,
        KICK = 5,
    }

    public class Packet
    {
        private PacketType type;
        private int bodyLength;
        private byte[] body;

        public PacketType Type {
            get { return type; }
            set { type = value; }
        }

        public int BodyLength {
            get {return bodyLength;}
        }

        public byte[] Body {
            get { return body; }
            set {
                body = value;
                if (value == null) {
                    bodyLength = 0;
                } else {
                    bodyLength = value.Length;
                }
            }
        }

        public Packet(PacketType type, byte[] body)
        {
            this.type = type;
            this.body = body;
            if (body != null) {
                this.bodyLength = body.Length;
            } else {
                this.bodyLength = 0;
            }
        }
    }

    public class PacketProtocol
    {
        public const int HEADER_LENGTH = 4;

        public static byte[] Encode(Packet p) {
            if (p.BodyLength == 0) {
                return new byte[] { Convert.ToByte(p.Type), 0, 0, 0 };
            }

            int length = HEADER_LENGTH + p.BodyLength;

            byte[] buf = new byte[length];

            buf[0] = Convert.ToByte(p.Type); // Packet 1 byte 表示类型
            buf[1] = Convert.ToByte(p.BodyLength >> 16 & 0xFF);
            buf[2] = Convert.ToByte(p.BodyLength >> 8 & 0xFF);
            buf[3] = Convert.ToByte(p.BodyLength & 0xFF); // 接下来三个byte表示body长度

            Array.Copy(p.Body, 0, buf, HEADER_LENGTH, p.BodyLength);

            return buf;
        }

        public static Packet Decode(byte[] buf)
        {
            PacketType type = (PacketType)buf[0];

            int bodyLength = (buf[1] << 16) + (buf[2] << 8) + buf[3];
            byte[] body = new byte[bodyLength];
            Array.Copy(buf, HEADER_LENGTH, body, 0, bodyLength);

            return new Packet(type, body);
        }
    }
}