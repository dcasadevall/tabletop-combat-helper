using System;
using UniRx;

namespace Networking.Messaging {
    public interface INetworkMessageHandler {
        /// <summary>
        /// An <see cref="IObservable{T}"/> stream that can be listened to in order to receive notifications
        /// of incoming messages.
        /// </summary>
        IObservable<NetworkMessage> NetworkMessageStream { get; }
        IObservable<Unit> BroadcastMessage(NetworkMessage networkMessage);
        IObservable<Unit> SendMessage(NetworkMessage networkMessage, int clientId);
    }
}