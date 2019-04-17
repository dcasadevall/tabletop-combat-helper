using Zenject;

namespace Networking.NetworkCommands {
    public class NetworkCommandsInstaller : Installer {
        public override void InstallBindings() {
            // These two classes need a non lazy initialization since they are listening to events.
            Container.BindInterfacesTo<NetworkCommandReceiver>().AsSingle().NonLazy();
            Container.BindInterfacesTo<NetworkCommandBroadcaster>().AsSingle().NonLazy();
        }
    }
}