using Zenject;

namespace EncounterSelection {
    public class EncounterSelectionInstaller : MonoInstaller {
        public override void InstallBindings() {
            EncounterSelectionContext context = new EncounterSelectionContext();
            Container.Bind<EncounterSelectionContext>().FromInstance(context).AsSingle()
                     .WhenInjectedInto<EncounterSelectionLoader>();
            Container.Bind<IEncounterSelectionContext>().To<EncounterSelectionContext>().FromInstance(context);
            Container.Bind<IInitializable>().To<EncounterSelectionLoader>().AsSingle();
        }
    }
}