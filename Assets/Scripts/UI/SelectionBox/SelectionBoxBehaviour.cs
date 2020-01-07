using System;
using System.Collections.Generic;
using CameraSystem;
using Logging;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace UI.SelectionBox {
    /// <summary>
    /// Implementation of <see cref="ISelectionBox"/> that uses a sliced sprite to show the selection.
    /// </summary>
    public class SelectionBoxBehaviour : MonoBehaviour, ISelectionBox {
        private const float SELECTION_THRESHOLD_VIEWPORT_SIZE = 0.01f;
        
        [SerializeField]
        private SpriteRenderer _selectionSprite;
        
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private Camera _camera;
        private ILogger _logger;

        [Inject]
        public void Construct(Camera camera, ILogger logger) {
            _camera = camera;
            _logger = logger;
        }

        // Zenject starts with injected behaviours enabled. So we have to Hide() the object.
        private void Start() {
            Hide();
        }

        public async UniTask<Rect> Show() {
            if (_selectionSprite.drawMode != SpriteDrawMode.Sliced) {
                var msg = "SelectionSprite drawMode must be sliced";
                _logger.LogError(LoggedFeature.UI, msg);
                throw new Exception(msg);
            }
            
            // Show sprite
            Vector2 selectionStart = _camera.ScreenToWorldPoint(Input.mousePosition);

            // Subscribe to mouse events
            var mouseUpStream = Observable.EveryUpdate()
                                          .Where(_ => Input.GetMouseButtonUp(0))
                                          .Select(_ => _camera.ScreenToWorldPoint(Input.mousePosition))
                                          .Select(selectionEnd => GetSelectionRect(selectionStart, selectionEnd))
                                          .First();

            var mouseDragStream = Observable.EveryUpdate()
                                            .Where(_ => Input.GetMouseButton(0))
                                            .Select(_ => _camera.ScreenToWorldPoint(Input.mousePosition))
                                            .TakeUntil(mouseUpStream)
                                            .Pairwise()
                                            .Where(mousePos =>
                                                           Vector2.Distance(mousePos.Current, mousePos.Previous) >=
                                                           SELECTION_THRESHOLD_VIEWPORT_SIZE)
                                            .Select(selection => GetSelectionRect(selectionStart, selection.Current));
            mouseDragStream.Subscribe(HandleMouseDrag).AddTo(_disposables);
            var mouseUpTask = mouseUpStream.ToUniTask();
            var selectionRect = await mouseUpTask;
            Hide();

            return selectionRect;
        }

        private void Hide() {
            // Hide sprite
            _selectionSprite.enabled = false;

            // Clear subscribers
            _disposables.ForEach(x => x.Dispose());
            _disposables.Clear();
        }

        private void HandleMouseDrag(Rect selectionRect) {
            _selectionSprite.enabled = true;
            _selectionSprite.size = new Vector2(selectionRect.width, selectionRect.height);
            _selectionSprite.transform.position =
                new Vector3(selectionRect.center.x, selectionRect.center.y, _selectionSprite.transform.position.z);
        }

        private static Rect GetSelectionRect(Vector2 selectionStart, Vector2 selectionEnd) {
            return Rect.MinMaxRect(Mathf.Min(selectionStart.x, selectionEnd.x),
                                   Mathf.Min(selectionStart.y, selectionEnd.y),
                                   Mathf.Max(selectionStart.x, selectionEnd.x),
                                   Mathf.Max(selectionStart.y, selectionEnd.y));
        }
    }
}