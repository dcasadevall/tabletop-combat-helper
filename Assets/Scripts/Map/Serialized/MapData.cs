using Grid.Serialized;
using Map.Rendering;
using UnityEngine;

namespace Map.Serialized {
    public class MapData : ScriptableObject, IMapData {
        public GridData gridData;
        public IGridData GridData {
            get {
                return gridData;
            }
        }

        public string name;
        public string Name {
            get {
                return name;
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
    }
}