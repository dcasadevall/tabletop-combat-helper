using Grid;
using Zenject;

namespace Map {
    public class MapInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.Bind<ICombatGrid>().To<CombatGrid>().AsSingle();
        }
    }
}
