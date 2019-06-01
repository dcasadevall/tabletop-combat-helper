using Units.Commands;
using Units.Spawning;
using Zenject;

namespace Units {
    /// <summary>
    /// This installer should be used at the context where a grid is loaded.
    /// This separation from <see cref="UnitsInstaller"/> is needed in order to have multiple grids spawning
    /// from the same unit pool.
    /// </summary>
    public class UnitSpawnInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.BindInterfacesTo<UnitSpawner>().AsSingle();
            Container.Install<UnitCommandsInstaller>();
        } 
    }
}