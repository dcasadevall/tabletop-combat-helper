using UnityEngine;

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

        private bool isEnabled = true;

        private void Update() {
            if (Input.GetKeyUp(toggleKey)) {
                isEnabled = !isEnabled;
            }

            bool isIconShown = Camera.main.orthographicSize >= zoomThreshold && isEnabled;
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