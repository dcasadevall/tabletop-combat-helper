using System;
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
        private readonly IUnitPickerViewController _unitPickerViewController;
        private readonly Texture2D _cursorTexture;

        public Texture2D CursorTexture {
            get {
                return _cursorTexture;
            }
        }

        public UnitMapEditorTool([Inject(Id = MapEditorInstaller.UNIT_TILES_CURSOR)]
                                 Texture2D cursorTexture, 
                                 IGridUnitManager gridUnitManager,
                                 IUnitPickerViewController unitPickerViewController) {
            _cursorTexture = cursorTexture;
            _gridUnitManager = gridUnitManager;
            _unitPickerViewController = unitPickerViewController;
        }

        public async UniTask Show(IntVector2 tileCoords) {
            // TODO: IDismissNotifyingViewController to be async
            bool viewControllerDismissed = false;
            Action dismissAction = () => viewControllerDismissed = true;

            _unitPickerViewController.ViewControllerDismissed += dismissAction;
            _unitPickerViewController.Show();
            await UniTask.WaitUntil(() => viewControllerDismissed);
            _unitPickerViewController.ViewControllerDismissed -= dismissAction;
        }

        public IMapElement MapElementAtTileCoords(IntVector2 tileCoords) {
            IUnit[] units = _gridUnitManager.GetUnitsAtTile(tileCoords);
            if (units.Length == 0) {
                return null;
            }

            return new UnitMapElement(_gridUnitManager, units[0]);
        }
    }
}