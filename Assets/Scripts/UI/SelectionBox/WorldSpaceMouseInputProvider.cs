using UnityEngine;

namespace UI.SelectionBox {
    public class WorldSpaceMouseInputProvider : ISelectionInputProvider {
        public Vector3 CurrentPosition {
            get {
                return _camera.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        private readonly Camera _camera;

        public WorldSpaceMouseInputProvider(Camera camera) {
            _camera = camera;
        }
    }
}
