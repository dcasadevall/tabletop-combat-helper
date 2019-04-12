using CommandSystem.Installers;
using Replays.Playback;
using Replays.UI;
using UnityEngine;
using Zenject;

namespace Replays {
    public class ReplayInstaller : MonoInstaller {
        [SerializeField]
        private GameObject _replayPlaybackVcPrefab;
        
        [SerializeField]
        private GameObject _replayRootVcPrefab;

        public override void InstallBindings() {
            Container.Bind<IReplayPlaybackViewController>().To<ReplayPlaybackViewController>()
                     .FromComponentInNewPrefab(_replayPlaybackVcPrefab).AsSingle();
            
            Container.Bind<IRootReplayViewController>().To<RootReplayViewController>()
                     .FromComponentInNewPrefab(_replayRootVcPrefab).AsSingle().NonLazy();

            Container.BindInterfacesTo<ReplayPlaybackManager>().FromSubContainerResolve()
                     .ByMethod(BindReplayPlaybackManager)
                     .WithKernel();
        }

        private void BindReplayPlaybackManager(DiContainer container) {
            container.BindInterfacesTo<ReplayPlaybackManager>().AsSingle();
            container.Install<CommandSystemInstaller>();
        }
    }
}