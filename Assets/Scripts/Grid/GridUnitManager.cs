using System.Collections.Generic;
using Logging;
using Math;
using Units;
using Units.Serialized;
using Zenject;

namespace Grid {
    /// <summary>
    /// Implementation of <see cref="IGridUnitManager"/> which uses an in memory map as a registry.
    /// </summary>
    internal class GridUnitManager : IGridUnitManager, IInitializable {
        public event System.Action<IUnit, IntVector2> UnitPlacedAtTile = delegate {};
        
        private Dictionary<UnitId, int> _unitMap = new Dictionary<UnitId, int>();
        private List<IUnit>[,] _tiles;
        private readonly IGrid _grid;
        private readonly IUnitRegistry _unitRegistry;
        private readonly ILogger _logger;

        public GridUnitManager(IGrid grid, IUnitRegistry unitRegistry, ILogger logger) {
            _grid = grid;
            _unitRegistry = unitRegistry;
            _logger = logger;
        }
        
        public void Initialize() {
            _tiles = new List<IUnit>[_grid.NumTilesX, _grid.NumTilesY];
            
            for (int x = 0; x < _grid.NumTilesX; x++) {
                for (int y = 0; y < _grid.NumTilesY; y++) {
                    _tiles[x, y] = new List<IUnit>();
                }
            }
        }

        public IUnit[] GetAllUnits(UnitType unitType) {
            List<IUnit> units = new List<IUnit>();
            foreach (var keyValuePair in _unitMap) {
                UnitId unitId = keyValuePair.Key;
                IUnit unit = _unitRegistry.GetUnit(unitId);
                if (unit.UnitData.UnitType == unitType) {
                    units.Add(unit);
                }
            }

            return units.ToArray();
        }

        public IUnit[] GetUnitsAtTile(IntVector2 tileCoords) {
            return _tiles[tileCoords.x, tileCoords.y].ToArray();
        }

        public bool PlaceUnitAtTile(IUnit unit, IntVector2 tileCoords) {
            if (_unitMap.ContainsKey(unit.UnitId)) {
                _tiles[tileCoords.x % _grid.NumTilesX, tileCoords.y / _grid.NumTilesY].Remove(unit);
                _unitMap.Remove(unit.UnitId);
            }

            int tileIndex = (int)(System.Math.Max(0, tileCoords.y) * _grid.NumTilesX + tileCoords.x);
            _unitMap.Add(unit.UnitId, tileIndex);

            UnitPlacedAtTile.Invoke(unit, tileCoords);
            return true;
        }

        public bool RemoveUnit(IUnit unit) {
            if (!_unitMap.ContainsKey(unit.UnitId)) {
                _logger.LogError(LoggedFeature.Grid, "Unit not found in grid: {0}", unit.UnitId);
                return false;
            }

            int tileIndex = _unitMap[unit.UnitId];
            _tiles[tileIndex % _grid.NumTilesX, tileIndex / _grid.NumTilesY].Remove(unit);
            _unitMap.Remove(unit.UnitId);

            return true;
        }
        
        public IntVector2? GetUnitCoords(IUnit unit) {
            if (!_unitMap.ContainsKey(unit.UnitId)) {
                _logger.LogError(LoggedFeature.Grid, "Unit not found in grid: {0}", unit.UnitId);
                return null;
            }
            
            int tileIndex = _unitMap[unit.UnitId];
            return IntVector2.Of((int)(tileIndex % _grid.NumTilesX), (int)(tileIndex / _grid.NumTilesY));
        }
    }
}