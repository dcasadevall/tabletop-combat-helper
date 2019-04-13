using Replays.Playback.UI;
using UnityEngine;
using Zenject;

namespace Replays.Playback {
    public class ReplayPlaybackInstaller : MonoInstaller {
        [SerializeField]
        private GameObject _replayPlaybackVcPrefab;

        public override void InstallBindings() {
            Container.Bind<IReplayPlaybackViewController>().To<ReplayPlaybackViewController>()
                     .FromComponentInNewPrefab(_replayPlaybackVcPrefab).AsSingle();

            Container.BindInterfacesTo<ReplayPlaybackManager>().AsSingle();
        }
    }
}