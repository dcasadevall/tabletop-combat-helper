using CameraSystem;
using Map.Rendering;
using Prototype;
using UnityEngine;
using Zenject;

namespace Map {
    /// <summary>
    /// A behaviour used to glue all the map serialized pieces in order to load them into the scene.
    /// </summary>
    public class MapBehaviour : MonoBehaviour {
#pragma warning disable 649
        [SerializeField]
        private RegionHandler _regionHandler;
#pragma warning restore 649

        private IMapRenderer _mapRenderer;
        private ICameraController _cameraController;
        
        public class Factory : PlaceholderFactory<IMapData, MapBehaviour> {
        }

        [Inject]
        public void Construct(IMapData mapData, ICameraController cameraController,
                              IMapRenderer mapRenderer) {
            _mapRenderer = mapRenderer;
            _cameraController = cameraController;
            SetMapData(mapData);
        }
        
        private void SetMapData(IMapData mapData) {
            _mapRenderer.RenderMap(mapData);
            _cameraController.SetRegionHandler(_regionHandler);
        }
    }
}