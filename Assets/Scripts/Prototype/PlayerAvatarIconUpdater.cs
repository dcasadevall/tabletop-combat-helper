using UnityEngine;
using Zenject;

namespace Prototype {
    /// <summary>
    /// Simple Monobehaviour to show / hide an avatar icon based on the camera zoom.
    /// It can be globally disabled with the "I" key
    /// </summary>
    public class PlayerAvatarIconUpdater : MonoBehaviour {
        public KeyCode toggleKey = KeyCode.I;
        public float zoomThreshold = 5;
        public SpriteRenderer iconRenderer;
        public SpriteRenderer[] unitRenderers;
    
        [SerializeField]
        private Mode _mode = Mode.Mixed;
        private enum Mode {
            Mixed = 0,
            AvatarIconOnly,
            UnitOnly
        }

        private Camera _camera;

        [Inject]
        public void Construct(Camera camera) {
            _camera = camera;
        }

        private void Update() {
            if (Input.GetKeyUp(toggleKey)) {
                _mode = (Mode)(((int)_mode + 1) % 3);
            }

            bool isIconShown = _camera.orthographicSize >= zoomThreshold;
            if (_mode == Mode.UnitOnly) {
                isIconShown = false;
            }

            if (_mode == Mode.AvatarIconOnly) {
                isIconShown = true;
            }
            
            SetAvatarIcon(isIconShown);
        }

        private void SetAvatarIcon(bool shown) {
            if (shown) {
                iconRenderer.enabled = true;
                foreach (var spriteRenderer in unitRenderers) {
                    spriteRenderer.enabled = false;
                }
            } else {
                iconRenderer.enabled = false;
                foreach (var spriteRenderer in unitRenderers) {
                    spriteRenderer.enabled = true;
                }
            }    
        }
    }
}