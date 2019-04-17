
using System.Collections.Generic;

namespace Networking.Messaging {
    /// <summary>
    /// A queue that clients can use to consume messages received by other network entities.
    /// Clients will have a unique client id, which will be used to keep track of the latest messages consumed
    /// for a given tag.
    /// </summary>
    public interface INetworkMessageQueue {
        /// <summary>
        /// Returns all the messages received since the last time the given client called ConsumeMessages,
        /// for the given tag.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        List<NetworkMessage> ConsumeMessages(string clientId, short tag);
        
        /// <summary>
        /// Messages with the given tag will be recorded so that they can be consumed by any client, at any point in time
        /// This is on demand, so we don't store a bunch of useless data in memory
        /// </summary>
        /// <param name="tag"></param>
        void RecordMessagesWithTag(short tag);
    }
}