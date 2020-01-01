using Replays.Playback.UI;
using UI;
using UnityEngine;
using Zenject;

namespace Replays.Playback {
    public class ReplayPlaybackInstaller : MonoInstaller {
        public const string REPLAY_OVERLAY_ID = "Replay";
            
        [SerializeField]
        private GameObject _replayPlaybackVcPrefab;

        public override void InstallBindings() {
            Container.Bind<IDismissNotifyingViewController>()
                     .To<ReplayPlaybackViewController>()
                     .FromComponentInNewPrefab(_replayPlaybackVcPrefab)
                     .AsSingle()
                     .WithConcreteId(REPLAY_OVERLAY_ID);

            Container.BindInterfacesTo<ReplayPlaybackManager>().AsSingle();
        }
    }
}