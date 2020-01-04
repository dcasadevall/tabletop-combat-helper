using System;
using CommandSystem;
using Drawing.Commands;
using Drawing.Input;
using Drawing.TexturePainter;
using InputSystem;
using Logging;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Logging.ILogger;

namespace Drawing.UI {
    /// <summary>
    /// ViewController to manage the painting view. This should be injected, but for now, just have it manage
    /// itself through unity's lifecycle.
    /// </summary>
    public class DrawingViewController : MonoBehaviour, IDrawingViewController, IDismissNotifyingViewController {
        private static Color kDefaultColor = Color.red;

        public event Action DrawingEnabled = delegate { };
        public event Action DrawingDisabled = delegate { };
        public event Action ViewControllerDismissed;

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
        private IInputLock _inputLock;
        private ILogger _logger;
        private Guid? _lockId;

        [Inject]
        public void Construct(ICommandQueue commandQueue,
                              IInputLock inputLock, ILogger logger) {
            _commandQueue = commandQueue;
            _inputLock = inputLock;
            _logger = logger;
        }

        private void Start() {
            Hide();
            SetColor(kDefaultColor);
        }

        public void Show() {
            _lockId = _inputLock.Lock();
            if (_lockId == null) {
                _logger.LogError(LoggedFeature.Drawing, "Failed to acquire input lock.");
                return;
            }

            _stopPaintingButton.SetActive(true);
            _drawingTools.SetActive(true);

            DrawingEnabled?.Invoke();
        }

        private void Hide() {
            if (_lockId != null) {
                _inputLock.Unlock(_lockId.Value);
                _lockId = null;
            }

            _stopPaintingButton.SetActive(false);
            _drawingTools.SetActive(false);

            DrawingDisabled?.Invoke();
            ViewControllerDismissed?.Invoke();
        }

        public void SetBrush() {
            PaintParams = TexturePaintParams.MakeWithColor(_colorPicker.CurrentColor, (int) _brushSizeSlider.value);
        }

        public void SetColor(Color color) {
            PaintParams = TexturePaintParams.MakeWithColor(color, (int) _brushSizeSlider.value);
        }

        public void SetEraser() {
            PaintParams = TexturePaintParams.MakeEraser((int) _brushSizeSlider.value);
        }

        public void HandleBrushSizeSliderValueChanged() {
            PaintParams = TexturePaintParams.MakeWithColor(PaintParams.color, (int) _brushSizeSlider.value);
        }

        public void HandleCancelButtonPressed() {
            Hide();
        }

        public void Clear() {
            _commandQueue.Enqueue<ClearAllPixelsCommand, ClearAllPixelsCommandData>(CommandSource.Game);
        }
    }
}