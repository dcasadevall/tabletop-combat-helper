using System;
using Grid;
using Grid.Positioning;
using Math;
using UnityEngine;
using Zenject;

namespace Drawing {
    /// <summary>
    /// Handles mouse input and forwards events to the <see cref="IDrawableTile"/>s pertinent to the
    /// currently hovered tile.
    /// </summary>
    public class DrawingInputManager : ITickable, IDrawingInputManager, IDrawingInputManagerInternal {
        public event Action DrawingEnabled = delegate {};
        public event Action DrawingDisabled = delegate {};
        
        public bool IsDrawing { get; private set; }

        public bool IsEnabled {
            get {
                return _isEnabled;
            }
            set {
                if (value == _isEnabled) {
                    return;
                }
                
                _isEnabled = value;
                if (_isEnabled) {
                    DrawingEnabled.Invoke();
                } else {
                    DrawingDisabled.Invoke();
                }
            }
        }

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
                IsDrawing = true;
                IDrawableTile drawableTile = _drawableTileRegistry.GetDrawableTileAtCoordinates(tileAtMouse.Value);
                drawableTile.HandleMouseDown();
            }

            if (Input.GetMouseButton(0)) {
                IDrawableTile drawableTile = _drawableTileRegistry.GetDrawableTileAtCoordinates(tileAtMouse.Value);
                drawableTile.HandleMouseDrag();
            }

            if (Input.GetMouseButtonUp(0)) {
                IsDrawing = false;
                IDrawableTile drawableTile = _drawableTileRegistry.GetDrawableTileAtCoordinates(tileAtMouse.Value);
                drawableTile.HandleMouseUp();
            }
        }
    }
}