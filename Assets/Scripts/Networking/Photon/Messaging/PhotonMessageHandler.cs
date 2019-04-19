using System;
using ExitGames.Client.Photon;
using Networking.Messaging;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using Zenject;

namespace Networking.Photon.Messaging {
    public class PhotonMessageHandler : INetworkMessageHandler, IInitializable, IDisposable {
        private Subject<NetworkMessage> _networkMessageSubject = new Subject<NetworkMessage>();
        public IObservable<NetworkMessage> NetworkMessageStream {
            get {
                return _networkMessageSubject.AsObservable();
            }
        }

        public void Initialize() {
            PhotonNetwork.NetworkingClient.EventReceived += HandleEventReceived;
        }

        public void Dispose() {
            _networkMessageSubject?.Dispose();
            PhotonNetwork.NetworkingClient.EventReceived -= HandleEventReceived;
        }

        private void HandleEventReceived(EventData eventData) {
            // For now, only handle network commands.
            if (eventData.Code != MessageTags.kNetworkCommand) {
                return;
            }
            
            byte[] data = (byte[]) eventData.Parameters[ParameterCode.Data];
            NetworkMessage networkMessage = new NetworkMessage(new Guid(), data, eventData.Code);
            _networkMessageSubject.OnNext(networkMessage);
        }

        public IObservable<Unit> BroadcastMessage(NetworkMessage networkMessage) {
            bool success = PhotonNetwork.RaiseEvent(MessageTags.kNetworkCommand,
                                                    networkMessage.data,
                                                    RaiseEventOptions.Default,
                                                    SendOptions.SendReliable);

            if (!success) {
                return Observable.Throw<Unit>(new Exception($"Error broadcasting message: {networkMessage}"));
            }

            return Observable.ReturnUnit();
        }

        public IObservable<Unit> SendMessage(NetworkMessage networkMessage, int clientId) {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
            raiseEventOptions.TargetActors = new[] {clientId};

            bool success = PhotonNetwork.RaiseEvent(MessageTags.kNetworkCommand,
                                                    networkMessage.data,
                                                    raiseEventOptions,
                                                    SendOptions.SendReliable);

            if (!success) {
                return Observable.Throw<Unit>(new Exception($"Error sending message: {networkMessage}"));
            }

            return Observable.ReturnUnit();
        }
    }
}