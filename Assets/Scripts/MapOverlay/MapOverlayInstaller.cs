using UnityEngine;
using Zenject;

namespace MapOverlay {
    public class MapOverlayInstaller : MonoInstaller {
        [SerializeField]
        private GameObject _mapOverlayPrefab;

        public override void InstallBindings() {
            Container.Bind<IMapOverlayViewController>().To<MapOverlayViewController>()
                     .FromComponentInNewPrefab(_mapOverlayPrefab).AsSingle().NonLazy();
        }
    }
}