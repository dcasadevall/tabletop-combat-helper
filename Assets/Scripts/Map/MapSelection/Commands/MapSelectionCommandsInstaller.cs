using Zenject;

namespace Map.MapSelection.Commands {
    public class MapSelectionCommandsInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<LoadMapCommand>().AsSingle();
        }
    }
}