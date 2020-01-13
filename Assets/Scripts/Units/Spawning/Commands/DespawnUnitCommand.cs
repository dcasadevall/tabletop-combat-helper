using System;
using System.Collections.Generic;
using CommandSystem;
using Grid;
using Logging;
using Math;
using UniRx;
using Units.Serialized;

namespace Units.Spawning.Commands {
    public class DespawnUnitCommand : ICommand {
        private readonly DespawnUnitData _data;
        private readonly ICommandFactory _commandFactory;
        private readonly IMutableUnitRegistry _unitRegistry;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IUnitDataIndexResolver _indexResolver;
        private readonly IUnitPool _unitPool;
        private readonly ILogger _logger;

        public bool IsInitialGameStateCommand {
            get {
                return false;
            }
        }

        // Coordinates at which the unit was before despawned. This allows us to undo the command.
        private IntVector2? _tileCoords;
        private IUnit _unit;
        private List<ICommand> _petCommands = new List<ICommand>();

        public DespawnUnitCommand(DespawnUnitData data,
                                  ICommandFactory commandFactory,
                                  IMutableUnitRegistry unitRegistry,
                                  IGridUnitManager gridUnitManager,
                                  IUnitDataIndexResolver indexResolver,
                                  IUnitPool unitPool,
                                  ILogger logger) {
            _data = data;
            _commandFactory = commandFactory;
            _unitRegistry = unitRegistry;
            _gridUnitManager = gridUnitManager;
            _indexResolver = indexResolver;
            _unitPool = unitPool;
            _logger = logger;
        }

        public IObservable<UniRx.Unit> Run() {
            IUnit unit = _unitRegistry.GetUnit(_data.unitId);
            _tileCoords = _gridUnitManager.GetUnitCoords(unit);

            // Despawn pets first.
            // We need to directly run the command since these are children of the parent despawn command,
            // and should never be ran as standalone
            for (var i = 0; i < unit.PetUnits.Length; i++) {
                ICommand petDespawnCommand =
                    _commandFactory.Create(typeof(DespawnUnitCommand),
                                           typeof(DespawnUnitData),
                                           new DespawnUnitData(unit.PetUnits[i].UnitId));
                petDespawnCommand.Run();
                _petCommands.Add(petDespawnCommand);
            }

            // Now the unit itself.
            _unitPool.Despawn(unit.UnitId);
            _gridUnitManager.RemoveUnit(unit);

            return Observable.ReturnUnit();
        }

        public void Undo() {
            if (_unit == null) {
                _logger.LogError(LoggedFeature.Units, "Cannot undo Despawn command without previously running it.");
                return;
            }
            
            uint? index = _indexResolver.ResolveUnitIndex(_unit.UnitData);
            if (index == null) {
                _logger.LogError(LoggedFeature.Units, $"Unit index not resolved: {_unit.UnitData.Name}");
                return;
            }

            if (_tileCoords == null) {
                _logger.LogError(LoggedFeature.Units, "Cannot undo Despawn command without tileCoords.");
                return;
            }
            
            // Spawn the unit at the same coords it was despawned. This does not spawn pet units.
            IUnit unit = _unitPool.Spawn(_unit.UnitId, _unit.UnitData, _unit.PetUnits);
            _gridUnitManager.PlaceUnitAtTile(unit, _tileCoords.Value);
            
            // Undo pet command despawns.
            _petCommands.ForEach(x => x.Undo());
//
//            var commandData = new UnitCommandData(_unit.UnitId, index.Value, _unit.UnitData.UnitType);
//            // This needs to be created directly since the section command is dependant on this command.
//            SpawnUnitData spawnUnitData = new SpawnUnitData(commandData, _tileCoords.Value, isInitialSpawn: false);
//            ICommand spawnUnitCommand = _commandFactory.Create(typeof(SpawnUnitCommand),
//                                                               typeof(SpawnUnitData),
//                                                               spawnUnitData);
//            spawnUnitCommand.Run();
        }
    }
}