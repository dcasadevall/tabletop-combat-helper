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
    internal class UNetNetworkMessageHandler : IInitializable, IDisposable, INetworkMessageHandler {
        private Subject<Messaging.NetworkMessage> _subject = new Subject<Messaging.NetworkMessage>();

        public IObservable<Messaging.NetworkMessage> NetworkMessageStream {
            get {
                return _subject.AsObservable();
            }
        }

        private readonly INetworkManager _networkManager;
        private readonly ILogger _logger;
        private CompositeDisposable _disposables = new CompositeDisposable();

        public UNetNetworkMessageHandler(INetworkManager networkManager, ILogger logger) {
            _networkManager = networkManager;
            _logger = logger;
        }

        public void Initialize() {
            var disposable = _networkManager.GetNetworkClientObservable().Subscribe(Observer.Create<NetworkClient>(client => {
                _logger.Log(LoggedFeature.Network, "NetworkClient found. Initializing UNetNetworkMessageHandler");
                client.RegisterHandler(UNetMessageEnevlope.kMessageType, HandleUNetNetworkMessage);
            }));
            
            _disposables.Add(disposable);
        }
        
        public void Dispose() {
            _disposables.Dispose();
         }

        private void HandleUNetNetworkMessage(NetworkMessage unetMessage) {
            UNetMessageEnevlope messageEnvelope = unetMessage.ReadMessage<UNetMessageEnevlope>();
            using (var memoryStream = new MemoryStream()) {
                var binaryFormatter = new BinaryFormatter();
                memoryStream.Write(messageEnvelope.Payload, 0, messageEnvelope.Payload.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                Networking.Messaging.NetworkMessage networkMessage =
                    (Networking.Messaging.NetworkMessage) binaryFormatter.Deserialize(memoryStream);

                _subject.OnNext(networkMessage);
            }
        }

        public IObservable<Unit> BroadcastMessage(Messaging.NetworkMessage networkMessage) {
            if (!_networkManager.IsConnected) {
                return Observable.Throw<Unit>(new Exception("BroadcastMessage called when not connected."));
            }
            
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