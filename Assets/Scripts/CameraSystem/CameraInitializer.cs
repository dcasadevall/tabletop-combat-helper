using UnityEngine;
using Zenject;

namespace CameraSystem {
    public class CameraInitializer : IInitializable {
        private readonly Camera _camera;

        public CameraInitializer(Camera camera) {
            _camera = camera;
        }
        
        public void Initialize() {
        }
    }
}