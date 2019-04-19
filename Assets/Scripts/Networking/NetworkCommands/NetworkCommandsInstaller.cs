using Zenject;

namespace Networking.NetworkCommands {
    /// <summary>
    /// This installer should be used in whichever context your game already has bindings to the proper
    /// <see cref="CommandSystem.ICommand"/>. That is why it is not directly included in the
    /// <see cref="NetworkingInstaller"/>.
    /// </summary>
    public class NetworkCommandsInstaller : Installer {
        public override void InstallBindings() {
            // These two classes need a non lazy initialization since they are listening to events.
            Container.BindInterfacesTo<NetworkCommandReceiver>().AsSingle().NonLazy();
            Container.BindInterfacesTo<NetworkCommandBroadcaster>().AsSingle().NonLazy();
        }
    }
}