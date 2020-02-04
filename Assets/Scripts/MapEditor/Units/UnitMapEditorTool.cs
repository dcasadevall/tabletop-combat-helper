using System.Threading;
using CommandSystem;
using Grid;
using Logging;
using Map.MapData;
using MapEditor.MapElement;
using MapEditor.SingleTileEditor;
using Math;
using UniRx.Async;
using Units;
using Units.Serialized;
using Units.Spawning.UI;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace MapEditor.Units {
    public class UnitMapEditorTool : ISingleTileMapEditorToolDelegate {
        private readonly IGridUnitManager _gridUnitManager;
        private readonly ICommandQueue _commandQueue;
        private readonly IUnitSpawnViewController _unitSpawnViewController;
        private readonly ILogger _logger;
        private readonly IUnitDataIndexResolver _unitDataIndexResolver;
        private readonly IMutableMapSectionData _mapSectionData;
        private readonly Texture2D _cursorTexture;

        public Texture2D CursorTexture {
            get {
                return _cursorTexture;
            }
        }

        public UnitMapEditorTool([Inject(Id = MapEditorInstaller.UNIT_TILES_CURSOR)]
                                 Texture2D cursorTexture,
                                 IGridUnitManager gridUnitManager,
                                 ICommandQueue commandQueue,
                                 IUnitSpawnViewController unitSpawnViewController,
                                 ILogger logger,
                                 IUnitDataIndexResolver unitDataIndexResolver,
                                 IMutableMapSectionData mapSectionData) {
            _cursorTexture = cursorTexture;
            _gridUnitManager = gridUnitManager;
            _commandQueue = commandQueue;
            _unitSpawnViewController = unitSpawnViewController;
            _logger = logger;
            _unitDataIndexResolver = unitDataIndexResolver;
            _mapSectionData = mapSectionData;
        }

        public async UniTask Show(IntVector2 tileCoords, CancellationToken cancellationToken) {
            _gridUnitManager.UnitPlacedAtTile += HandleUnitPlacedAtTile;
            await _unitSpawnViewController.Show(tileCoords, cancellationToken);
            // We need to wait 1 frame because of command queue not being immediately triggered :/
            await UniTask.DelayFrame(1);
            _gridUnitManager.UnitPlacedAtTile -= HandleUnitPlacedAtTile;
        }

        private void HandleUnitPlacedAtTile(IUnit unit, IntVector2 tileCoords) {
            uint? unitIndex = _unitDataIndexResolver.ResolveUnitIndex(unit.UnitData);
            if (unitIndex == null) {
                _logger.LogError(LoggedFeature.Units,
                                 "Error adding unit with name: {0}. Index not resolved.",
                                 unit.UnitData.Name);
                return;
            }

            var unitDataReference = new UnitDataReference(unitIndex.Value, unit.UnitData.UnitType);
            _mapSectionData.AddInitialUnit(tileCoords, unitDataReference);
        }

        public IMapElement MapElementAtTileCoords(IntVector2 tileCoords) {
            IUnit[] units = _gridUnitManager.GetUnitsAtTile(tileCoords);
            if (units.Length == 0) {
                return null;
            }

            return
                new UnitMapElement(_commandQueue,
                                   units[0],
                                   tileCoords,
                                   _logger,
                                   _mapSectionData,
                                   _unitDataIndexResolver);
        }
    }
}