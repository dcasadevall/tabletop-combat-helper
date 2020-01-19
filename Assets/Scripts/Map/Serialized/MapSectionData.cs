using System;
using System.Collections.Generic;
using System.Linq;
using Grid.Serialized;
using Map.Rendering;
using Math;
using UnityEngine;

namespace Map.Serialized {
    [Serializable]
    public class MapSectionData : IMapSectionData {
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

        public List<TileMetadataPair> tileMetadataPairs = new List<TileMetadataPair>();
        public Dictionary<IntVector2, ITileMetadata> TileMetadataMap {
            get {
                return tileMetadataPairs.ToDictionary(x => IntVector2.Of(x.tileCoords),
                                                      x => (ITileMetadata) x.tileMetadata);
            }
        }
    }
}