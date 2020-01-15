using System.Collections.Generic;
using Grid.Positioning;
using Logging;
using Math;
using Units;
using Units.Serialized;
using Units.Spawning;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace Grid {
    /// <summary>
    /// Implementation of <see cref="IGridUnitManager"/> which uses an in memory map as a registry.
    /// </summary>
    internal class GridUnitManager : IGridUnitManager, IInitializable {
        public event System.Action<IUnit, IntVector2> UnitPlacedAtTile = delegate {};
        public event System.Action<IUnit, IntVector2> UnitRemovedFromTile = delegate {};
        
        private Dictionary<UnitId, int> _unitMap = new Dictionary<UnitId, int>();
        private List<IUnit>[,] _tiles;
        private readonly IGrid _grid;
        private readonly IGridPositionCalculator _gridPositionCalculator;
        private readonly IUnitTransformRegistry _unitTransformRegistry;
        private readonly ILogger _logger;

        public GridUnitManager(IGrid grid, 
                               IGridPositionCalculator gridPositionCalculator,
                               IUnitTransformRegistry unitTransformRegistry, 
                               ILogger logger) {
            _grid = grid;
            _gridPositionCalculator = gridPositionCalculator;
            _unitTransformRegistry = unitTransformRegistry;
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

        public IUnit[] GetAllUnits(UnitType unitType, IUnitRegistry unitRegistry) {
            List<IUnit> units = new List<IUnit>();
            foreach (var keyValuePair in _unitMap) {
                UnitId unitId = keyValuePair.Key;
                IUnit unit = unitRegistry.GetUnit(unitId);
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
            // Remove previous unit position if present
            if (_unitMap.ContainsKey(unit.UnitId)) {
                RemoveUnit(unit);
            }

            // Add unit to new position.
            _tiles[tileCoords.x, tileCoords.y].Add(unit);
            int tileIndex = (int)(System.Math.Max(0, tileCoords.y) * _grid.NumTilesX + tileCoords.x);
            _unitMap.Add(unit.UnitId, tileIndex);
            
            // Move unit in 3D space.
            Transform unitTransform = _unitTransformRegistry.GetTransformableUnit(unit.UnitId).Transform;
            Vector2 worldPosition = _gridPositionCalculator.GetTileCenterWorldPosition(tileCoords);
            unitTransform.position = new Vector3(worldPosition.x, worldPosition.y, unitTransform.position.z);

            // Notify listeners
            UnitPlacedAtTile.Invoke(unit, tileCoords);
            return true;
        }

        public bool RemoveUnit(IUnit unit) {
            if (!_unitMap.ContainsKey(unit.UnitId)) {
                _logger.LogError(LoggedFeature.Grid, "Unit not found in grid: {0}", unit.UnitId);
                return false;
            }

            // Remove unit from our unit / tile caches.
            int tileIndex = _unitMap[unit.UnitId];
            IntVector2 tileCoords = IntVector2.Of((int)(tileIndex % _grid.NumTilesX), (int)(tileIndex / _grid.NumTilesY));
            _tiles[tileCoords.x, tileCoords.y].Remove(unit);
            _unitMap.Remove(unit.UnitId);

            // Notify listeners.
            UnitRemovedFromTile.Invoke(unit, tileCoords);
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