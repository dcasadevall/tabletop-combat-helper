using System;
using UniRx;
using UnityEngine.Networking;
using Zenject;

namespace Networking.UNet {
    internal class UNetNetworkManager : INetworkManager, ITickable {
        private NetworkClient _networkClient;
        private bool _connectionError;
        
        /// <summary>
        /// Returns a <see cref="NetworkClient"/> for use with UNet implementations of our network interfaces.
        /// If the network manager is not yet connected, or currently connecting, returns an Observable that
        /// will complete once a connection is finished.
        /// </summary>
        internal IObservable<NetworkClient> NetworkClient {
            get {
                if (_networkClient == null && _connectionError) {
                    return Observable.Throw<NetworkClient>(new Exception("There was an error creating the network session."));
                }

                if (_networkClient == null) {
                    return this.ObserveEveryValueChanged(manager => manager._networkClient);
                }

                return Observable.Return(_networkClient);
            }
        }
        
        public bool IsConnected { get; private set; }
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
                _connectionError = true;
                Exception exception = new Exception("Failed to connect to host using the given connection settings.");
                return Observable.Throw<NetworkConnectionResult>(exception);
            }

            IsConnected = true;
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