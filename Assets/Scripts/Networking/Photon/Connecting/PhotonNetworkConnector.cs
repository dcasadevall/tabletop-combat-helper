using System;
using System.Collections.Generic;
using Logging;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UniRx.Async;
using Zenject;

namespace Networking.Photon.Connecting {
    internal class PhotonNetworkConnector : IPhotonNetworkConnector {
        private readonly ServerSettings _photonServerSettings;
        private readonly ILogger _logger;

        public PhotonNetworkConnector(ServerSettings photonServerSettings, ILogger logger) {
            _photonServerSettings = photonServerSettings;
            _logger = logger;
        }
        
        public async UniTask Connect() {
            _logger.Log(LoggedFeature.Network, "Connecting to USW Server.");
            PhotonNetwork.NetworkingClient.AppId = _photonServerSettings.AppSettings.AppIdRealtime;
            PhotonNetwork.NetworkingClient.AppVersion = _photonServerSettings.AppSettings.AppVersion;
            PhotonNetwork.NetworkingClient.ExpectedProtocol = _photonServerSettings.AppSettings.Protocol;
            PhotonNetwork.ConnectToRegion("usw");
            await Observable.EveryUpdate().Where(_ => PhotonNetwork.IsConnectedAndReady).FirstOrDefault()
                            .Timeout(TimeSpan.FromSeconds(5)).CatchIgnore((Exception e) => {
                                _logger.Log(LoggedFeature.Network, "Starting in offline mode. Connection Exception: {0}", e);
                                PhotonNetwork.OfflineMode = true;
                            });
        }
    }
}