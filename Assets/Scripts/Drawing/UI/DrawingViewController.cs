using System;
using CommandSystem;
using Drawing.Commands;
using Drawing.Input;
using Drawing.TexturePainter;
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
        public event Action CancelButtonPressed = delegate {};

        public bool IsDrawing { get; private set; }
        public TexturePaintParams PaintParams { get; private set; }

        // TODO: These should be shown / hidden via animator
        [SerializeField]
        private GameObject _stopPaintingButton;
        [SerializeField]
        private GameObject _drawingTools;
        [SerializeField]
        private ColorPicker _colorPicker;
        [SerializeField]
        private Slider _brushSizeSlider;

        private ICommandQueue _commandQueue;
        private IDrawingInputManager _drawingInputManager;
        private IInputLock _inputLock;
        private ILogger _logger;
        private Guid? _lockId;

        [Inject]
        public void Construct(ICommandQueue commandQueue,
                              IDrawingInputManager drawingInputManager, 
                              IInputLock inputLock, ILogger logger) {
            _commandQueue = commandQueue;
            _drawingInputManager = drawingInputManager;
            _inputLock = inputLock;
            _logger = logger;
        }

        private void Start() {
            Hide();
            SetColor(kDefaultColor);
        }

        // TODO: This is only needed because we can't seem to bind lifecycle events via facade,
        // even with "withKernel()"
        private void Update() {
            if (_lockId == null) {
                return;
            }
            
            _drawingInputManager.Tick(PaintParams);
        }

        public void Show() {
            _lockId = _inputLock.Lock();
            if (_lockId == null) {
                _logger.LogError(LoggedFeature.Drawing, "Failed to acquire input lock.");
                return;
            }

            IsDrawing = true;
            _stopPaintingButton.SetActive(true);
            _drawingTools.SetActive(true);

            DrawingEnabled.Invoke();
        }
        
        public void Hide() {
            if (_lockId != null) {
                _inputLock.Unlock(_lockId.Value);
                _lockId = null;
            }
            
            IsDrawing = false;
            _stopPaintingButton.SetActive(false);
            _drawingTools.SetActive(false);
            
            DrawingDisabled.Invoke();
        }

        public void SetBrush() {
            PaintParams = TexturePaintParams.MakeWithColor(_colorPicker.CurrentColor, (int)_brushSizeSlider.value);
        }

        public void SetColor(Color color) {
            PaintParams = TexturePaintParams.MakeWithColor(color, (int)_brushSizeSlider.value);
        }

        public void SetEraser() {
            PaintParams = TexturePaintParams.MakeEraser((int)_brushSizeSlider.value);
        }
        
        public void HandleBrushSizeSliderValueChanged() {
            PaintParams = TexturePaintParams.MakeWithColor(PaintParams.color, (int)_brushSizeSlider.value);
        }

        public void HandleCancelButtonPressed() {
            CancelButtonPressed.Invoke();
        }

        public void Clear() {
            _commandQueue.Enqueue<ClearAllPixelsCommand, ClearAllPixelsCommandData>();
        }
    }
}
