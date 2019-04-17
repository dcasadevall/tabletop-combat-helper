using System;
using UniRx;
using UnityEngine.Networking;

namespace Networking.UNet {
    /// <summary>
    /// Extensions used internally in the UNet network classes to expose useful functionality specific to
    /// UNet.
    /// </summary>
    internal static class UNetNetworkManagerExtensions {
        internal static IObservable<NetworkClient> GetNetworkClientObservable(this INetworkManager networkManager) {
            if (!networkManager.IsConnected) {
                // This creates an observable that stops observing after is connected is true.
                var serverConnectObservable =
                    networkManager.ObserveEveryValueChanged(manager => manager.IsConnected)
                                  .FirstOrDefault(isConnected => isConnected);
                
                // This uses the previous observable to return the current client once the observer receives a value.
                return serverConnectObservable.Select(connected => {
                    return NetworkManager.singleton.client;
                });
            }

            return Observable.Return(NetworkManager.singleton.client);
        }
    }
}