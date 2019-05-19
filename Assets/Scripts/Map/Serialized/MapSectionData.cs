using System;
using Grid.Serialized;
using Map.Rendering;
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
    }
}