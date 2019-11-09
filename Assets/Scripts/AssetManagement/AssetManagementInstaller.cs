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
        private ILogger _logger;

        [SerializeField]
        private PreloadedAsset[] _preloadedAssets;

        [SerializeField]
        private String _nextScene;

        /// <summary>
        /// This context will be required as a parent to each of the loaded assets scene context.
        /// This ensures that we correctly assign context parents before loading the scene.
        /// </summary>
        [SerializeField]
        private SceneContext _enforcedParentContext;

        [Inject]
        public void Construct(ILogger logger) {
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
                                     "Preloaded asset scene context: {0} should have a parent contract in: {1}",
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
            Container.BindInstance(_nextScene).WithId("NextScene").AsSingle();
        }
    }
}