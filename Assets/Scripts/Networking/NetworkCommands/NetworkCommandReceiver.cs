using System;
using CommandSystem;
using Networking.Messaging;
using Replays.Persistence;
using UniRx;
using Zenject;

namespace Networking.NetworkCommands {
    public class NetworkCommandReceiver : IInitializable, IDisposable {
        private IDisposable _disposable;
        private readonly SequenceIndex _sequenceIndex;
        private readonly INetworkManager _networkManager;
        private readonly INetworkMessageHandler _networkMessageHandler;
        private readonly INetworkMessageSerializer _messageSerializer;
        private readonly ICommandQueue _commandQueue;

        public NetworkCommandReceiver(SequenceIndex sequenceIndex,
                                      INetworkManager networkManager,
                                      INetworkMessageHandler networkMessageHandler,
                                      INetworkMessageSerializer messageSerializer, ICommandQueue commandQueue) {
            _sequenceIndex = sequenceIndex;
            _networkManager = networkManager;
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
            // This may change.
            // For now, don't handle collisions with commands either.
            // We will have to handle versioning but we need to decide what to do with master / clients.
            
            SerializableNetworkCommand serializableCommand =
                _messageSerializer.Deserialize<SerializableNetworkCommand>(networkMessage);
            _commandQueue.Enqueue(serializableCommand.command.commandType,
                                  serializableCommand.command.dataType,
                                  serializableCommand.command.data,
                                  CommandSource.Network);
        }
    }
}