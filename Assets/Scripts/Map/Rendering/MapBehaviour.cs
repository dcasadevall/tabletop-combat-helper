using CameraSystem;
using UnityEngine;
using Zenject;

namespace Map.Rendering {
    /// <summary>
    /// A behaviour used to glue all the map serialized pieces in order to load them into the scene.
    /// </summary>
    public class MapBehaviour : MonoBehaviour {
#pragma warning disable 649
        [SerializeField]
        private RegionHandler _regionHandler;
#pragma warning restore 649

        private ICameraController _cameraController;
        
        public class Factory : PlaceholderFactory<IMapData, MapBehaviour> {
        }

        [Inject]
        public void Construct(IMapData mapData, ICameraController cameraController,
                              ITileLoader tileLoader) {
            _cameraController = cameraController;
            _cameraController.SetRegionHandler(_regionHandler);
        }
    }
}