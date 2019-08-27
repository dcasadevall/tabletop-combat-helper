using System;
using CommandSystem;
using Networking.Messaging;
using Replays.Persistence;
using UniRx;
using Zenject;

namespace Networking.NetworkCommands {
    public class NetworkCommandReceiver : IInitializable, IDisposable {
        private IDisposable _disposable;
        private readonly INetworkMessageHandler _networkMessageHandler;
        private readonly INetworkMessageSerializer _messageSerializer;
        private readonly ICommandQueue _commandQueue;

        public NetworkCommandReceiver(INetworkMessageHandler networkMessageHandler,
                                      INetworkMessageSerializer messageSerializer, ICommandQueue commandQueue) {
            _networkMessageHandler = networkMessageHandler;
            _messageSerializer = messageSerializer;
            _commandQueue = commandQueue;
        }

        public void Initialize() {
            _disposable =
                _networkMessageHandler.NetworkMessageStream
                                      .Subscribe(Observer.Create<NetworkMessage>(HandleMessageReceived));
        }

        public void Dispose() {
            _disposable.Dispose();
        }

        private void HandleMessageReceived(NetworkMessage networkMessage) {
            // Note: We enqueue this command even as server, because we want clients to broadcast all their events
            // (for now)
            SerializableCommand serializableCommand =
                _messageSerializer.Deserialize<SerializableCommand>(networkMessage);
            _commandQueue.Enqueue(serializableCommand.commandType,
                                  serializableCommand.dataType,
                                  serializableCommand.data,
                                  CommandSource.Network);
        }
    }
}