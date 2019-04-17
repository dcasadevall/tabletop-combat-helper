using System;
using UniRx;
using UnityEngine.Networking;
using Zenject;

namespace Networking.UNet {
    internal class UNetNetworkManager : INetworkManager, ITickable {
        private NetworkClient _networkClient;
        
        public bool IsConnected {
            get {
                return _networkClient != null;
            }
        }

        public bool IsServer { get; private set; }
        
        private INetworkSettings _networkSettings;
        public UNetNetworkManager(INetworkSettings networkSettings) {
            _networkSettings = networkSettings;
        }

        public IObservable<NetworkConnectionResult> Connect() {
            if (IsConnected) {
                return Observable.Throw<NetworkConnectionResult>(new Exception("Already connected"));
            }
            
            NetworkConnectionResult result = ConnectSync(out _networkClient);
            if (_networkClient == null) {
                Exception exception = new Exception("Failed to connect to host using the given connection settings.");
                return Observable.Throw<NetworkConnectionResult>(exception);
            }

            IsServer = result.isServer;
            return Observable.Return(result);
        }

        /// <summary>
        /// UNet uses a synchronous method. We wrap it with an <see cref="IObservable{T}"/> to make the API
        /// more robust.
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        private NetworkConnectionResult ConnectSync(out NetworkClient networkClient) {
            networkClient = NetworkManager.singleton.StartHost();
            bool isServer = true;
            if (networkClient == null) {
                isServer = false;
                networkClient = NetworkManager.singleton.StartClient();
            }

            if (networkClient == null) {
                return new NetworkConnectionResult();
            }

            return new NetworkConnectionResult(isServer);
        }

        public void Tick() {
        }
    }
}