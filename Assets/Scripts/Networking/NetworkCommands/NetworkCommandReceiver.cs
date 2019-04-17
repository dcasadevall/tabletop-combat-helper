using System;
using CommandSystem;
using Networking.Messaging;
using Replays.Persistence;
using UniRx;
using Zenject;

namespace Networking.NetworkCommands {
    public class NetworkCommandReceiver : IInitializable, IDisposable {
        private IDisposable _disposable;
        private readonly INetworkManager _networkManager;
        private readonly INetworkMessageHandler _networkMessageHandler;
        private readonly INetworkMessageSerializer _messageSerializer;
        private readonly ICommandQueue _commandQueue;

        public NetworkCommandReceiver(INetworkManager networkManager,
                                      INetworkMessageHandler networkMessageHandler,
                                      INetworkMessageSerializer messageSerializer, ICommandQueue commandQueue) {
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
            if (_networkManager.IsServer) {
                return;
            }
            
            SerializableCommand serializableCommand = _messageSerializer.Deserialize<SerializableCommand>(networkMessage);
            _commandQueue.Enqueue(serializableCommand.commandType, serializableCommand.dataType, serializableCommand.data);
        }
    }
}