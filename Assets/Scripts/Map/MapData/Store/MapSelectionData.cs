using UnityEngine;

namespace Map.MapData.Store {
    public class MapSelectionData : ScriptableObject {
        [SerializeField]
        public AddressableAssetMapReference[] mapReferences;
    }
}
