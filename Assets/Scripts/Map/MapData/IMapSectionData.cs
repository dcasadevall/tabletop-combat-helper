using System;
using System.Collections.Generic;
using Grid.Serialized;
using Map.MapData.TileMetadata;
using Map.Rendering;
using Math;
using UniRx;
using UnityEngine;

namespace Map.MapData {
    public interface IMapSectionData {
        uint SectionIndex { get; }
        IGridData GridData { get; }
        string SectionName { get; }
        Sprite[] Sprites { get; }
        MapTileType MapTileType { get; }
        int PixelsPerUnit { get; }
        /// <summary>
        /// If set, coordinates of the tile where players will spawn in this section on map load.
        /// </summary>
        IntVector2? PlayerUnitSpawnPoint { get; }
        IObservable<IntVector2?> PlayerUnitSpawnPointChanged { get; }

        Dictionary<IntVector2, ITileMetadata> TileMetadataMap { get; }
        IObservable<Tuple<IntVector2, ITileMetadata>> TileMetadataChanged { get; }
    }
}