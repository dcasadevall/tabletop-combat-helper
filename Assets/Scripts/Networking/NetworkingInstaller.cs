using System;
using Networking.Messaging;
using Networking.UNet;
using Zenject;

namespace Networking {
    /// <summary>
    /// Installer used for all networking related bindings.
    /// </summary>
    public class NetworkingInstaller : MonoInstaller {
        public SerializableNetworkSettings settings;
        
        public override void InstallBindings() {
            Container.Bind<INetworkSettings>().To<SerializableNetworkSettings>().FromInstance(settings);
            Container.Bind<INetworkManager>().To<UNetNetworkManager>().AsSingle();
            Container.Bind(typeof(INetworkMessageHandler), typeof(IInitializable)).To<UNetNetworkMessageHandler>()
                     .AsSingle();
            Container.Bind(typeof(INetworkMessageQueue), typeof(IInitializable), typeof(IDisposable))
                     .To<NetworkMessageQueue>().AsSingle();
        } 
    }
}