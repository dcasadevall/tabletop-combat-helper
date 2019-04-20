using UnityEngine.Networking;

namespace Networking {
    /// <summary>
    /// Data structure used to relay the result of <see cref="INetworkManager.Connect()"/>.
    /// </summary>
    public struct NetworkConnectionResult {
        /// <summary>
        /// True if the current client is the server of this network session.
        /// </summary>
        public readonly bool isServer;

        internal NetworkConnectionResult(bool isServer) {
            this.isServer = isServer;
        }
    }
}