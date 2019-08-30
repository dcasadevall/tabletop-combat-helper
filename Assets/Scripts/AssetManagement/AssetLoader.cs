using System;
using System.Collections.Generic;
using UniRx;
using UnityEditor;
using UnityEngine.SceneManagement;
using Zenject;

namespace AssetManagement {
    /// <summary>
    /// This loader class can be used to load a set of assets before proceeding to the next scene.
    /// The functions used to load the assets, as well as the scene to jump to, are injected in the current scene's context.
    /// </summary>
    internal class AssetLoader : IInitializable {
        private readonly DiContainer _container;
        private readonly SceneAsset _nextScene;
        private readonly AssetLoadingFunction[] _assetLoadingFunctions;
        private readonly ZenjectSceneLoader _sceneLoader;
        private readonly List<AssetBinding> _loadedAssets = new List<AssetBinding>();

        public AssetLoader(DiContainer container,
                           SceneAsset nextScene, 
                           AssetLoadingFunction[] assetLoadingFunctions,
                           ZenjectSceneLoader sceneLoader) {
            _container = container;
            _nextScene = nextScene;
            _assetLoadingFunctions = assetLoadingFunctions;
            _sceneLoader = sceneLoader;
        }
        
        public void Initialize() {
            var observables = new List<IObservable<AssetBinding>>(); 
            foreach (var assetLoadingFunction in _assetLoadingFunctions) {
                observables.Add(assetLoadingFunction.Invoke());
            }

            observables.Merge().Subscribe(binding => _loadedAssets.Add(binding), HandleAssetsLoaded);
        }

        private void HandleAssetsLoaded() {
            _sceneLoader.LoadScene(_nextScene.name,
                                   LoadSceneMode.Additive,
                                   container => {
                                       _loadedAssets.ForEach(binding => _container
                                                                        .Bind(binding.type)
                                                                        .FromInstance(binding.asset));
                                   });
        }
    }
}