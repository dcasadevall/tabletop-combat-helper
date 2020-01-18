using System;
using System.Collections.Generic;
using Grid.Serialized;
using Map.Rendering;
using Math;
using UnityEngine;

namespace Map {
    public interface IMapSectionData {
        IGridData GridData { get; }
        String SectionName { get; }
        Sprite[] Sprites { get; }
        MapTileType MapTileType { get; }
        int PixelsPerUnit { get; }
        Dictionary<IntVector2, ITileMetadata> TileMetadataMap { get; }
    }
}