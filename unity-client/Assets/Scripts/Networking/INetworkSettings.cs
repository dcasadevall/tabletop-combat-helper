
namespace Networking {
    /// <summary>
    /// Interface used to expose the network settings to the network abstraction layer.
    /// </summary>
    public interface INetworkSettings {
        /// <summary>
        /// Port the server will be hosted on.
        /// </summary>
        int Port { get; }
        
        /// <summary>
        /// Address the server will be hosted on.
        /// </summary>
        string Address { get; }
    }
}