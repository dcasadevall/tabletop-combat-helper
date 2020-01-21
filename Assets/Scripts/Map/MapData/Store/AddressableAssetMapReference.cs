using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Map.MapData.Store {
    [Serializable]
    public class AddressableAssetMapReference : IMapReference {
        [SerializeField]
        private string _name;
        public string MapName {
            get {
                return _name;
            }
        }

        [SerializeField]
        private AssetReference _mapData;
        public AssetReference AssetReference {
            get {
                return _mapData;
            }
        }
        
        public MapStoreId MapStoreId { get; }

        public AddressableAssetMapReference(MapStoreId mapStoreId) {
            MapStoreId = mapStoreId;
        }
    }
}