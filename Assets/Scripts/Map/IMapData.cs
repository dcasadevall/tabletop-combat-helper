using System;
using Grid.Serialized;
using Map.Rendering;
using UnityEngine;

namespace Map {
    public interface IMapData {
        IGridData GridData { get; }
        String Name { get; }
        Sprite[] Sprites { get; }
        MapTileType MapTileType { get; }
        int PixelsPerUnit { get; }
    }
}