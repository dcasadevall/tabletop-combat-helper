using System;
using Grid.Positioning;
using Math;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Grid {
    public class GridInputManager : IGridInputManager {
        private readonly Camera _camera;
        private readonly IGridPositionCalculator _gridPositionCalculator;

        /// <summary>
        /// Coordinates in the grid (if not null) over which the mouse is at.
        /// </summary>
        private ReadOnlyReactiveProperty<IntVector2?> MouseTileCoords { get; }

        public IObservable<IntVector2> MouseEnteredTile {
            get {
                return MouseTileCoords.Where(tile => tile != null).Select(tile => tile.GetValueChecked());
            }
        }

        public IObservable<IntVector2> LeftMouseButtonOnTile { get; }
        public IObservable<IntVector2> LeftMouseDownOnTile { get; }
        public IObservable<IntVector2> LeftMouseDragOnTile { get; }

        public IntVector2? TileAtMousePosition {
            get {
                return MouseTileCoords.Value;
            }
        }

        public GridInputManager(Camera camera, EventSystem eventSystem,
                                IGridPositionCalculator gridPositionCalculator) {
            _camera = camera;
            _gridPositionCalculator = gridPositionCalculator;

            // These are initialized on construction to avoid race conditions on initialize
            MouseTileCoords = Observable.EveryUpdate()
                                        .Select(_ => GetTileAtMousePositionInternal())
                                        .DistinctUntilChanged()
                                        .ToReadOnlyReactiveProperty();

            IObservable<IntVector2> mouseDownStream = Observable.EveryUpdate()
                                                                .Where(_ => Input.GetMouseButtonDown(0))
                                                                .Where(_ => !eventSystem.IsPointerOverGameObject())
                                                                .Where(_ => TileAtMousePosition != null)
                                                                .Select(_ => TileAtMousePosition.GetValueChecked());

            IObservable<IntVector2> mouseUpStream = Observable.EveryUpdate()
                                                              .Where(_ => Input.GetMouseButtonUp(0))
                                                              .Where(_ => !eventSystem.IsPointerOverGameObject())
                                                              .Where(_ => TileAtMousePosition != null)
                                                              .Select(_ => TileAtMousePosition.GetValueChecked());

            IObservable<IntVector2> mouseDragStream = Observable.EveryUpdate()
                                                                .Where(_ => Input.GetMouseButton(0))
                                                                .Where(_ => TileAtMousePosition != null)
                                                                .TakeUntil(mouseUpStream)
                                                                .Select(_ => TileAtMousePosition.GetValueChecked());
            
            LeftMouseButtonOnTile = mouseDownStream.Select(pos => mouseUpStream).Switch();
            LeftMouseDownOnTile = mouseDownStream;
            LeftMouseDragOnTile = mouseDragStream;
        }

        private IntVector2? GetTileAtMousePositionInternal() {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = _camera.ScreenToWorldPoint(curScreenPoint);
            return _gridPositionCalculator.GetTileContainingWorldPosition(curPosition);
        }
    }
}