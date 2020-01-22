using Networking.NetworkCommands;
using Zenject;

namespace EncounterSelection {
    public class EncounterSelectionInstaller : MonoInstaller {
        public bool showEncounterSelectionView = true;
        
        public override void InstallBindings() {
            EncounterSelectionContext context = new EncounterSelectionContext();
            Container.Bind<IEncounterSelectionContext>().To<EncounterSelectionContext>().FromInstance(context);

            if (showEncounterSelectionView) {
                Container.Bind<EncounterSelectionContext>().FromInstance(context).AsSingle()
                         .WhenInjectedInto<EncounterSelectionLoader>();
                Container.BindInterfacesTo<EncounterSelectionLoader>().AsSingle();
            } else {
                context.EncounterType = EncounterType.Replay;
            }
            
            // Network commands are safe to be executed at this point.
            Container.Install<NetworkCommandsInstaller>();
        }
    }
}