using System;
using System.Collections.Generic;
using CommandSystem;
using Grid;
using Grid.Commands;
using Map.MapData;
using Math;
using UniRx.Async;
using Zenject;

namespace Units.Movement {
    public class MapSectionUnitMover : IInitializable, IDisposable {
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IMapSectionData _mapSectionData;
        private readonly ICommandQueue _commandQueue;
        private static readonly HashSet<UnitId> _unitsOnCooldown = new HashSet<UnitId>();

        public MapSectionUnitMover(IGridUnitManager gridUnitManager,
                                   IMapSectionData mapSectionData,
                                   ICommandQueue commandQueue) {
            _gridUnitManager = gridUnitManager;
            _mapSectionData = mapSectionData;
            _commandQueue = commandQueue;
        }

        public void Initialize() {
            _gridUnitManager.UnitPlacedAtTile += HandleUnitPlacedAtTile;
        }

        public void Dispose() {
            _gridUnitManager.UnitPlacedAtTile -= HandleUnitPlacedAtTile;
        }

        private async void HandleUnitPlacedAtTile(IUnit unit, IntVector2 tileCoords) {
            if (!_mapSectionData.TileMetadataMap.ContainsKey(tileCoords)) {
                return;
            }

            if (_mapSectionData.TileMetadataMap[tileCoords].SectionConnection == null) {
                return;
            }

            // Avoid moving the same unit more than once in a few frames.
            // This is so we don't move a unit to a new section, then immediately back to the original one.
            if (_unitsOnCooldown.Contains(unit.UnitId)) {
                return;
            }

            _unitsOnCooldown.Add(unit.UnitId);
            uint sectionIndex = _mapSectionData.TileMetadataMap[tileCoords].SectionConnection.Value;
            var commandData = new MoveUnitSectionCommandData(unit.UnitId,
                                                             _mapSectionData.SectionIndex,
                                                             sectionIndex);
            _commandQueue.Enqueue<MoveUnitSectionCommand, MoveUnitSectionCommandData>(commandData,
                                                                                      CommandSource.Game);
            await UniTask.Delay(TimeSpan.FromMilliseconds(500));
            _unitsOnCooldown.Remove(unit.UnitId);
        }
    }
}