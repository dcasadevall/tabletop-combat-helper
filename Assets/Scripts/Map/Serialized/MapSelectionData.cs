using UnityEngine;

namespace Map.Serialized {
    public class MapSelectionData : ScriptableObject {
        [SerializeField]
        public MapData[] mapDatas;
    }
}
