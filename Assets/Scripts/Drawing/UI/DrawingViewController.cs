using FreeDraw;
using UnityEngine;

namespace Drawing.UI {
    /// <summary>
    /// ViewController to manage the painting view. This should be injected, but for now, just have it manage
    /// itself through unity's lifecycle.
    /// </summary>
    public class DrawingViewController : MonoBehaviour {
        // TODO: Inject this
        public static bool isDrawing = false;
        public GameObject startPaintingButton;
        public GameObject stopPaintingButton;
        public GameObject drawingTools;
        public BoxCollider2D drawableCollider;

        public void Awake() {
            StopPainting();
        }

        public void StartPainting() {
            startPaintingButton.SetActive(false);
            stopPaintingButton.SetActive(true);
            drawingTools.SetActive(true);
            drawableCollider.enabled = true;
            isDrawing = true;
        }
        
        public void StopPainting() {
            startPaintingButton.SetActive(true);
            stopPaintingButton.SetActive(false);
            drawingTools.SetActive(false);
            drawableCollider.enabled = false;
            isDrawing = false;
        }
    }
}
