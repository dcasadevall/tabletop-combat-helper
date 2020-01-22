using Zenject;

namespace Map.MapData.Store.Commands {
    public class MapDataStoreCommandsInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<LoadMapCommand>().AsSingle();
        }
    }
}