using UnityEngine;
using Zenject;

namespace Replays.Persistence.UI {
    public class ReplayLoaderInstaller : MonoInstaller {
        [SerializeField]
        private GameObject _replayLoaderVcPrefab;

        public override void InstallBindings() {
            Container.Bind<IReplayLoaderViewController>().To<ReplayLoaderViewController>()
                     .FromComponentInNewPrefab(_replayLoaderVcPrefab).AsSingle();
        }
    }
}