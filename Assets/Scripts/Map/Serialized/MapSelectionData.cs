using UnityEngine;

namespace Map.Serialized {
    public class MapSelectionData : ScriptableObject {
        [SerializeField]
        public MapReference[] mapReferences;
    }
}
