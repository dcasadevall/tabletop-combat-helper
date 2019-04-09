using CommandSystem;
using Drawing.Commands;
using Grid;
using Logging;
using Math;
using UnityEngine;
using UnityEngine.EventSystems;
using ILogger = Logging.ILogger;
using Vector2 = UnityEngine.Vector2;

namespace Drawing {
    /// <summary>
    /// Handles mouse input and forwards events to the <see cref="IDrawableTile"/>s pertinent to the
    /// currently hovered tile.
    /// </summary>
    public class DrawingInputManager : IDrawingInputManager {
        private const string kDrawingLayerName = "Drawing";
        
        private readonly Camera _camera;
        private readonly EventSystem _eventSystem;
        private readonly ILogger _logger;
        private readonly ICommand<PaintPixelData> _paintPixelCommand;
        private readonly ICommandQueue _commandQueue;
        private readonly IGridInputManager _gridInputManager;
        private readonly IDrawableTileRegistry _drawableTileRegistry;
        private bool _isEnabled;

        public DrawingInputManager(Camera camera, 
                                   EventSystem eventSystem,
                                   ILogger logger,
                                   ICommand<PaintPixelData> paintPixelCommand,
                                   ICommandQueue commandQueue,
                                   IGridInputManager gridInputManager,
                                   IDrawableTileRegistry drawableTileRegistry) {
            _camera = camera;
            _eventSystem = eventSystem;
            _logger = logger;
            _paintPixelCommand = paintPixelCommand;
            _commandQueue = commandQueue;
            _gridInputManager = gridInputManager;
            _drawableTileRegistry = drawableTileRegistry;
        }

        public void Tick(TexturePaintParams paintParams) {
            // Are we inside the grid?
            IntVector2? tileAtMouse = _gridInputManager.GetTileAtMousePosition();
            if (tileAtMouse == null) {
                return;
            }
            
            // We we clicked?
            if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButton(0)) {
                return;
            }
            
            // Are we over a UI element?
            if (_eventSystem.IsPointerOverGameObject()) {
                return;
            }
            
            // This may lazily instantiate the tile used to draw, so it is important that it happens before the raycast.
            IDrawableTile drawableTile = _drawableTileRegistry.GetDrawableTileAtCoordinates(tileAtMouse.Value);
            Vector2 mouseWorldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D raycastHit = Physics2D.Raycast(mouseWorldPoint, Vector2.zero, LayerMask.NameToLayer(kDrawingLayerName));
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
            PaintPixelData paintPixelData = new PaintPixelData(tileAtMouse.Value, pixelPosition, paintParams);
            _commandQueue.Enqueue(_paintPixelCommand, paintPixelData);
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