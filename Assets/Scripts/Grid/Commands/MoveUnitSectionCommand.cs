using System;
using CommandSystem;
using Logging;
using Map.MapData.Store;
using Map.MapData.Store.Commands;
using Map.MapSections;
using Map.MapSections.Commands;
using Math;
using UniRx;
using Units;
using Units.Serialized;
using Units.Spawning;
using Units.Spawning.Commands;
using Zenject;
using Unit = UniRx.Unit;

namespace Grid.Commands {
    public class MoveUnitSectionCommand : ICommand {
        public bool IsInitialGameStateCommand => false;
        private readonly MoveUnitSectionCommandData _data;
        private readonly ICommandFactory _commandFactory;
        private readonly IUnitRegistry _unitRegistry;
        private readonly IMapSectionEntryTileFinder _entryTileFinder;
        private readonly IUnitDataIndexResolver _unitDataIndexResolver;
        private readonly ILogger _logger;
        private readonly MapStoreId _mapStoreId;

        private ICommand _despawnCommand;
        private ICommand _spawnCommand;

        public MoveUnitSectionCommand(MoveUnitSectionCommandData data,
                                      ICommandFactory commandFactory,
                                      IUnitRegistry unitRegistry,
                                      IMapSectionEntryTileFinder entryTileFinder,
                                      IUnitDataIndexResolver unitDataIndexResolver,
                                      ILogger logger,
                                      MapStoreId mapStoreId) {
            _data = data;
            _commandFactory = commandFactory;
            _unitRegistry = unitRegistry;
            _entryTileFinder = entryTileFinder;
            _unitDataIndexResolver = unitDataIndexResolver;
            _logger = logger;
            _mapStoreId = mapStoreId;
        }

        public IObservable<Unit> Run() {
            IUnit unit = _unitRegistry.GetUnit(_data.unitId);
            if (unit == null) {
                _logger.LogError(LoggedFeature.Units,
                                 "MoveUnitSectionCommand called on unit not in registry: {0}",
                                 _data.unitId);
                return Observable.Empty<Unit>();
            }

            uint? unitIndex = _unitDataIndexResolver.ResolveUnitIndex(unit.UnitData);
            if (unitIndex == null) {
                _logger.LogError(LoggedFeature.Units,
                                 "Failed to resolve unit index: {0}",
                                 _data.unitId);
                return Observable.Empty<Unit>();
            }

            _despawnCommand =
                _commandFactory.Create<DespawnUnitCommand, DespawnUnitData>(new DespawnUnitData(_data.unitId));
            _despawnCommand.Run();

            ICommand loadMapSectionCommand = _commandFactory.Create(typeof(LoadMapSectionCommand),
                                                                    typeof(LoadMapSectionCommandData),
                                                                    new LoadMapSectionCommandData(_data.toSectionIndex,
                                                                                                  new
                                                                                                      LoadMapCommandData(_mapStoreId
                                                                                                                             .index)));
            Subject<Unit> sectionLoadedSubject = new Subject<Unit>();
            loadMapSectionCommand.Run().Subscribe(_ => {
                // We need to wait 1 frame in order to avoid race conditions between listeners on new section
                Observable.IntervalFrame(1).First().Subscribe(__ => {
                    IntVector2 entryTileCoords =
                        _entryTileFinder.GetEntryTile(_data.toSectionIndex, _data.fromSectionIndex);
                    // We don't spawn pets (which also are not despawn on despawn command)
                    var unitCommandData = new UnitCommandData(unit.UnitId, unitIndex.Value, unit.UnitData.UnitType);
                    _spawnCommand = _commandFactory.Create(typeof(SpawnUnitCommand),
                                                           typeof(SpawnUnitData),
                                                           new SpawnUnitData(unitCommandData,
                                                                             entryTileCoords,
                                                                             isInitialSpawn: false));
                    _spawnCommand.Run();
                    sectionLoadedSubject.OnNext(Unit.Default);
                });
            });

            return sectionLoadedSubject;
        }

        public void Undo() {
            ICommand loadMapSectionCommand = _commandFactory.Create(typeof(LoadMapSectionCommand),
                                                                    typeof(LoadMapSectionCommandData),
                                                                    new
                                                                        LoadMapSectionCommandData(_data
                                                                                                      .fromSectionIndex,
                                                                                                  new
                                                                                                      LoadMapCommandData(_mapStoreId
                                                                                                                             .index)));
            _spawnCommand.Undo();
            loadMapSectionCommand.Run().Subscribe(_ => {
                _despawnCommand.Undo();
            });
        }
    }
}