using System;
using Logging;
using Networking.UNet.Requests;
using UniRx;
using UniRx.Async;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

namespace Networking.UNet {
    public class UNetMatchmakingNetworkManager : INetworkManager, IDisposable {
        private readonly NetworkMatch _networkMatch;
        private readonly ILogger _logger;

        public bool IsConnected { get; private set; }
        public bool IsServer { get; private set; }

        /// <summary>
        /// If not null, this points to the pending connect task.
        /// </summary>
        private UniTask<NetworkConnectionResult>? _connectTask;

        /// <summary>
        /// If set, the id of the created / joined match.
        /// </summary>
        private NetworkID? _matchNetworkId;

        private Subject<int> _clientConnectedSubject = new Subject<int>();

        public IObservable<int> ClientConnected {
            get {
                return _clientConnectedSubject.AsObservable();
            }
        }

        public UNetMatchmakingNetworkManager(NetworkMatch networkMatch, ILogger logger) {
            _networkMatch = networkMatch;
            _logger = logger;
        }

        public IObservable<NetworkConnectionResult> Connect() {
            _connectTask = ConnectAsyncTask();
            return _connectTask.Value.ToObservable();
        }

        /// <summary>
        /// Helper UniTask that either joins a room or creates it if none exist yet (becoming the "host").
        /// </summary>
        /// <returns></returns>
        private async UniTask<NetworkConnectionResult> ConnectAsyncTask() {
            var matchList = await NetworkRequests.ListMatches(_networkMatch);
            MatchInfo matchInfo;
            if (matchList.Count == 0) {
                IsServer = true;
                matchInfo = await NetworkRequests.CreateMatch(_networkMatch, 10);
                _logger.Log(LoggedFeature.Network, "Created match as server: {0}", matchInfo);
                
                // First player needs to create the match and then join it.
                NetworkClient networkClient = new NetworkClient();
                await UNetNetworkManagerUtils.ClientConnectToMatch(networkClient, matchInfo);
                _logger.Log(LoggedFeature.Network, "Client connected to match: ", networkClient);
            } else {
                matchInfo = await NetworkRequests.JoinMatch(_networkMatch, matchList[0]);
                _logger.Log(LoggedFeature.Network, "Joining match as client: {0}", matchInfo);
            }

            
            _matchNetworkId = matchInfo.networkId;
            IsConnected = true;
            return new NetworkConnectionResult(IsServer);
        }

        public void Dispose() {
            _connectTask?.Forget();

            if (_matchNetworkId.HasValue && IsServer) {
                _logger.Log(LoggedFeature.Network, "Destroying match with network id: {0}", _matchNetworkId.Value);
                NetworkRequests.DestroyMatch(_networkMatch, _matchNetworkId.Value);
            }
        }
    }
}