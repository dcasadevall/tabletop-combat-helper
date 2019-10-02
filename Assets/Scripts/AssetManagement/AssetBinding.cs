using System;

namespace AssetManagement {
    internal class AssetBinding {
        public readonly object asset;
        public readonly Type type;
        
        public AssetBinding(PreloadedAsset preloadedAsset) {
            asset = preloadedAsset.assetReference.Asset;
            type = preloadedAsset.assetReference.Asset.GetType();
        }
    }
}