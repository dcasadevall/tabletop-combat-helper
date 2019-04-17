using System;
using System.Collections.Generic;
using Logging;
using UniRx;
using Zenject;

namespace Networking.Messaging {
    public class NetworkMessageQueue : INetworkMessageQueue, IInitializable, IDisposable {
        private readonly INetworkMessageHandler _networkMessageHandler;
        private readonly ILogger _logger;
        private IDisposable _networkMessageDisposable;

        private HashSet<short> _tagsRecorded = new HashSet<short>();
        private Dictionary<short, List<NetworkMessage>> _messages = new Dictionary<short, List<NetworkMessage>>();
        private Dictionary<string, int> _clientCursors = new Dictionary<string, int>();

        public NetworkMessageQueue(INetworkMessageHandler networkMessageHandler, ILogger logger) {
            _networkMessageHandler = networkMessageHandler;
            _logger = logger;
        }

        public void Initialize() {
            _networkMessageDisposable =
                _networkMessageHandler.NetworkMessageStream
                                      .Subscribe(Observer.Create<NetworkMessage>(HandleMessageReceived));
        }

        public void Dispose() {
            _networkMessageDisposable.Dispose();
        }

        private void HandleMessageReceived(NetworkMessage networkMessage) {
            if (!_tagsRecorded.Contains(networkMessage.tag)) {
                //Debug.Log(string.Format("NetworkMessageQueue. Ignore message with tag: {0}", tag));
                return;
            }

            if (!_messages.ContainsKey(networkMessage.tag)) {
                _messages[networkMessage.tag] = new List<NetworkMessage>();
            }

            _logger.Log(LoggedFeature.Network,
                        "NetworkMessageQueue received message with tag: {0}. From Player: {1}",
                        networkMessage.tag,
                        networkMessage.playerId);

            _messages[networkMessage.tag].Add(networkMessage);
        }

        public List<NetworkMessage> ConsumeMessages(string clientId, short tag) {
            if (!_messages.ContainsKey(tag)) {
                return new List<NetworkMessage>();
            }

            if (!_clientCursors.ContainsKey(clientId)) {
                _clientCursors[clientId] = 0;
            }

            // Has client consumed all messages?
            if (_clientCursors[clientId] >= _messages[tag].Count) {
                return new List<NetworkMessage>();
            }

            int cursor = _clientCursors[clientId];
            int count = _messages[tag].Count - cursor;
            List<NetworkMessage> consumedMessages = _messages[tag].GetRange(cursor, count);
            _clientCursors[clientId] = _messages[tag].Count;

            _logger.Log(LoggedFeature.Network, "Consuming {0} messages. Client: {1}", count, clientId);
            return consumedMessages;
        }

        public void RecordMessagesWithTag(short tag) {
            _logger.Log(LoggedFeature.Network, "NetworkMessageQueue: Listening to messages with tag: {0}", tag);
            _tagsRecorded.Add(tag);
        }
    }
}