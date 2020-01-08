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
        private ILogger _logger;

        [Inject]
        public void Construct(ILogger logger) {
            _logger = logger;
        }

        // Zenject starts with injected behaviours enabled. So we have to Hide() the object.
        private void Start() {
            Hide();
        }

        public IObservable<Rect> Show(ISelectionInputProvider selectionInputProvider) {
            if (_selectionSprite.drawMode != SpriteDrawMode.Sliced) {
                var msg = "SelectionSprite drawMode must be sliced";
                _logger.LogError(LoggedFeature.UI, msg);
                throw new Exception(msg);
            }

            // Subscribe to mouse events
            var mouseDownStream = Observable.EveryUpdate()
                                            .Where(_ => Input.GetMouseButtonDown(0))
                                            .Select(_ => selectionInputProvider.CurrentPosition);

            // Return a drag stream for each time we receive a mouse down.
            // This builds an observable of observables which will only emit as we drag.
            mouseDownStream
                .Select(_ => GetMouseDragStream(selectionInputProvider.CurrentPosition, selectionInputProvider))
                // Switch "Flattens" the observable and only emits the drag events.
                .Switch()
                .Subscribe(HandleMouseDrag)
                .AddTo(_disposables);

            // Same deal but this time only emit mouse ups. This is the event that we want to return.
            // Note that we need to pass in the "StartPosition" to the mouse up stream method to
            // create a different observable based on the start position, which depends on mouseDownStream.
            var mouseUpObservable = mouseDownStream
                                    .Select(startPosition => GetMouseUpStream(startPosition, selectionInputProvider))
                                    .Switch();
            mouseUpObservable.Subscribe(HandleMouseUp).AddTo(_disposables);
            
            return mouseUpObservable;
        }

        private IObservable<Rect> GetMouseDragStream(Vector3 startPosition,
                                                     ISelectionInputProvider selectionInputProvider) {
            return Observable.EveryUpdate()
                             .Where(_ => Input.GetMouseButton(0))
                             .Select(_ => selectionInputProvider.CurrentPosition)
                             .TakeUntil(GetMouseUpStream(startPosition, selectionInputProvider))
                             .Pairwise()
                             .Where(mousePos =>
                                        Vector2.Distance(mousePos.Current, mousePos.Previous) >=
                                        SELECTION_THRESHOLD_VIEWPORT_SIZE)
                             .Select(selection => GetSelectionRect(startPosition, selection.Current));
        }

        private IObservable<Rect> GetMouseUpStream(Vector3 startPosition,
                                                   ISelectionInputProvider selectionInputProvider) {
            return Observable.EveryUpdate()
                             .Where(__ => Input.GetMouseButtonUp(0))
                             .Select(selectionEnd =>
                                         GetSelectionRect(startPosition, selectionInputProvider.CurrentPosition));
        }

        public void Hide() {
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
        
        private void HandleMouseUp(Rect selectionRect) {
            _selectionSprite.enabled = false;
        }

        private static Rect GetSelectionRect(Vector2 selectionStart, Vector2 selectionEnd) {
            return Rect.MinMaxRect(Mathf.Min(selectionStart.x, selectionEnd.x),
                                   Mathf.Min(selectionStart.y, selectionEnd.y),
                                   Mathf.Max(selectionStart.x, selectionEnd.x),
                                   Mathf.Max(selectionStart.y, selectionEnd.y));
        }
    }
}