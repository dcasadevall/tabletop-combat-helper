using System;
using Grid.Serialized;
using Map.Rendering;
using UnityEngine;

namespace Map {
    public interface IMapSectionData {
        IGridData GridData { get; }
        String SectionName { get; }
        Sprite[] Sprites { get; }
        MapTileType MapTileType { get; }
        int PixelsPerUnit { get; }
    }
}