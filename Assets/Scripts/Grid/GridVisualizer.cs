using Debugging;
using Grid.Highlighting;
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
        private readonly IGrid _grid;
        private readonly IDebugSettings _debugSettings;
        private readonly IGridPositionCalculator _positionCalculator;
        private readonly IGridCellHighlightPool _cellHighlightPool;

        private IGridCellHighlight[,] _cells;

        public GridVisualizer(IGrid grid,
                              IGridPositionCalculator gridPositionCalculator,
                              IGridCellHighlightPool cellHighlightPool,
                              IDebugSettings debugSettings) {
            _grid = grid;
            _positionCalculator = gridPositionCalculator;
            _cellHighlightPool = cellHighlightPool;
            _debugSettings = debugSettings;
        }

        public void Initialize() {
            _cells = new IGridCellHighlight[_grid.NumTilesX, _grid.NumTilesY];
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

            if (_debugSettings.ShowDebugGrid) {
                if (_cells[x, y] == null) {
                    var position = _positionCalculator.GetTileCenterWorldPosition(IntVector2.Of(x, y));
                    _cells[x, y] = _cellHighlightPool.Spawn(position, new Color(0, 0, 0, 0));
                }

                _cells[x, y].SetColor(new Color(0, 0, 0, 0));
            }

            if (!_debugSettings.ShowDebugGrid && _cells[x, y] != null) {
                _cellHighlightPool.Despawn(_cells[x, y]);
                _cells[x, y] = null;
            }
        }

        public class GridCellFactory : PlaceholderFactory<Transform> { }
    }
}