using CommandSystem;
using Zenject;

namespace Grid.Commands {
    public class GridCommandsInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<ICommand<MoveUnitData>>().To<MoveUnitCommand>().AsSingle();
        }
    }
}