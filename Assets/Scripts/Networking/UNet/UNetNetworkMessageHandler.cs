using System;
using System.Collections.Generic;
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

        private readonly List<short> _messageTypes;
        private readonly UNetNetworkManager _uNetNetworkManager;
        private readonly ILogger _logger;

        private CompositeDisposable _disposables;

        public UNetNetworkMessageHandler(List<short> messageTypes, UNetNetworkManager uNetNetworkManager, ILogger logger) {
            _messageTypes = messageTypes;
            _uNetNetworkManager = uNetNetworkManager;
            _logger = logger;
        }
        
        public void Initialize() {
            var disposable = _uNetNetworkManager.NetworkClient.Subscribe(Observer.Create<NetworkClient>(client => {
                _logger.Log(LoggedFeature.Network, "NetworkClient found. Initializing UNetNetworkMessageHandler");
                
                foreach (var messageType in _messageTypes) {
                    // The UNet API does not return a callback with the specific type,
                    // so we wrap it here with a lambda.
                    NetworkMessageDelegate callback = networkMessage => HandleUNetNetworkMessage(messageType, networkMessage);
                    client.RegisterHandler(messageType, callback);
                }
            }));
            
            _disposables.Add(disposable);
        }

        public void Dispose() {
            _disposables.Dispose();
        }

        private void HandleUNetNetworkMessage(short messageType, NetworkMessage unetMessage) {
            using (var memoryStream = new MemoryStream()) {
                var binaryFormatter = new BinaryFormatter();
                memoryStream.Write(unetMessage.reader.ReadBytes(unetMessage.reader.Length), 0, unetMessage.reader.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                Networking.Messaging.NetworkMessage networkMessage =
                    (Networking.Messaging.NetworkMessage) binaryFormatter.Deserialize(memoryStream);
                
                _subject.OnNext(networkMessage);
            }
        }

        public IObservable<Unit> BroadcastMessage(Messaging.NetworkMessage networkMessage) {
            var disposable = _uNetNetworkManager.NetworkClient.Subscribe(Observer.Create<NetworkClient>(client => {
                if (!_uNetNetworkManager.IsServer) {
                    return Observable.Throw<Unit>(new Exception());
                }
            
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (var memoryStream = new MemoryStream()) {
                    binaryFormatter.Serialize(memoryStream, networkMessage);
                    NetworkMessage unetMessage = new NetworkMessage();
                    client.Send
                    // bool result = client.SendBytes(memoryStream.ToArray(), (int) memoryStream.Length, 1);
                }
            }));
            
            _disposables.Add(disposable);
        }
    }
}