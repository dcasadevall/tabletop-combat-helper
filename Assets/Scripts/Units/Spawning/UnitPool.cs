using System;
using System.Collections.Generic;
using Grid;
using Logging;
using Math;
using Units.Serialized;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;
using Object = UnityEngine.Object;

namespace Units.Spawning {
    public class UnitPool : IUnitPool {
        private readonly Dictionary<UnitId, UnitBehaviour> _unitBehaviours = new Dictionary<UnitId, UnitBehaviour>();
        private readonly DiContainer _container;
        private readonly IMutableUnitRegistry _unitRegistry;
        private readonly ILogger _logger;

        public UnitPool(DiContainer container, 
                        IMutableUnitRegistry unitRegistry,
                        ILogger logger) {
            _container = container;
            _unitRegistry = unitRegistry;
            _logger = logger;
        }

        public IUnit Spawn(UnitId unitId, IUnitData unitData, IUnit[] pets) {
            // Create Behaviour
            UnitBehaviour unitBehaviour = _container.Instantiate<UnitBehaviour>();
            _unitBehaviours[unitId] = unitBehaviour;

            // Create Unit and register it
            IUnit unit = new Unit(unitId, unitData, pets, _unitBehaviours[unitId].transform);
            unitBehaviour.SetUnit(unit);
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