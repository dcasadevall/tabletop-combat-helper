using System;
using Grid.Positioning;
using Math;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Zenject;

namespace Grid {
    public class GridInputManager : IGridInputManager, IInitializable {
        private readonly Camera _camera;
        private readonly EventSystem _eventSystem;
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

        public IObservable<IntVector2> LeftMouseButtonOnTile { get; private set; }

        public IntVector2? TileAtMousePosition {
            get {
                return MouseTileCoords.Value;
            }
        }

        public GridInputManager(Camera camera, EventSystem eventSystem, IGridPositionCalculator gridPositionCalculator) {
            _camera = camera;
            _eventSystem = eventSystem;
            _gridPositionCalculator = gridPositionCalculator;
        }

        public void Initialize() {
            MouseTileCoords = Observable.EveryUpdate()
                                        .Select(_ => GetTileAtMousePositionInternal())
                                        .DistinctUntilChanged()
                                        .ToReadOnlyReactiveProperty();

            LeftMouseButtonOnTile = Observable.EveryUpdate()
                                              .Where(_ => Input.GetMouseButton(0))
                                              .Where(_ => !_eventSystem.IsPointerOverGameObject())
                                              .Where(_ => TileAtMousePosition != null)
                                              .Select(_ => TileAtMousePosition.GetValueChecked());
        }

        private IntVector2? GetTileAtMousePositionInternal() {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = _camera.ScreenToWorldPoint(curScreenPoint);
            return _gridPositionCalculator.GetTileContainingWorldPosition(curPosition);
        }
    }
}