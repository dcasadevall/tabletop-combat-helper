using System;
using CommandSystem;
using Drawing.Commands;
using Drawing.DrawableTiles;
using Drawing.UI;
using Grid;
using Logging;
using Math;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using ILogger = Logging.ILogger;
using Vector2 = UnityEngine.Vector2;

namespace Drawing.Input {
    /// <summary>
    /// Handles mouse input and forwards events to the <see cref="IDrawableTile"/>s pertinent to the
    /// currently hovered tile.
    /// </summary>
    public class DrawingInputManager : ITickable, IInitializable, IDisposable {
        private const string kDrawingLayerName = "Drawing";

        private readonly Camera _camera;
        private readonly EventSystem _eventSystem;
        private readonly IDrawingViewController _drawingViewController;
        private readonly ILogger _logger;
        private readonly ICommandQueue _commandQueue;
        private readonly IGridInputManager _gridInputManager;
        private readonly IDrawableTileRegistry _drawableTileRegistry;
        private bool _isEnabled;

        public DrawingInputManager(Camera camera,
                                   EventSystem eventSystem,
                                   IDrawingViewController drawingViewController,
                                   ILogger logger,
                                   ICommandQueue commandQueue,
                                   IGridInputManager gridInputManager,
                                   IDrawableTileRegistry drawableTileRegistry) {
            _camera = camera;
            _eventSystem = eventSystem;
            _drawingViewController = drawingViewController;
            _logger = logger;
            _commandQueue = commandQueue;
            _gridInputManager = gridInputManager;
            _drawableTileRegistry = drawableTileRegistry;
        }

        public void Initialize() {
            _drawingViewController.DrawingEnabled += HandleDrawingEnabled;
            _drawingViewController.DrawingDisabled += HandleDrawingDisabled;
        }

        public void Dispose() {
            _drawingViewController.DrawingEnabled -= HandleDrawingEnabled;
            _drawingViewController.DrawingDisabled -= HandleDrawingDisabled;
        }

        private void HandleDrawingEnabled() {
            _isEnabled = true;
        }

        private void HandleDrawingDisabled() {
            _isEnabled = false;
        }

        public void Tick() {
            if (!_isEnabled) {
                return;
            }

            // Are we inside the grid?
            IntVector2? tileAtMouse = _gridInputManager.GetTileAtMousePosition();
            if (tileAtMouse == null) {
                return;
            }

            // We we clicked?
            if (!UnityEngine.Input.GetMouseButtonDown(0) && !UnityEngine.Input.GetMouseButton(0)) {
                return;
            }

            // Are we over a UI element?
            if (_eventSystem.IsPointerOverGameObject()) {
                return;
            }

            // This may lazily instantiate the tile used to draw, so it is important that it happens before the raycast.
            IDrawableTile drawableTile = _drawableTileRegistry.GetDrawableTileAtCoordinates(tileAtMouse.Value);
            Vector2 mouseWorldPoint = _camera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            RaycastHit2D raycastHit =
                Physics2D.Raycast(mouseWorldPoint, Vector2.zero, LayerMask.NameToLayer(kDrawingLayerName));
            if (raycastHit.collider == null) {
                return;
            }

            Vector2? localPosition = _drawableTileRegistry.GetLocalPosition(raycastHit.point);
            if (localPosition == null) {
                _logger.LogError(LoggedFeature.Drawing,
                                 "Raycast hit point not found inside drawable tile. Point: {0}. Tile: {1}",
                                 raycastHit.point,
                                 drawableTile);
                return;
            }

            Vector2 pixelPosition = GetLocalToPixelCoordinates(drawableTile.Sprite, localPosition.Value);
            PaintPixelData paintPixelData =
                new PaintPixelData(tileAtMouse.Value, IntVector2.Of(pixelPosition), _drawingViewController.PaintParams);
            _commandQueue.Enqueue<PaintPixelCommand, PaintPixelData>(paintPixelData, CommandSource.Game);
        }

        private Vector2 GetLocalToPixelCoordinates(Sprite sprite, Vector2 localPosition) {
            // Scale based on PixelsPerUnit in the sprite.
            float scaledX = localPosition.x * sprite.pixelsPerUnit;
            float scaledY = localPosition.y * sprite.pixelsPerUnit;

            // Round to nearest pixel
            return new Vector2(Mathf.RoundToInt(scaledX), Mathf.RoundToInt(scaledY));
        }
    }
}