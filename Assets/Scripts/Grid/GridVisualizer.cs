using Debugging;
using Grid.Positioning;
using Math;
using UnityEngine;
using Zenject;

namespace Grid {
    /// <summary>
    /// Debug class that allows os to visualize a <see cref="IGrid"/>.
    /// Uses <see cref="SpriteRenderer"/> provided by <see cref="GridCellFactory"/>, which is bound by
    /// Zenject.
    ///
    /// If <see cref="IDebugSettings.ShowDebugGrid"/> is false, the <see cref="SpriteRenderer"/>s are not shown.
    /// </summary>
    internal class GridVisualizer : IInitializable, ITickable {
        private IGrid _grid;
        private IDebugSettings _debugSettings;
        private IGridPositionCalculator _positionCalculator;
        private SpriteRenderer[,] _cells;
        private GridCellFactory _factory;

        public GridVisualizer(IGrid grid, IGridPositionCalculator gridPositionCalculator, GridCellFactory factory,
                              IDebugSettings debugSettings) {
            _grid = grid;
            _positionCalculator = gridPositionCalculator;
            _debugSettings = debugSettings;
            _factory = factory;
        }

        public void Initialize() {
            _cells = new SpriteRenderer[_grid.NumTilesX, _grid.NumTilesY];
        }
        
        public void Tick() {
            for (int x = 0; x < _grid.NumTilesX; x++) {
                for (int y = 0; y < _grid.NumTilesY; y++) {
                    RefreshCell(x, y);
                }
            }
        }

        private void RefreshCell(int x, int y) {
            // Avoid drawing if not necessary.
            if (_cells[x, y] == null && !_debugSettings.ShowDebugGrid) {
                return;
            }
            
            if (_cells[x, y] == null) {
                _cells[x, y] = _factory.Create();
                _cells[x, y].transform.position =
                    _positionCalculator.GetTileCenterWorldPosition(_grid, IntVector2.Of(x, y));
            }

            _cells[x, y].enabled = _debugSettings.ShowDebugGrid;
        }

        public class GridCellFactory : PlaceholderFactory<SpriteRenderer> {
        }
    }
}