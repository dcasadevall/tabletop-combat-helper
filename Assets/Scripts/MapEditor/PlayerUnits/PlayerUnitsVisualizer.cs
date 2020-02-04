using System;
using Map.MapData;
using MapEditor.MapElement;
using Math;
using UniRx;
using UnityEngine;
using Zenject;

namespace MapEditor.PlayerUnits {
    /// <summary>
    /// Class responsible for showing the initial player units in the map editor.
    /// </summary>
    internal class PlayerUnitsTileVisualizer : IInitializable, IDisposable {
        private readonly IMapSectionData _mapSectionData;
        private readonly Sprite _sprite;
        private readonly MapElementTileRenderer.Pool _tileRendererPool;

        private IntVector2? _spawnCoords;
        private IDisposable _observer;

        public PlayerUnitsTileVisualizer(IMapSectionData mapSectionData,
                                         Sprite sprite,
                                         MapElementTileRenderer.Pool tileRendererPool) {
            _mapSectionData = mapSectionData;
            _sprite = sprite;
            _tileRendererPool = tileRendererPool;
        }

        public void Initialize() {
            _observer = _mapSectionData.PlayerUnitSpawnPointChanged
                                       .StartWith(_mapSectionData.PlayerUnitSpawnPoint)
                                       .Subscribe(HandlePlayerUnitSpawnPointChanged);
        }

        public void Dispose() {
            _observer?.Dispose();
            _observer = null;
        }

        private void HandlePlayerUnitSpawnPointChanged(IntVector2? playerUnitSpawnPoint) {
            if (_spawnCoords != null) {
                _tileRendererPool.Despawn(_spawnCoords.Value);
            }

            _spawnCoords = playerUnitSpawnPoint;
            if (playerUnitSpawnPoint != null) {
                _tileRendererPool.Spawn(playerUnitSpawnPoint.Value, _sprite, "Players");
            }
        }
    }
}