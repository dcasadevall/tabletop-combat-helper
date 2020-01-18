using System;
using System.Collections.Generic;
using Grid.Serialized;
using Map.Rendering;
using Math;
using UnityEngine;

namespace Map.Serialized {
    [Serializable]
    public class MapSectionData : IMapSectionData, IMutableMapSectionData {
        public GridData gridData;
        public IGridData GridData {
            get {
                return gridData;
            }
        }

        public string sectionName;
        public string SectionName {
            get {
                return sectionName;
            }
        }

        public Sprite[] sprites;
        public Sprite[] Sprites {
            get {
                return sprites;
            }
        }

        public MapTileType generationType;
        public MapTileType MapTileType {
            get {
                return generationType;
            }
        }

        public int PixelsPerUnit {
            get {
                return 1;
            }
        }

        public TileMetadataPair[] tileMetadataPairs;
        public Dictionary<IntVector2, ITileMetadata> TileMetadataMap {
            get {
                
            }
        }
    }
}