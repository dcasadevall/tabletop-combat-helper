using System;
using CommandSystem;
using Networking.Messaging;
using Replays.Persistence;
using UnityEngine.Networking;
using NetworkMessage = Networking.Messaging.NetworkMessage;

namespace Networking.NetworkCommands {
    public class NetworkCommandBroadcaster : ICommandQueueListener {
        private readonly INetworkManager _networkManager;
        private readonly INetworkMessageHandler _networkMessageHandler;
        private readonly INetworkMessageSerializer _networkMessageSerializer;

        public NetworkCommandBroadcaster(ICommandQueue commandQueue,
                                         INetworkManager networkManager,
                                         INetworkMessageHandler networkMessageHandler,
                                         INetworkMessageSerializer networkMessageSerializer) {
            _networkManager = networkManager;
            _networkMessageHandler = networkMessageHandler;
            _networkMessageSerializer = networkMessageSerializer;
            commandQueue.AddListener(this);
        }

        public void HandleCommandQueued(ICommandSnapshot commandSnapshot) {
            if (!_networkManager.IsServer) {
                return;
            }
            
            SerializableCommand serializableCommand = new SerializableCommand(commandSnapshot);
            NetworkMessage networkMessage =
                _networkMessageSerializer.Serialize(serializableCommand, MessageTags.kNetworkCommand);
            _networkMessageHandler.BroadcastMessage(networkMessage);
        }
    }
}