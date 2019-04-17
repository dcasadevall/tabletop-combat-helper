using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Zenject;

namespace Networking.Messaging {
    public class NetworkMessageSerializer : INetworkMessageSerializer {
        public NetworkMessage Serialize(ISerializable payload, short tag) {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream()) {
                binaryFormatter.Serialize(memoryStream, payload);
                // For now, ignore the playerId. In the future, it should be injected.
                return new NetworkMessage(new Guid(), memoryStream.ToArray(), tag);
            }
        }

        public T Deserialize<T>(NetworkMessage networkMessage) where T : ISerializable {
            using (var memoryStream = new MemoryStream()) {
                var binaryFormatter = new BinaryFormatter();
                memoryStream.Write(networkMessage.data, 0, networkMessage.data.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}