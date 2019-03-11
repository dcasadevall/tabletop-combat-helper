using Grid.Serialized;
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

        public Sprite backgroundSprite;
        public Sprite BackgroundSprite {
            get {
                return backgroundSprite;
            }
        }
    }
}