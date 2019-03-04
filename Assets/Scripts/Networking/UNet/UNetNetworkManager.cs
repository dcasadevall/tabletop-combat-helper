using System;
using System.Collections.Generic;
using Networking.UNet.Requests;
using UniRx;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using Observable = UniRx.Observable;

namespace Networking.UNet {
    internal class UNetNetworkManager : INetworkManager {
        public bool IsConnected { get; private set; }
        public bool IsServer { get; private set; }

        private NetworkMatch _networkMatch;

        public UNetNetworkManager(NetworkMatch networkMatch) {
            _networkMatch = networkMatch;
        }
        
        public IObservable<NetworkConnectionResult> Connect() {
            if (networkClient == null) {
                
            }
            Obser

            Observable.IntervalFrame(1).
        }
    }
}