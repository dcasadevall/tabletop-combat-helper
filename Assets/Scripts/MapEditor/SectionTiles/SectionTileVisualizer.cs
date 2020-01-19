using System;
using Map;
using Zenject;

namespace MapEditor.SectionTiles {
    public class SectionTileVisualizer : IInitializable, IDisposable {
        private readonly IMapSectionData _mapSectionData;
        public SectionTileVisualizer(IMapSectionData mapSectionData) {
            _mapSectionData = mapSectionData;
        }

        public void Initialize() {
            
        }

        public void Dispose() {
        }
    }
}