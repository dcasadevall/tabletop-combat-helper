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
        /// The context this will be available in. This context MUST have the asset loading scene context
        /// as parent (error will be thrown otherwise) 
        /// </summary>
        public SceneContext sceneContext;
    }
}