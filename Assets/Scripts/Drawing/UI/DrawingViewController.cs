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
        public event Action DrawingEnabled = delegate {};
        public event Action DrawingDisabled = delegate {};
        
        public bool IsDrawing { get; private set; }

        // TODO: Inject this
        public GameObject startPaintingButton;
        public GameObject stopPaintingButton;
        public GameObject clearButton;
        public GameObject drawingTools;

        private IDrawingInputManager _drawingInputManager;

        [Inject]
        public void Construct(IDrawingInputManager drawingInputManager) {
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

            IsDrawing = true;
            _drawingInputManager.IsEnabled = true;
            DrawingEnabled.Invoke();
        }
        
        public void StopPainting() {
            startPaintingButton.SetActive(true);
            stopPaintingButton.SetActive(false);
            clearButton.SetActive(false);
            drawingTools.SetActive(false);
            
            IsDrawing = false;
            _drawingInputManager.IsEnabled = false;
            DrawingDisabled.Invoke();
        }

        public void Clear() {
            
        }
    }
}
