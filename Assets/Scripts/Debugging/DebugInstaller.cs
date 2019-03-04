using Zenject;

namespace Debugging {
    public class DebugInstaller : MonoInstaller {
#if DEBUG
        public SerializableDebugSettings debugSettings;
        
        public override void InstallBindings() {
            Container.BindInterfacesTo<DebugToggler>().FromInstance(new DebugToggler(debugSettings)).AsSingle();
            Container.Bind<IDebugSettings>().To<SerializableDebugSettings>().FromInstance(debugSettings).AsSingle();
        } 
#endif
    }
}