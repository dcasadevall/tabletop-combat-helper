using System;
using UniRx;
using UnityEngine.Networking;
using Zenject;

namespace Networking.UNet {
    internal class UNetLocalHostNetworkManager : INetworkManager, IDisposable {
        private NetworkClient _networkClient;
        
        public bool IsConnected {
            get {
                return _networkClient != null;
            }
        }

        public bool IsServer { get; private set; }

        private Subject<int> _clientConnectedSubject = new Subject<int>();
        public IObservable<int> ClientConnected {
            get {
                return _clientConnectedSubject.AsObservable();
            }
        }

        public IObservable<Unit> Disconnected {
            get {
                throw new NotImplementedException();
            }
        }

        private INetworkSettings _networkSettings;
        public UNetLocalHostNetworkManager(INetworkSettings networkSettings) {
            _networkSettings = networkSettings;
        }

        public void Dispose() {
            _clientConnectedSubject?.Dispose();
        }

        public IObservable<NetworkConnectionResult> Connect(bool allowOfflineMode) {
            if (IsConnected) {
                return Observable.Throw<NetworkConnectionResult>(new Exception("Already connected"));
            }
            
            NetworkConnectionResult result = ConnectSync(out _networkClient);
            if (_networkClient == null) {
                Exception exception = new Exception("Failed to connect to host using the given connection settings.");
                return Observable.Throw<NetworkConnectionResult>(exception);
            }

            IsServer = result.isServer;
            NetworkServer.RegisterHandler(MsgType.Connect, HandleClientConnected);
            return Observable.Return(result);
        }

        private void HandleClientConnected(NetworkMessage netmsg) {
            _clientConnectedSubject.OnNext(netmsg.conn.connectionId);
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
    }
}