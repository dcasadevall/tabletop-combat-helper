using System;
using Logging;
using Networking.UNet.Requests;
using UniRx;
using UniRx.Async;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using Zenject;

namespace Networking.UNet {
    public class UNetMatchmakingNetworkManager : INetworkManager, IDisposable {
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
        
        /// <summary>
        /// If set, the network match used for matchmaking.
        /// </summary>
        private NetworkMatch _networkMatch;

        private Subject<int> _clientConnectedSubject = new Subject<int>();
        public IObservable<int> ClientConnected {
            get {
                return _clientConnectedSubject.AsObservable();
            }
        }

        public UNetMatchmakingNetworkManager(ILogger logger) {
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
            MatchInfo matchInfo;
            NetworkClient networkClient;
            NetworkManager networkManager = NetworkManager.singleton;
            networkManager.StartMatchMaker();
            _networkMatch = networkManager.matchMaker;
            var matchList = await NetworkRequests.ListMatches(_networkMatch);
            if (matchList.Count == 0) {
                matchInfo = await NetworkRequests.CreateMatch(_networkMatch, 10);
                _logger.Log(LoggedFeature.Network, "Created match as server: {0}", matchInfo);

                // First player needs to create the match and then start the host session.
                networkClient = networkManager.StartHost(matchInfo);
                IsServer = true;
            } else {
                // Other players join the mach, then start the client.
                matchInfo = await NetworkRequests.JoinMatch(_networkMatch, matchList[0]);
                _logger.Log(LoggedFeature.Network, "Joined match as client: {0}", matchInfo);

                networkClient = networkManager.StartClient(matchInfo);
            }

            await UNetNetworkManagerUtils.GetClientConnectedObservable(networkClient);
            _logger.Log(LoggedFeature.Network, "Connection established: {0}", networkClient.connection);
            _matchNetworkId = matchInfo.networkId;
            IsConnected = true;
            return new NetworkConnectionResult(IsServer);
        }

        public void Dispose() {
            _connectTask?.Forget();
            _clientConnectedSubject?.Dispose();

            if (_networkMatch != null && _matchNetworkId.HasValue && IsServer) {
                _logger.Log(LoggedFeature.Network, "Destroying match with network id: {0}", _matchNetworkId.Value);
                NetworkRequests.DestroyMatch(_networkMatch, _matchNetworkId.Value);
            }
        }
    }
}