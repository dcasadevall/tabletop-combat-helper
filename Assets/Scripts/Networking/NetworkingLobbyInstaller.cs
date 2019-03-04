using Zenject;

namespace Networking {
    /// <summary>
    /// This installer should be added to the lobby's <see cref="SceneContext"/>.
    /// It will deal with initializing objects that may need to do so after the scene has loaded, but may not be
    /// global to networking for all scenes (i.e: Autoconnect to network session).
    /// </summary>
    public class NetworkingLobbyInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.Bind<IInitializable>().To<NetworkConnector>().AsSingle();
        } 
    }
}