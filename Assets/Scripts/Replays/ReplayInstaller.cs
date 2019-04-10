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
        }
    }
}