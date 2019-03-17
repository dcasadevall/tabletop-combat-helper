using Grid;
using Grid.Serialized;
using Map.Rendering;
using Map.UI;
using UnityEngine.SceneManagement;
using Zenject;

namespace Map {
    public class MapSelectionLoader : IInitializable {
        // TODO: Inject this
        private const string kCombatSceneName = "CombatScene";
        
        private IMapSelectViewController _mapSelectViewController;
        private readonly ZenjectSceneLoader _sceneLoader;

        public MapSelectionLoader(IMapSelectViewController mapSelectViewController,
                                  ZenjectSceneLoader sceneLoader) {
            _mapSelectViewController = mapSelectViewController;
            _sceneLoader = sceneLoader;
            
            _mapSelectViewController.LoadMapClicked += HandleLoadMapClicked;
        }

        public void Initialize() {
            _mapSelectViewController.Show();
        }
        
        private void HandleLoadMapClicked(IMapData mapData) {
            _mapSelectViewController.LoadMapClicked -= HandleLoadMapClicked;
            _mapSelectViewController.Hide();
            
            _sceneLoader.LoadScene(kCombatSceneName , LoadSceneMode.Additive, container => {
                container.BindInstance(mapData).WhenInjectedInto<TileLoaderFactory>();
                container.BindInstance(mapData).WhenInjectedInto<MapRenderer>();
                container.BindInstance(mapData.GridData).WhenInjectedInto<GridInstaller>();
            });
        }
    }
}