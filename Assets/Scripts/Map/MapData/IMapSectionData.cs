using System;
using System.Collections.Generic;
using Grid.Serialized;
using Map.MapData.TileMetadata;
using Map.Rendering;
using Math;
using UniRx;
using Units.Serialized;
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
        Dictionary<IntVector2, ITileMetadata> TileMetadataMap { get; }
        
        #region Events
        IObservable<IntVector2?> PlayerUnitSpawnPointChanged { get; }
        IObservable<Tuple<IntVector2, uint?>> SectionConnectionChanged { get; }
        IObservable<Tuple<IntVector2, UnitDataReference>> UnitAdded { get; }
        IObservable<Tuple<IntVector2, UnitDataReference>> UnitRemoved { get; }
        #endregion
    }
}