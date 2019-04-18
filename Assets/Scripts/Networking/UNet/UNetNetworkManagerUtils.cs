using System;
using UniRx;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

namespace Networking.UNet {
    /// <summary>
    /// Helper classed used internally in the UNet network classes to expose useful functionality specific to
    /// UNet.
    /// </summary>
    internal static class UNetNetworkManagerUtils {
        internal static IObservable<NetworkClient> GetNetworkClientObservable() {
            // Return an observable that will stop the moment client is not null.
            return NetworkManager.singleton.ObserveEveryValueChanged(manager => manager.client)
                                 .FirstOrDefault(client => client != null);
        }

        /// <summary>
        /// Connect to the given active match..
        /// The result is returned as an <see cref="IObservable{NetworkMessage}"/>.
        /// </summary>
        /// <param name="networkClient"></param>
        /// <returns></returns>
        internal static IObservable<Unit> GetClientConnectedObservable(NetworkClient networkClient) {
            return networkClient.ObserveEveryValueChanged(client => client.isConnected)
                                .FirstOrDefault(isConnected => isConnected).AsUnitObservable();
        }
    }
}