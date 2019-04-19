using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UniRx.Async;
using Zenject;

namespace Networking.Photon.Connecting {
    internal class PhotonNetworkConnector : IPhotonNetworkConnector, IInitializable, IDisposable, IConnectionCallbacks {
        private readonly ServerSettings _photonServerSettings;
        private ConnectState _connectState;

        public PhotonNetworkConnector(ServerSettings photonServerSettings) {
            _photonServerSettings = photonServerSettings;
        }
        
        public async UniTask Connect() {
            _connectState = new ConnectState();
            PhotonNetwork.NetworkingClient.AppId = _photonServerSettings.AppSettings.AppIdRealtime;
            PhotonNetwork.ConnectToRegion("usw");
            await Observable.EveryUpdate().Where(_ => PhotonNetwork.IsConnectedAndReady).FirstOrDefault()
                            .Timeout(TimeSpan.FromSeconds(5));
            await Observable.EveryUpdate().Where(_ => _connectState.isConnectedToMaster).FirstOrDefault()
                            .Timeout(TimeSpan.FromSeconds(5));
        }
        
        public void Initialize() {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void Dispose() {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
        
        public void OnConnected() {
        }

        public void OnConnectedToMaster() {
            _connectState.isConnectedToMaster = true;
        }

        public void OnDisconnected(DisconnectCause cause) {
            _connectState.error = true;
        }

        public void OnRegionListReceived(global::Photon.Realtime.RegionHandler regionHandler) {
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data) {
        }

        public void OnCustomAuthenticationFailed(string debugMessage) {
        }

        private class ConnectState {
            public bool isConnectedToMaster;
            public bool error;
        }
    }
}