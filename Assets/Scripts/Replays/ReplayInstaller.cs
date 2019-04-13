using MapOverlay;
using Replays.Playback;
using Replays.UI;
using UnityEngine;
using Zenject;

namespace Replays {
    public class ReplayInstaller : MonoInstaller {
        [SerializeField]
        private GameObject _replayPlaybackVcPrefab;
        
        public override void InstallBindings() {
            Container.Bind<IReplayPlaybackViewController>().To<ReplayPlaybackViewController>()
                     .FromComponentInNewPrefab(_replayPlaybackVcPrefab).AsSingle();

            Container.BindInterfacesTo<ReplayPlaybackManager>().AsSingle();
        }
    }
}