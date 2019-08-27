using System;
using UniRx;

namespace Networking {
    /// <summary>
    /// Layer of abstraction on top of the application's networking system.
    ///
    /// It allows developers to connect to a network session and query for information about the type of session.
    /// </summary>
    public interface INetworkManager {
        /// <summary>
        /// Returns true if the client is currently connected to a network session.
        /// </summary>
        bool IsConnected { get; }
      
        /// <summary>
        /// Returns true if this client is the server (or host) of the current network session.
        /// </summary>
        bool IsServer { get; }
        
        /// <summary>
        /// An Observable that will receive events when a client connects, providing the connection id of such client.
        /// </summary>
        IObservable<int> ClientConnected { get; }
        
        /// <summary>
        /// Observable that will fire events any time the manager is disconnected.
        /// </summary>
        IObservable<Unit> Disconnected { get; }

        /// <summary>
        /// Attempts to connect to a new or existing network session.
        ///
        /// If no session exists in the path determined by <see cref="INetworkSettings"/>, a new one will be created.
        /// In that case, <see cref="NetworkConnectionResult.isServer"/> will be true. Otherwise, the existing session
        /// will be joined (if there is room to do so), and <see cref="NetworkConnectionResult.isServer"/> will be false.
        /// </summary>
        /// <param name="allowOfflineMode">If true, a failure to connect will result in offline mode being used.</param>
        /// <returns></returns>
        IObservable<NetworkConnectionResult> Connect(bool allowOfflineMode);
    }
}