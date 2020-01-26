using System;
using System.Threading;
using CommandSystem;
using Grid;
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
                                 IUnitSpawnViewController unitSpawnViewController) {
            _cursorTexture = cursorTexture;
            _gridUnitManager = gridUnitManager;
            _commandQueue = commandQueue;
            _unitSpawnViewController = unitSpawnViewController;
        }

        public UniTask Show(IntVector2 tileCoords, CancellationToken cancellationToken) {
            return _unitSpawnViewController.Show(tileCoords, cancellationToken);
        }

        public IMapElement MapElementAtTileCoords(IntVector2 tileCoords) {
            IUnit[] units = _gridUnitManager.GetUnitsAtTile(tileCoords);
            if (units.Length == 0) {
                return null;
            }

            return new UnitMapElement(_commandQueue, units[0]);
        }
    }
}