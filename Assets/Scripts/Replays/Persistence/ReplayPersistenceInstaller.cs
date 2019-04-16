using UnityEngine;
using Zenject;

namespace Replays.Persistence {
    /// <summary>
    /// This installer should live in the project context, so that it can listen for commands throughout the
    /// application.
    /// </summary>
    public class ReplayPersistenceInstaller : MonoInstaller {
        [SerializeField]
        private PersistenceLayerSettings _settings;
        
        public override void InstallBindings() {
            Container.BindInterfacesTo<CommandHistoryFileSaver>().AsSingle();
            Container.BindInterfacesTo<CommandHistoryFileLoader>().AsSingle();
            
            Container.Bind<PersistenceLayerSettings>().FromInstance(_settings).AsSingle();
        }
    }
}