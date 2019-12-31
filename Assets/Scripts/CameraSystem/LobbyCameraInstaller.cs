using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CameraSystem {
    public class LobbyCameraInstaller : MonoInstaller {
        [SerializeField]
        private GameObject _cameraPrefab;
        
        public override void InstallBindings() {
            Container.Bind(typeof(Camera), typeof(EventSystem)).FromComponentInNewPrefab(_cameraPrefab)
                     .AsSingle()
                     .NonLazy();
        }
    }
}