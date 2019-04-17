using System;
using System.Collections.Generic;
using CommandSystem;
using Networking.Messaging;
using Replays.Persistence;
using UniRx;
using UnityEngine.Networking;
using NetworkMessage = Networking.Messaging.NetworkMessage;

namespace Networking.NetworkCommands {
    public class NetworkCommandBroadcaster : ICommandQueueListener, IDisposable {
        private IDisposable _disposable;
        private readonly INetworkManager _networkManager;
        private readonly INetworkMessageHandler _networkMessageHandler;
        private readonly INetworkMessageSerializer _networkMessageSerializer;
        private readonly List<NetworkMessage> _broadcastedCommands = new List<NetworkMessage>();

        public NetworkCommandBroadcaster(ICommandQueue commandQueue,
                                         INetworkManager networkManager,
                                         INetworkMessageHandler networkMessageHandler,
                                         INetworkMessageSerializer networkMessageSerializer) {
            _networkManager = networkManager;
            _networkMessageHandler = networkMessageHandler;
            _networkMessageSerializer = networkMessageSerializer;

            _disposable = networkManager.ClientConnected.Subscribe(Observer.Create<int>(HandleClientConnected));
            commandQueue.AddListener(this);
        }

        public void Dispose() {
            _disposable?.Dispose();
        }

        public void HandleCommandQueued(ICommandSnapshot commandSnapshot) {
            if (!_networkManager.IsServer) {
                return;
            }
            
            SerializableCommand serializableCommand = new SerializableCommand(commandSnapshot);
            NetworkMessage networkMessage =
                _networkMessageSerializer.Serialize(serializableCommand, MessageTags.kNetworkCommand);
            _networkMessageHandler.BroadcastMessage(networkMessage);
            _broadcastedCommands.Add(networkMessage);
        }

        private void HandleClientConnected(int clientId) {
            foreach (var networkMessage in _broadcastedCommands) {
                _networkMessageHandler.SendMessage(networkMessage, clientId);
            }
        }
    }
}