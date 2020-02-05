using System.Threading;
using CommandSystem;
using Grid;
using Map.MapData;
using MapEditor.MapElement;
using MapEditor.SingleTileEditor;
using MapEditor.Units;
using Math;
using UniRx.Async;
using Units;
using Units.Spawning.UI;
using UnityEngine;
using Zenject;

namespace MapEditor.PlayerUnits {
    public class PlayerUnitsMapEditorTool : ISingleTileMapEditorToolDelegate {
        private readonly IMutableMapSectionData _mapSectionData;
        private readonly Texture2D _cursorTexture;

        public Texture2D CursorTexture {
            get {
                return _cursorTexture;
            }
        }

        public PlayerUnitsMapEditorTool([Inject(Id = MapEditorInstaller.PLAYER_UNITS_TILES_CURSOR)]
                                        Texture2D cursorTexture,
                                        IMutableMapSectionData mapSectionData) {
            _cursorTexture = cursorTexture;
            _mapSectionData = mapSectionData;
        }

        public async UniTask Show(IntVector2 tileCoords, CancellationToken cancellationToken) {
            _mapSectionData.SetPlayerUnitSpawnPoint(tileCoords);
        }

        public IMapElement MapElementAtTileCoords(IntVector2 tileCoords) {
            if (_mapSectionData.PlayerUnitSpawnPoint == null) {
                return null;
            }

            if (_mapSectionData.PlayerUnitSpawnPoint.Value != tileCoords) {
                return null;
            }

            return new PlayerUnitsMapElement(_mapSectionData);
        }
    }
}