using Logging;
using Units.Serialized;
using Zenject;

namespace Units.Commands {
    public class UnitCommandDataFactory : IFactory<IUnitData, UnitCommandData> {
        private readonly IUnitDataIndexResolver _unitDataIndexResolver;
        private readonly ILogger _logger;

        public UnitCommandDataFactory(IUnitDataIndexResolver unitDataIndexResolver, ILogger logger) {
            _unitDataIndexResolver = unitDataIndexResolver;
            _logger = logger;
        }
        
        public UnitCommandData Create(IUnitData unitData) {
            uint? unitIndex = _unitDataIndexResolver.ResolveUnitIndex(unitData);
            if (unitIndex == null) {
                _logger.LogError(LoggedFeature.Units,
                                 "Error Spawning unit with name: {0}. Index not resolved.",
                                 unitData);
                return null;
            }
            
            UnitCommandData[] petData = new UnitCommandData[unitData.Pets.Length];
            for (int i = 0; i < unitData.Pets.Length; i++) {
                petData[i] = Create(unitData.Pets[i]);
            }
            
            return new UnitCommandData(new UnitId(), unitIndex.Value, unitData.UnitType, petData);
        }
    }
}