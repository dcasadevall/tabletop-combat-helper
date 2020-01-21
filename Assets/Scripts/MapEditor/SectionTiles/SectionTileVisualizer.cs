using System;
using System.Collections.Generic;
using Logging;
using Map;
using Map.MapData;
using Map.MapData.TileMetadata;
using Math;
using UniRx;
using Zenject;

namespace MapEditor.SectionTiles {
    internal class SectionTileVisualizer : IInitializable, IDisposable {
        private readonly IMapData _mapData;
        private readonly IMapSectionData _mapSectionData;
        private readonly SectionTileRenderer.Pool _tileRendererPool;
        private readonly ILogger _logger;

        private IDisposable _observer;

        public SectionTileVisualizer(IMapData mapData,
                                     IMapSectionData mapSectionData,
                                     SectionTileRenderer.Pool tileRendererPool, 
                                     ILogger logger) {
            _mapData = mapData;
            _mapSectionData = mapSectionData;
            _tileRendererPool = tileRendererPool;
            _logger = logger;
        }

        public void Initialize() {
            foreach (var kvp in _mapSectionData.TileMetadataMap) {
                HandleTileMetadataChanged(kvp.Key, kvp.Value);
            }

            _observer =
                _mapSectionData.TileMetadataChanged
                               .Subscribe(Observer.Create<Tuple<IntVector2, ITileMetadata>>(tuple => {
                                   HandleTileMetadataChanged(tuple.Item1, tuple.Item2);
                               }));
        }

        public void Dispose() {
            _observer?.Dispose();
            _observer = null;
        }

        private void HandleTileMetadataChanged(IntVector2 tileCoords, ITileMetadata tileMetadata) {
            if (tileMetadata.SectionConnection == null) {
                _tileRendererPool.Despawn(tileCoords);
                return;
            }

            if (tileMetadata.SectionConnection.Value >= _mapData.Sections.Length) {
                _logger.LogError(LoggedFeature.MapEditor,
                                 "Metadata change on SectionConnection out of bounds: {0}",
                                 tileMetadata.SectionConnection.Value);
                return;
            }

            string sectionName = _mapData.Sections[tileMetadata.SectionConnection.Value].SectionName;
            _tileRendererPool.Spawn(tileCoords, sectionName);
        }
    }
}