using System;
using Networking.Messaging;
using Networking.NetworkCommands;
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
            Container.Bind<INetworkMessageSerializer>().To<NetworkMessageSerializer>().AsSingle();
            
            Container.Install<UNetInstaller>();
            Container.Install<NetworkCommandsInstaller>();
        }
    }
}