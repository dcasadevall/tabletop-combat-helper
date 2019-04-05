using System;
using FreeDraw;
using UnityEngine;
using Zenject;

namespace Drawing.UI {
    /// <summary>
    /// ViewController to manage the painting view. This should be injected, but for now, just have it manage
    /// itself through unity's lifecycle.
    /// </summary>
    public class DrawingViewController : MonoBehaviour, IDrawingViewController {
        // TODO: Inject this
        public GameObject startPaintingButton;
        public GameObject stopPaintingButton;
        public GameObject clearButton;
        public GameObject drawingTools;

        private IDrawingInputManagerInternal _drawingInputManager;

        [Inject]
        public void Construct(IDrawingInputManagerInternal drawingInputManager) {
            _drawingInputManager = drawingInputManager;
        }

        private void Start() {
            StopPainting();
        }

        public void StartPainting() {
            startPaintingButton.SetActive(false);
            stopPaintingButton.SetActive(true);
            clearButton.SetActive(true);
            drawingTools.SetActive(true);

            _drawingInputManager.IsEnabled = true;
        }
        
        public void StopPainting() {
            startPaintingButton.SetActive(true);
            stopPaintingButton.SetActive(false);
            clearButton.SetActive(false);
            drawingTools.SetActive(false);
            
            _drawingInputManager.IsEnabled = false;
        }

        public void Clear() {
            
        }
    }
}
