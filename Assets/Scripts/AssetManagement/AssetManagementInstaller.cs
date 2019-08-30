using System;
using UniRx;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace AssetManagement {
    public class AssetManagementInstaller : MonoInstaller {
        [SerializeField]
        private PreloadedAsset[] _preloadedAssets;
        
        [SerializeField]
        private SceneAsset _nextScene;

        public override void InstallBindings() {
            foreach (var preloadedAsset in _preloadedAssets) {
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