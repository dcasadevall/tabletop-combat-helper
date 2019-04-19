using System;
using Networking.Matchmaking;
using Networking.Messaging;
using Networking.NetworkCommands;
using Networking.Photon;
using Networking.UNet;
using Zenject;

namespace Networking {
    /// <summary>
    /// Installer used for all networking related bindings.
    /// </summary>
    public class NetworkingInstaller : MonoInstaller {
        public SerializableNetworkSettings settings;
        public RoomSettings roomSettings;
        
        public override void InstallBindings() {
            Container.Bind<INetworkSettings>().To<SerializableNetworkSettings>().FromInstance(settings);
            Container.Bind<INetworkMessageSerializer>().To<NetworkMessageSerializer>().AsSingle();
            Container.Bind<IRoomSettings>().To<RoomSettings>().FromInstance(roomSettings);

            Container.Install<PhotonInstaller>();
        }
    }
}