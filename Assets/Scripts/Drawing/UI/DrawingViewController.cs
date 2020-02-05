using System;
using CommandSystem;
using Drawing.Commands;
using Drawing.Input;
using Drawing.TexturePainter;
using InputSystem;
using Logging;
using UI;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using Utils;
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

        public TexturePaintParams PaintParams { get; private set; }

        [SerializeField]
        private GameObject _stopPaintingButton;

        [SerializeField]
        private GameObject _drawingTools;

        [SerializeField]
        private ColorPicker _colorPicker;

        [SerializeField]
        private Slider _brushSizeSlider;
        
        [SerializeField]
        private Button _cancelButton;

        private ICommandQueue _commandQueue;
        private IInputLock _inputLock;
        private IDisposable _lockToken;

        [Inject]
        public void Construct(ICommandQueue commandQueue,
                              IInputLock inputLock) {
            _commandQueue = commandQueue;
            _inputLock = inputLock;
            
            Preconditions.CheckNotNull(_stopPaintingButton, _drawingTools, _colorPicker, _brushSizeSlider);
        }

        private void Start() {
            _stopPaintingButton.SetActive(false);
            _drawingTools.SetActive(false);
            SetColor(kDefaultColor);
        }

        public async UniTask Show() {
            using (_inputLock.Lock()) {
                _stopPaintingButton.SetActive(true);
                _drawingTools.SetActive(true);
                DrawingEnabled?.Invoke();

                await _cancelButton.OnClickAsync();

                _stopPaintingButton.SetActive(false);
                _drawingTools.SetActive(false);
                DrawingDisabled?.Invoke();
            }
        }

        // The handlers below are set via onclick events on the prefab.
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

        public void Clear() {
            _commandQueue.Enqueue<ClearAllPixelsCommand, ClearAllPixelsCommandData>(CommandSource.Game);
        }
    }
}