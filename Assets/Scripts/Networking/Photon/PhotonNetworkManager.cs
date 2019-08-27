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

        public bool IsServer {
            get {
                return _roomHandler.IsRoomHost;
            }
        }
        
        public IObservable<int> ClientConnected {
            get {
                return _roomHandler.PlayedJoinedRoomStream;
            }
        }

        public IObservable<Unit> Disconnected {
            get {
                return _roomHandler.RoomLeft;
            }
        }

        public PhotonNetworkManager(IPhotonRoomHandler roomHandler, IPhotonNetworkConnector networkConnector) {
            _roomHandler = roomHandler;
            _networkConnector = networkConnector;
            _roomHandler.RoomLeft.Subscribe(Observer.Create((Unit unit) => IsConnected = false));
        }

        public IObservable<NetworkConnectionResult> Connect(bool allowOfflineMode) {
            return ConnectTask(allowOfflineMode).ToObservable();
        }

        private async UniTask<NetworkConnectionResult> ConnectTask(bool allowOfflineMode) {
            await _networkConnector.Connect(allowOfflineMode);

            await _roomHandler.JoinOrCreateRoom();
            IsConnected = true;
            
            return new NetworkConnectionResult(IsServer);
        }
    }
}