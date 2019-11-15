using System;
using System.Collections.Generic;
using UI;
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
        private readonly string _nextScene;
        private readonly AssetLoadingFunction[] _assetLoadingFunctions;
        private readonly ZenjectSceneLoader _sceneLoader;
        private readonly IModalViewController _modalViewController;

        public AssetLoader(DiContainer container,
                           [Inject(Id = "NextScene")] string nextScene,
                           AssetLoadingFunction[] assetLoadingFunctions,
                           ZenjectSceneLoader sceneLoader,
                           IModalViewController modalViewController) {
            _container = container;
            _nextScene = nextScene;
            _assetLoadingFunctions = assetLoadingFunctions;
            _sceneLoader = sceneLoader;
            _modalViewController = modalViewController;
        }

        public void Initialize() {
            var observables = new List<IObservable<AssetBinding>>();
            foreach (var assetLoadingFunction in _assetLoadingFunctions) {
                observables.Add(assetLoadingFunction.Invoke());
            }

            _modalViewController.Show("Loading Assets...");
            observables.Merge().Subscribe(binding => _container
                                                     .Bind(binding.type)
                                                     .FromInstance(binding.asset),
                                          () => {
                                              _modalViewController.Hide();
                                              _sceneLoader.LoadScene(_nextScene, LoadSceneMode.Additive);
                                          });
        }
    }
}