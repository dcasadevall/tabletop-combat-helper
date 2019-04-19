using Networking.Messaging;
using Networking.Photon.Connecting;
using Networking.Photon.Matchmaking;
using Networking.Photon.Messaging;
using Photon.Pun;
using Zenject;

namespace Networking.Photon {
    public class PhotonInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<INetworkManager>().To<PhotonNetworkManager>().FromSubContainerResolve()
                     .ByMethod(BindPhotonNetworkManager).WithKernel().AsSingle();

            Container.BindInterfacesTo<PhotonMessageHandler>().AsSingle();
        }

        private void BindPhotonNetworkManager(DiContainer container) {
            container.Bind<PhotonNetworkManager>().AsSingle();
            container.BindInterfacesTo<PhotonRoomHandler>().AsSingle();
            container.Bind<IPhotonNetworkConnector>().To<PhotonNetworkConnector>().AsSingle();
            container.Bind<ServerSettings>().FromResources("PhotonServerSettings").AsSingle();
        }
    }
}