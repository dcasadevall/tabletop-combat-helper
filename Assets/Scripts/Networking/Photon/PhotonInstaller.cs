using Networking.Messaging;
using Networking.Photon.Connecting;
using Networking.Photon.Matchmaking;
using Networking.Photon.Messaging;
using Photon.Pun;
using Zenject;

namespace Networking.Photon {
    public class PhotonInstaller : Installer {
        public override void InstallBindings() {
            Container.BindInterfacesTo<PhotonRoomHandler>().AsSingle()
                     .WhenInjectedInto<PhotonNetworkManager>();
            Container.BindInterfacesTo<PhotonNetworkConnector>().AsSingle()
                     .WhenInjectedInto<PhotonNetworkManager>();
            Container.Bind<ServerSettings>().FromResources("PhotonServerSettings")
                     .WhenInjectedInto<PhotonNetworkConnector>();

            Container.Bind<INetworkManager>().To<PhotonNetworkManager>().AsSingle();
            Container.BindInterfacesTo<PhotonMessageHandler>().AsSingle();
        }
    }
}