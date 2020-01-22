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
        
        // TODO: Maybe separate the serializable map reference vs the addressable asset reference so we don't have
        // this mutable field.
        public MapStoreId MapStoreId { get; set; }
    }
}