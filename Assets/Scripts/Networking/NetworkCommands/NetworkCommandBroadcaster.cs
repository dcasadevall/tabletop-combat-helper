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
        private readonly SequenceIndex _sequenceIndex;
        private readonly INetworkManager _networkManager;
        private readonly INetworkMessageHandler _networkMessageHandler;
        private readonly INetworkMessageSerializer _networkMessageSerializer;
        private readonly List<NetworkMessage> _enqueuedCommands = new List<NetworkMessage>();
        private readonly List<NetworkMessage> _erroredCommands = new List<NetworkMessage>();

        public NetworkCommandBroadcaster(SequenceIndex sequenceIndex,
                                         ICommandQueue commandQueue,
                                         INetworkManager networkManager,
                                         INetworkMessageHandler networkMessageHandler,
                                         INetworkMessageSerializer networkMessageSerializer) {
            _sequenceIndex = sequenceIndex;
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
            NetworkMessage networkMessage = SerializeSnapshot(commandSnapshot);
            _enqueuedCommands.Add(networkMessage);

            // For now, every player will broadcast all commands (not just the server)
            // So simply make sure we don't requeue commands that have been queued by other means.
            if (commandSnapshot.Source != CommandSource.Game) {
                return;
            }

            // Broadcast the message, adding it to the errored message queue if we fail to do so.
            _networkMessageHandler.BroadcastMessage(networkMessage)
                                  .Subscribe(unit => { }, error => _erroredCommands.Add(networkMessage));
        }

        private void HandleClientConnected(int clientId) {
            if (!_networkManager.IsServer) {
                return;
            }

            foreach (var enqueuedCommand in _enqueuedCommands) {
                _networkMessageHandler.SendMessage(enqueuedCommand, clientId);
            }
        }

        private NetworkMessage SerializeSnapshot(ICommandSnapshot commandSnapshot) {
            SerializableNetworkCommand serializableCommand =
                new SerializableNetworkCommand(_sequenceIndex.index, new SerializableCommand(commandSnapshot));
            _sequenceIndex.index++;
            
            return _networkMessageSerializer.Serialize(serializableCommand, MessageTags.kNetworkCommand);
        }
    }
}