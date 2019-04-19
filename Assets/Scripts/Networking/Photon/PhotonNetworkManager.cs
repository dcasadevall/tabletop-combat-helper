using System;
using Networking.Matchmaking;
using Networking.Photon.Connecting;
using Networking.Photon.Matchmaking;
using UniRx;
using UniRx.Async;

namespace Networking.Photon {
    internal class PhotonNetworkManager : INetworkManager {
        private readonly IPhotonRoomHandler _roomHandler;
        private readonly IPhotonNetworkConnector _networkConnector;
        private readonly IRoomSettings _roomSettings;
        
        public bool IsConnected { get; private set; }
        public bool IsServer { get; private set; }
        
        private Subject<int> _clientConnectedSubject = new Subject<int>();
        public IObservable<int> ClientConnected {
            get {
                return _clientConnectedSubject.AsObservable();
            }
        }

        public PhotonNetworkManager(IPhotonRoomHandler roomHandler, IPhotonNetworkConnector networkConnector) {
            _roomHandler = roomHandler;
            _networkConnector = networkConnector;
        }

        public IObservable<NetworkConnectionResult> Connect() {
            return ConnectTask().ToObservable();
        }

        private async UniTask<NetworkConnectionResult> ConnectTask() {
            await _networkConnector.Connect();

            var roomJoinResult = await _roomHandler.JoinOrCreateRoom();
            IsServer = roomJoinResult.isFirstParticipant;
            IsConnected = true;
            return new NetworkConnectionResult(IsServer);
        }
    }
}