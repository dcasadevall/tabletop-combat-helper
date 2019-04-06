using Grid;
using Math;
using UnityEngine;
using Zenject;

namespace Drawing {
    /// <summary>
    /// Handles mouse input and forwards events to the <see cref="IDrawableTile"/>s pertinent to the
    /// currently hovered tile.
    /// </summary>
    public class DrawingInputManager : ITickable, IDrawingInputManager {
        public bool IsEnabled { get; set; }

        private readonly IGridInputManager _gridInputManager;
        private readonly IDrawableTileRegistry _drawableTileRegistry;
        private bool _isEnabled;

        public DrawingInputManager(IGridInputManager gridInputManager, IDrawableTileRegistry drawableTileRegistry) {
            _gridInputManager = gridInputManager;
            _drawableTileRegistry = drawableTileRegistry;
        }

        public void Tick() {
            if (!IsEnabled) {
                return;
            }
            
            IntVector2? tileAtMouse = _gridInputManager.GetTileAtMousePosition();
            if (tileAtMouse == null) {
                return;
            }
            
            if (Input.GetMouseButtonDown(0)) {
                IDrawableTile drawableTile = _drawableTileRegistry.GetDrawableTileAtCoordinates(tileAtMouse.Value);
                drawableTile.HandleMouseDown();
            }

            if (Input.GetMouseButton(0)) {
                IDrawableTile drawableTile = _drawableTileRegistry.GetDrawableTileAtCoordinates(tileAtMouse.Value);
                drawableTile.HandleMouseDrag();
            }

            if (Input.GetMouseButtonUp(0)) {
                IDrawableTile drawableTile = _drawableTileRegistry.GetDrawableTileAtCoordinates(tileAtMouse.Value);
                drawableTile.HandleMouseUp();
            }
        }
    }
}