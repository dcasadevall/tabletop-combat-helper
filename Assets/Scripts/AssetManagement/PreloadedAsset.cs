using System;
using UnityEngine.AddressableAssets;
using Zenject;

namespace AssetManagement {
    [Serializable]
    public class PreloadedAsset {
        /// <summary>
        /// Asset to be preloaded
        /// </summary>
        public AssetReference assetReference;
        /// <summary>
        /// The contexts this will be available in. These contexts MUST have the asset loading scene context
        /// as parent (error will be thrown otherwise) 
        /// </summary>
        public SceneContext[] sceneContexts;
    }
}