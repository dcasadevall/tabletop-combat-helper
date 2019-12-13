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
        private readonly Dictionary<UnitId, UnitRenderer> _unitBehaviours = new Dictionary<UnitId, UnitRenderer>();
        private readonly UnitRenderer.Pool _unitRendererPool;
        private readonly IMutableUnitRegistry _unitRegistry;
        private readonly ILogger _logger;

        internal UnitPool(UnitRenderer.Pool unitRendererPool,
                          IMutableUnitRegistry unitRegistry,
                          ILogger logger) {
            _unitRendererPool = unitRendererPool;
            _unitRegistry = unitRegistry;
            _logger = logger;
        }

        public IUnit Spawn(UnitId unitId, IUnitData unitData, IUnit[] pets) {
            // Create Initializer
            UnitRenderer unitRenderer = _unitRendererPool.Spawn();
            _unitBehaviours[unitId] = unitRenderer;

            // Create Unit and register it
            IUnit unit = new Unit(unitId, unitData, pets, _unitBehaviours[unitId].transform);
            unitRenderer.SetUnit(unit);
            _unitRegistry.RegisterUnit(unit);

            return unit;
        }

        public void Despawn(UnitId unitId) {
            if (!_unitBehaviours.ContainsKey(unitId)) {
                _logger.LogError(LoggedFeature.Units, "Despawn called on unitId not found: {0}", unitId);
                return;
            }

            _unitRegistry.UnregisterUnit(unitId);
            _unitRendererPool.Despawn(_unitBehaviours[unitId]);
        }
    }
}