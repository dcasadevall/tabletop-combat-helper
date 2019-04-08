using System;
using InputSystem;
using Logging;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Logging.ILogger;

namespace Drawing.UI {
    /// <summary>
    /// ViewController to manage the painting view. This should be injected, but for now, just have it manage
    /// itself through unity's lifecycle.
    /// </summary>
    public class DrawingViewController : MonoBehaviour, IDrawingViewController {
        private static Color kDefaultColor = Color.red;
        
        public event Action DrawingEnabled = delegate {};
        public event Action DrawingDisabled = delegate {};
        
        public bool IsDrawing { get; private set; }
        public TexturePaintParams PaintParams { get; private set; }

        [SerializeField]
        private GameObject _startPaintingButton;
        [SerializeField]
        private GameObject _stopPaintingButton;
        [SerializeField]
        private GameObject _drawingTools;
        [SerializeField]
        private Slider _brushSizeSlider;

        private IDrawingInputManager _drawingInputManager;
        private IDrawableTileRegistry _drawableTileRegistry;
        private IInputLock _inputLock;
        private ILogger _logger;
        private Guid? _lockId;

        [Inject]
        public void Construct(IDrawingInputManager drawingInputManager, IDrawableTileRegistry drawableTileRegistry,
                              IInputLock inputLock, ILogger logger) {
            _drawingInputManager = drawingInputManager;
            _drawableTileRegistry = drawableTileRegistry;
            _inputLock = inputLock;
            _logger = logger;
        }

        private void Start() {
            SetColor(kDefaultColor);
        }

        // TODO: This is only needed because we can't seem to bind lifecycle events via facade,
        // even with "withKernel()"
        private void Update() {
            if (_lockId == null) {
                return;
            }
            
            _drawingInputManager.Tick();
        }

        public void TogglePainting() {
            IsDrawing = !IsDrawing;
            if (IsDrawing) {
                StartPainting();
            } else {
                StopPainting();
            }
        }

        private void StartPainting() {
            _lockId = _inputLock.Lock();
            if (_lockId == null) {
                _logger.LogError(LoggedFeature.Drawing, "Failed to acquire input lock.");
                return;
            }
            
            _startPaintingButton.SetActive(false);
            _stopPaintingButton.SetActive(true);
            _drawingTools.SetActive(true);

            DrawingEnabled.Invoke();
        }
        
        private void StopPainting() {
            if (_lockId == null) {
                _logger.LogError(LoggedFeature.Drawing, "Stopped painting without input lock.");
                return;
            }
            _inputLock.Unlock(_lockId.Value);
            _lockId = null;
            
            _startPaintingButton.SetActive(true);
            _stopPaintingButton.SetActive(false);
            _drawingTools.SetActive(false);
            
            DrawingDisabled.Invoke();
        }

        public void SetColor(Color color) {
            PaintParams = TexturePaintParams.MakeWithColor(color, (int)_brushSizeSlider.value);
        }

        public void SetEraser() {
            PaintParams = TexturePaintParams.MakeEraser((int)_brushSizeSlider.value);
        }

        public void Clear() {
            foreach (var drawableTile in _drawableTileRegistry.GetAllTiles()) {
                drawableTile.Clear();
            }
        }
    }
}
