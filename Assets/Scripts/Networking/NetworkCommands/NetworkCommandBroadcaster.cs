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
        private readonly List<ICommandSnapshot> _enqueuedCommands = new List<ICommandSnapshot>();

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
            // Record all commands we have queued, as they may need to be sent to other clients if
            // we become the room host.
            _enqueuedCommands.Add(commandSnapshot);
            
            // For now, every player will broadcast all commands (not just the server)
            // So simply make sure we don't requeue commands that have been queued by other means.
            if (commandSnapshot.Source != CommandSource.Game) {
                return;
            }
            
            _networkMessageHandler.BroadcastMessage(SerializeSnapshot(commandSnapshot));
        }

        private void HandleClientConnected(int clientId) {
            if (!_networkManager.IsServer) {
                return;
            }
            
            foreach (var commandSnapshot in _enqueuedCommands) {
                _networkMessageHandler.SendMessage(SerializeSnapshot(commandSnapshot), clientId);
            }
        }
        
        private NetworkMessage SerializeSnapshot(ICommandSnapshot commandSnapshot) {
            SerializableCommand serializableCommand = new SerializableCommand(commandSnapshot);
            return _networkMessageSerializer.Serialize(serializableCommand, MessageTags.kNetworkCommand); 
        }
    }
}