using System;
using System.Collections.Generic;
using Logging;
using UniRx;
using UnityEditor;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace AssetManagement {
    public class AssetManagementInstaller : MonoInstaller {
        private readonly ILogger _logger;

        [SerializeField]
        private PreloadedAsset[] _preloadedAssets;

        [SerializeField]
        private SceneAsset _nextScene;

        /// <summary>
        /// This context will be required as a parent to each of the loaded assets scene context.
        /// This ensures that we correctly assign context parents before loading the scene.
        /// </summary>
        [SerializeField]
        private SceneContext _enforcedParentContext;

        public AssetManagementInstaller(ILogger logger) {
            _logger = logger;
        }

        public override void InstallBindings() {
            HashSet<string> allowedParentContracts = new HashSet<string>();
            foreach (var contractName in _enforcedParentContext.ContractNames) {
                allowedParentContracts.Add(contractName);
            }

            foreach (var preloadedAsset in _preloadedAssets) {
                if (!allowedParentContracts.Overlaps(preloadedAsset.sceneContext.ParentContractNames)) {
                    _logger.LogError(LoggedFeature.Assets,
                                     "Preloaded asset scene context: {1} should have a parent contract in: {2}",
                                     preloadedAsset.sceneContext.name,
                                     _enforcedParentContext.name);
                    continue;
                }

                AssetLoadingFunction assetLoadingFunction = () => {
                    var handle = preloadedAsset.assetReference.LoadAssetAsync<object>();
                    return Observable.EveryUpdate().Where(_ => handle.IsDone).FirstOrDefault()
                                     .Timeout(TimeSpan.FromSeconds(10))
                                     .Select(_ => new AssetBinding(preloadedAsset));
                };

                Container.Bind<AssetLoadingFunction>().FromInstance(assetLoadingFunction);
            }

            Container.Bind<IInitializable>().To<AssetLoader>().AsSingle();
            Container.Bind<SceneAsset>().FromInstance(_nextScene).AsSingle();
        }
    }
}