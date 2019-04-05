using UnityEngine;
using Zenject;

namespace CameraSystem {
    public class LobbyCameraInstaller : MonoInstaller {
        [SerializeField]
        private GameObject _cameraPrefab;
        
        public override void InstallBindings() {
            Container.Bind<Camera>().FromComponentInNewPrefab(_cameraPrefab)
                     .AsSingle()
                     .NonLazy();
        }
    }
}