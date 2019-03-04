using Networking.UNet;
using Zenject;

namespace Networking {
    /// <summary>
    /// Installer used for all networking related bindings.
    /// </summary>
    public class NetworkingInstaller : MonoInstaller {
        public SerializableNetworkSettings settings;
        
        public override void InstallBindings() {
            Container.BindInstance(settings);
            Container.Bind<INetworkManager>().To<UNetNetworkManager>().AsSingle();
        } 
    }
}