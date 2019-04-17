using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Logging;
using Networking.Messaging;
using UniRx;
using UnityEngine.Networking;
using Zenject;
using NetworkMessage = UnityEngine.Networking.NetworkMessage;

namespace Networking.UNet {
    internal class UNetNetworkMessageHandler : IInitializable, INetworkMessageHandler {
        private Subject<Messaging.NetworkMessage> _subject = new Subject<Messaging.NetworkMessage>();

        public IObservable<Messaging.NetworkMessage> NetworkMessageStream {
            get {
                return _subject.AsObservable();
            }
        }

        private readonly INetworkManager _networkManager;
        private readonly ILogger _logger;

        public UNetNetworkMessageHandler(INetworkManager networkManager, ILogger logger) {
            _networkManager = networkManager;
            _logger = logger;
        }

        public void Initialize() {
            _logger.Log(LoggedFeature.Network, "Initializing UNetNetworkMessageHandler");
            NetworkManager.singleton.client.RegisterHandler(UNetMessageEnevlope.kMessageType, HandleUNetNetworkMessage);
        }

        private void HandleUNetNetworkMessage(NetworkMessage unetMessage) {
            UNetMessageEnevlope messageEnvelope = unetMessage.ReadMessage<UNetMessageEnevlope>();
            using (var memoryStream = new MemoryStream()) {
                var binaryFormatter = new BinaryFormatter();
                memoryStream.Write(messageEnvelope.Payload,
                                   0,
                                   messageEnvelope.Payload.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                Networking.Messaging.NetworkMessage networkMessage =
                    (Networking.Messaging.NetworkMessage) binaryFormatter.Deserialize(memoryStream);

                _subject.OnNext(networkMessage);
            }
        }

        public IObservable<Unit> BroadcastMessage(Messaging.NetworkMessage networkMessage) {
            if (!_networkManager.IsServer) {
                return Observable.Throw<Unit>(new Exception("BroadcastMessage called from client."));
            }

            bool success;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream()) {
                binaryFormatter.Serialize(memoryStream, networkMessage);
                UNetMessageEnevlope messageEnevlope = new UNetMessageEnevlope(memoryStream.ToArray());
                success = NetworkServer.SendToAll(UNetMessageEnevlope.kMessageType, messageEnevlope);
            }

            if (!success) {
                return Observable.Throw<Unit>(new Exception($"Error broadcasting message: {networkMessage}"));
            }

            return Observable.ReturnUnit();
        }
    }
}