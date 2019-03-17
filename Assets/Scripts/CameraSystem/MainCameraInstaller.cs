using UnityEngine;
using Zenject;

namespace CameraSystem {
    public class MainCameraInstaller : MonoInstaller {
        [SerializeField]
        private GameObject _cameraPrefab;
        
        public override void InstallBindings() {
            Container.Bind<Camera>().FromComponentInNewPrefab(_cameraPrefab).AsSingle();
            Container.Bind<IInitializable>().To<CameraInitializer>().AsSingle();
        }
    }
}