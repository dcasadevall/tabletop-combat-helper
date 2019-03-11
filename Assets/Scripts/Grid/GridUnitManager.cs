using System.Collections.Generic;
using Math;
using Units;
using Zenject;

namespace Grid {
    /// <summary>
    /// Implementation of <see cref="IGridUnitManager"/> which uses an in memory map as a registry.
    /// </summary>
    public class GridUnitManager : IGridUnitManager, IInitializable {
        public event System.Action<IUnit, IntVector2> UnitPlacedAtTile = delegate {};
        
        private Dictionary<UnitId, int> _unitMap = new Dictionary<UnitId, int>();
        private List<IUnit>[,] _tiles;
        private IGrid _grid;

        public GridUnitManager(IGrid grid) {
            _grid = grid;
        }
        
        public void Initialize() {
            _tiles = new List<IUnit>[_grid.NumTilesX, _grid.NumTilesY];
            
            for (int x = 0; x < _grid.NumTilesX; x++) {
                for (int y = 0; y < _grid.NumTilesY; y++) {
                    _tiles[x, y] = new List<IUnit>();
                }
            }
        }
        
        public IUnit[] GetUnitsAtTile(int x, int y) {
            return _tiles[x, y].ToArray();
        }

        public bool PlaceUnitAtTile(IUnit unit, int x, int y) {
            if (_unitMap.ContainsKey(unit.UnitId)) {
                _tiles[x % _grid.NumTilesX, y / _grid.NumTilesY].Remove(unit);
                _unitMap.Remove(unit.UnitId);
            }

            int tileIndex = (int)(System.Math.Max(0, y - 1) * _grid.NumTilesX + _grid.NumTilesY);
            _unitMap.Add(unit.UnitId, tileIndex);

            UnitPlacedAtTile.Invoke(unit, IntVector2.Of(x, y));
            return true;
        }
    }
}