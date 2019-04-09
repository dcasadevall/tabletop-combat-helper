using Logging;

namespace Units.Serialized {
    public class UnitDataIndexResolver : IUnitDataIndexResolver {
        private readonly IUnitSpawnSettings _unitSpawnSettings;
        private readonly ILogger _logger;

        public UnitDataIndexResolver(IUnitSpawnSettings unitSpawnSettings, ILogger logger) {
            _unitSpawnSettings = unitSpawnSettings;
            _logger = logger;
        }
        
        public uint? ResolveUnitIndex(UnitType unitType, IUnitData unitData) {
            IUnitData[] unitDatas = _unitSpawnSettings.GetUnits(unitType);
            for (var i = unitDatas.Length - 1; i >= 0; i--) {
                if (unitDatas[i] == unitData) {
                    return (uint)i;
                }
            }

            return null;
        }

        public IUnitData ResolveUnitData(UnitType unitType, uint unitIndex) {
            IUnitData[] unitDatas = _unitSpawnSettings.GetUnits(unitType);
            if (unitIndex >= unitDatas.Length) {
                _logger.LogError(LoggedFeature.Units, "Invalid unit index: {0}", unitIndex.ToString());
                return null;
            }

            return unitDatas[(int)unitIndex];
        }
    }
}