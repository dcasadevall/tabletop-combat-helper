using UnityEngine;

namespace CameraSystem {
    /// <summary>
    /// Helper class that provides a set of shortcuts for calculating input based on our game
    /// camera position.
    /// </summary>
    public class CameraInput {
        private readonly Camera _camera;

        public Vector2 MouseWorldPosition {
            get {
                return _camera.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        
        public CameraInput(Camera camera) {
            _camera = camera;
        }
    }
}