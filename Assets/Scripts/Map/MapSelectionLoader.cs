using Grid;
using Grid.Serialized;
using Map.UI;
using UnityEngine.SceneManagement;
using Zenject;

namespace Map {
    public class MapSelectionLoader : IInitializable {
        // TODO: Inject this
        private const string kCombatSceneName = "CombatScene";
        
        private IMapSelectViewController _mapSelectViewController;
        private MapBehaviour.Factory _mapFactory;
        private readonly ZenjectSceneLoader _sceneLoader;

        public MapSelectionLoader(IMapSelectViewController mapSelectViewController, MapBehaviour.Factory mapFactory,
                                  ZenjectSceneLoader sceneLoader) {
            _mapSelectViewController = mapSelectViewController;
            _mapFactory = mapFactory;
            _sceneLoader = sceneLoader;
            
            _mapSelectViewController.LoadMapClicked += HandleLoadMapClicked;
        }

        public void Initialize() {
            _mapSelectViewController.Show();
        }
        
        private void HandleLoadMapClicked(IMapData mapData) {
            _mapSelectViewController.LoadMapClicked -= HandleLoadMapClicked;
            _mapSelectViewController.Hide();
            
            _mapFactory.Create(mapData);
            _sceneLoader.LoadScene(kCombatSceneName , LoadSceneMode.Additive, container => {
                container.BindInstance(mapData).WithId("LoadedMap").WhenInjectedInto<MapInstaller>();
                container.BindInstance(mapData.GridData).WhenInjectedInto<GridInstaller>();
            });
        }
    }
}