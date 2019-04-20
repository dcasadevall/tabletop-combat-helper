using System;
using Networking.Matchmaking;
using Networking.Photon.Connecting;
using Networking.Photon.Matchmaking;
using UniRx;
using UniRx.Async;
using UnityEngine;

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

        public PhotonNetworkManager(IPhotonRoomHandler roomHandler, IPhotonNetworkConnector networkConnector) {
            _roomHandler = roomHandler;
            _networkConnector = networkConnector;
        }

        public IObservable<NetworkConnectionResult> Connect() {
            return ConnectTask().ToObservable();
        }

        private async UniTask<NetworkConnectionResult> ConnectTask() {
            await _networkConnector.Connect();

            await _roomHandler.JoinOrCreateRoom();
            IsConnected = true;
            return new NetworkConnectionResult(IsServer);
        }
    }
}