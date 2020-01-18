using System;
using Math;
using UniRx;
using Units;
using UnityEngine;
using Zenject;

namespace Grid.GridUnits {
    public class GridUnitInputManager : IGridUnitInputManager {
        private readonly IGridInputManager _gridInputManager;
        private readonly IGridUnitManager _gridUnitManager;

        public IUnit[] UnitsAtMousePosition {
            get {
                if (!_gridInputManager.TileAtMousePosition.HasValue) {
                    return new IUnit[0];
                }

                return _gridUnitManager.GetUnitsAtTile(_gridInputManager.TileAtMousePosition.Value);
            }
        }

        public GridUnitInputManager(IGridInputManager gridInputManager, IGridUnitManager gridUnitManager) {
            _gridInputManager = gridInputManager;
            _gridUnitManager = gridUnitManager;
        }
    }
}