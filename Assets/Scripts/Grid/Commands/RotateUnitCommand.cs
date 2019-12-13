using System;
using CommandSystem;
using Logging;
using UniRx;
using Units;
using Units.Spawning;
using UnityEngine;
using ILogger = Logging.ILogger;
using Unit = UniRx.Unit;

namespace Grid.Commands {
    public class RotateUnitCommand : ICommand {
        private readonly RotateUnitData _data;
        private readonly IUnitTransformRegistry _unitRegistry;
        private readonly ILogger _logger;

        public bool IsInitialGameStateCommand {
            get {
                return false;
            }
        }

        public RotateUnitCommand(RotateUnitData data, IUnitTransformRegistry unitRegistry, ILogger logger) {
            _data = data;
            _unitRegistry = unitRegistry;
            _logger = logger;
        }

        public IObservable<Unit> Run() {
            ITransformableUnit unit = _unitRegistry.GetTransformableUnit(_data.unitId);
            if (unit == null) {
                string errorMsg = $"Unit not found in registry: {_data.unitId}";
                _logger.LogError(LoggedFeature.Units, errorMsg);
                return Observable.Throw<Unit>(new Exception(errorMsg));
            }

            // TODO: Maybe encapsulate rotation of units
            unit.Transform.Rotate(Vector3.forward, _data.degrees);
            return Observable.ReturnUnit();
        }

        public void Undo() {
            ITransformableUnit unit = _unitRegistry.GetTransformableUnit(_data.unitId);
            if (unit == null) {
                string errorMsg = $"Unit not found in registry: {_data.unitId}";
                _logger.LogError(LoggedFeature.Units, errorMsg);
                return;
            }

            // TODO: Maybe encapsulate rotation of units
            unit.Transform.Rotate(Vector3.forward, -_data.degrees);
        }
    }
}