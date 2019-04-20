using UniRx.Async;

namespace Networking.Photon.Connecting {
    /// <summary>
    /// Interface used to just perform the connection part of Photon Network Manager.
    /// This is separated due to the awkward callback system Photon has, which forces us to implement a bunch of methods.
    /// </summary>
    internal interface IPhotonNetworkConnector {
        UniTask Connect();
    }
}