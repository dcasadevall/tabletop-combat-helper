using Zenject;

namespace Map.MapSelection.Commands {
    public class MapSelectionCommandsInstaller : Installer {
        public override void InstallBindings() {
            // We could potentially split the edit / non edit commands, but that may be overkill
            Container.Bind<LoadMapCommand>().AsSingle();
            Container.Bind<LoadEditableMapCommand>().AsSingle();
        }
    }
}