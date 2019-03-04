using System;
using UniRx;
using UnityEngine.Networking;

namespace Networking.UNet {
    internal class UNetNetworkManager : INetworkManager {
        public bool IsConnected { get; private set; }
        public bool IsServer { get; private set; }

        public IObservable<NetworkConnectionResult> Connect() {
            Subject<NetworkConnectionResult> subject = new Subject<NetworkConnectionResult>();
            Observable.IntervalFrame(1).Subscribe(delegate {
                bool success;
                NetworkConnectionResult result = ConnectSync(out success);
                if (!success) {
                    subject.OnError(new Exception("Failed to connect to host using the given connection settings."));
                    return;
                }

                IsConnected = true;
                IsServer = result.isServer;
                subject.OnNext(result);
                subject.OnCompleted();
            });

            return subject.AsObservable();
        }

        /// <summary>
        /// UNet uses a synchronous method. We wrap it with an <see cref="IObservable{T}"/> to make the API
        /// more robust.
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        private NetworkConnectionResult ConnectSync(out bool success) {
            NetworkClient networkClient = NetworkManager.singleton.StartHost();
            bool isServer = true;
            if (networkClient == null) {
                isServer = false;
                networkClient = NetworkManager.singleton.StartClient();
            }

            if (networkClient == null) {
                success = false;
                return new NetworkConnectionResult();
            }

            success = true;
            return new NetworkConnectionResult(isServer);
        }
    }
}