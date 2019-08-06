using UnityEngine;
using Zenject;

namespace EncounterOverlay {
    public class EncounterOverlayInstaller : MonoInstaller {
        [SerializeField]
        private GameObject _encounterOverlayPrefab;

        public override void InstallBindings() {
            Container.Bind<IEncounterOverlayViewController>().To<EncounterOverlayViewController>()
                     .FromComponentInNewPrefab(_encounterOverlayPrefab).AsSingle().NonLazy();
        }
    }
}