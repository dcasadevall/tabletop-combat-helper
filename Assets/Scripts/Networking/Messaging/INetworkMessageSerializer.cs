using System;
using System.Runtime.Serialization;

namespace Networking.Messaging {
    public interface INetworkMessageSerializer {
        NetworkMessage Serialize(ISerializable payload, short tag);
        T Deserialize<T>(NetworkMessage networkMessage) where T : ISerializable;
    }
}