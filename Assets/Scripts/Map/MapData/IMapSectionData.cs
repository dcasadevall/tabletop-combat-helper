using System;
using System.Collections.Generic;
using Grid.Serialized;
using Map.MapData.TileMetadata;
using Map.Rendering;
using Math;
using UnityEngine;

namespace Map.MapData {
    public interface IMapSectionData {
        IGridData GridData { get; }
        String SectionName { get; }
        Sprite[] Sprites { get; }
        MapTileType MapTileType { get; }
        int PixelsPerUnit { get; }
        
        IObservable<Tuple<IntVector2, ITileMetadata>> TileMetadataChanged { get; }
        Dictionary<IntVector2, ITileMetadata> TileMetadataMap { get; }
    }
}