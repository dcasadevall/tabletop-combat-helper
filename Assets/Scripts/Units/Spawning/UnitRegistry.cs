using System.Collections.Generic;
using Logging;

namespace Units.Spawning {
    public class UnitRegistry : IMutableUnitRegistry, IUnitTransformRegistry {
        private readonly ILogger _logger;
        private Dictionary<UnitId, IUnit> _unitMap = new Dictionary<UnitId, IUnit>();

        public UnitRegistry(ILogger logger) {
            _logger = logger;
        }

        public IEnumerable<IUnit> GetAllUnits() {
            return _unitMap.Values;
        }

        public IUnit GetUnit(UnitId unitId) {
            if (!_unitMap.ContainsKey(unitId)) {
                _logger.LogError(LoggedFeature.Units, "Unit not found in registry: {0}", unitId);
                return null;
            }

            return _unitMap[unitId];
        }
        
        public ITransformableUnit GetTransformableUnit(UnitId unitId) {
            return (ITransformableUnit) GetUnit(unitId);
        }

        public void RegisterUnit(IUnit unit) {
            _unitMap[unit.UnitId] = unit;
        }
        
        public void UnregisterUnit(UnitId unitId) {
            if (!_unitMap.ContainsKey(unitId)) {
                _logger.LogError(LoggedFeature.Units, "Unit not found in registry: {0}", unitId);
                return;
            }
            
            _unitMap.Remove(unitId);
        }
    }
}