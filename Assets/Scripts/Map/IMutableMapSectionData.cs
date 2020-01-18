using System.Collections.Generic;
using Map.Serialized;
using Math;

namespace Map {
    public interface IMutableMapSectionData {
        Dictionary<IntVector2, TileMetadata> TileMetadataMap { get; }
    }
}