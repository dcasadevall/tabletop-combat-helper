using UnityEngine;

namespace Prototype {
    /// <summary>
    /// Quick and dirty little behaviour to drag and drop 2d objects around the game.
    /// This will eventually be refactored
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class DragAndDropBehaviour : MonoBehaviour {
        // GHETTO remove
        public static bool isDragging = false;
        private Vector3 offset;

        private void OnMouseDown() {
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        }

        private void OnMouseUp() {
            isDragging = false;
        }

        private void OnMouseDrag() {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            transform.position = curPosition;
            isDragging = true;

            if (Input.GetKeyUp(KeyCode.R)) {
                transform.Rotate(Vector3.forward, 90);
            }
        }
    }
}
