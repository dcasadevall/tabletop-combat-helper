using System.Collections.Generic;
using Logging;
using Units.Serialized;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.Spawning {
    /// <summary>
    /// Spawns units, and registers them to the <see cref="IUnitRegistry"/>.
    /// </summary>
    public class UnitPool : IUnitPool {
        private readonly Dictionary<UnitId, UnitInitializer> _unitBehaviours = new Dictionary<UnitId, UnitInitializer>();
        private readonly DiContainer _container;
        private readonly GameObject _unitPrefab;
        private readonly IMutableUnitRegistry _unitRegistry;
        private readonly ILogger _logger;

        public UnitPool(DiContainer container, 
                        GameObject unitPrefab,
                        IMutableUnitRegistry unitRegistry,
                        ILogger logger) {
            _container = container;
            _unitPrefab = unitPrefab;
            _unitRegistry = unitRegistry;
            _logger = logger;
        }

        public IUnit Spawn(UnitId unitId, IUnitData unitData, IUnit[] pets) {
            // Create Behaviour
            UnitInitializer unitInitializer = _container.InstantiatePrefabForComponent<UnitInitializer>(_unitPrefab);
            _unitBehaviours[unitId] = unitInitializer;

            // Create Unit and register it
            IUnit unit = new Unit(unitId, unitData, pets, _unitBehaviours[unitId].transform);
            unitInitializer.SetUnit(unit);
            _unitRegistry.RegisterUnit(unit);

            return unit;
        }

        public void Despawn(UnitId unitId) {
            if (!_unitBehaviours.ContainsKey(unitId)) {
                _logger.LogError(LoggedFeature.Units, "Despawn called on unitId not found: {0}", unitId);
                return;
            }

            _unitRegistry.UnregisterUnit(unitId);
            Object.Destroy(_unitBehaviours[unitId]);
        }
    }
}