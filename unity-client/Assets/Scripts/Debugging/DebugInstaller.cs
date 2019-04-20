using Zenject;

namespace Debugging {
    public class DebugInstaller : MonoInstaller {
        public SerializableDebugSettings debugSettings;
        
        public override void InstallBindings() {
#if DEBUG
            Container.BindInterfacesTo<DebugToggler>().FromInstance(new DebugToggler(debugSettings)).AsSingle();
            Container.Bind<IDebugSettings>().To<SerializableDebugSettings>().FromInstance(debugSettings).AsSingle();
#endif
        } 
    }
}