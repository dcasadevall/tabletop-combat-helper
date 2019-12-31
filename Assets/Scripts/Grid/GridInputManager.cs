using System;
using Grid.Positioning;
using Math;
using UniRx;
using Units;
using UnityEngine;
using Zenject;

namespace Grid {
    public class GridInputManager : IGridInputManager, IInitializable {
        private readonly Camera _camera;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IGridPositionCalculator _gridPositionCalculator;

        /// <summary>
        /// Coordinates in the grid (if not null) over which the mouse is at.
        /// </summary>
        private ReadOnlyReactiveProperty<IntVector2?> MouseTileCoords { get; set; }

        public IObservable<IntVector2> MouseEnteredTile {
            get {
                return MouseTileCoords.Where(tile => tile != null).Select(tile => tile.Value);
            }
        }
        
        public IntVector2? TileAtMousePosition {
            get {
                return MouseTileCoords.Value;
            }
        }

        public IUnit[] UnitsAtMousePosition {
            get {
                if (!TileAtMousePosition.HasValue) {
                    return new IUnit[0];
                }

                return _gridUnitManager.GetUnitsAtTile(TileAtMousePosition.Value);
            }
        }

        public GridInputManager(Camera camera, 
                                IGridUnitManager gridUnitManager,
                                IGridPositionCalculator gridPositionCalculator) {
            _camera = camera;
            _gridUnitManager = gridUnitManager;
            _gridPositionCalculator = gridPositionCalculator;
        }

        public void Initialize() {
            MouseTileCoords = Observable.EveryUpdate()
                                        .Select(_ => GetTileAtMousePositionInternal())
                                        .DistinctUntilChanged()
                                        .ToReadOnlyReactiveProperty();
        }

        private IntVector2? GetTileAtMousePositionInternal() {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = _camera.ScreenToWorldPoint(curScreenPoint);
            return _gridPositionCalculator.GetTileContainingWorldPosition(curPosition); 
        }
    }
}