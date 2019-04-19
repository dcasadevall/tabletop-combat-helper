using UnityEngine.Networking;

namespace Networking.UNet.Messaging {
    /// <summary>
    /// UNet requires us to send data via a message class which inherits from <see cref="MessageBase"/>.
    /// This is not ideal, as we simply want to be able to send a byte payload to a specific client or all clients.
    /// Instead, we use this proxy message that contains an array of bytes.
    /// </summary>
    internal class UNetMessageEnvelope : MessageBase {
        public const short kMessageType = MsgType.Highest + 1;
        private byte[] _payload;
        public byte[] Payload {
            get {
                return _payload;
            }
        }

        public UNetMessageEnvelope() {
        }

        public UNetMessageEnvelope(byte[] payload) {
            _payload = payload;
        }

        public override void Deserialize(NetworkReader reader) {
            _payload = reader.ReadBytesAndSize();;
        }

        public override void Serialize(NetworkWriter writer) {
            writer.WriteBytesFull(_payload);
        }
    }
}