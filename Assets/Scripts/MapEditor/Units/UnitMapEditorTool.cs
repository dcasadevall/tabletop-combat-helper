using System;
using System.Threading;
using CommandSystem;
using Grid;
using Map.MapData;
using MapEditor.MapElement;
using MapEditor.SingleTileEditor;
using Math;
using UniRx.Async;
using Units;
using Units.Spawning.UI;
using UnityEngine;
using Zenject;

namespace MapEditor.Units {
    public class UnitMapEditorTool : ISingleTileMapEditorToolDelegate {
        private readonly IGridUnitManager _gridUnitManager;
        private readonly ICommandQueue _commandQueue;
        private readonly IUnitSpawnViewController _unitSpawnViewController;
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
                                 IMutableMapSectionData mapSectionData) {
            _cursorTexture = cursorTexture;
            _gridUnitManager = gridUnitManager;
            _commandQueue = commandQueue;
            _unitSpawnViewController = unitSpawnViewController;
            _mapSectionData = mapSectionData;
        }

        public async UniTask Show(IntVector2 tileCoords, CancellationToken cancellationToken) {
            _gridUnitManager.UnitPlacedAtTile += HandleUnitPlacedAtTile;
            await _unitSpawnViewController.Show(tileCoords, cancellationToken);
            _gridUnitManager.UnitPlacedAtTile -= HandleUnitPlacedAtTile;
        }
        
        private void HandleUnitPlacedAtTile(IUnit unit, IntVector2 tileCoords) {
            _mapSectionData.AddInitialUnit(tileCoords, unit.UnitData);
        }

        public IMapElement MapElementAtTileCoords(IntVector2 tileCoords) {
            IUnit[] units = _gridUnitManager.GetUnitsAtTile(tileCoords);
            if (units.Length == 0) {
                return null;
            }

            return new UnitMapElement(_commandQueue, units[0], tileCoords, _mapSectionData);
        }
    }
}