using System.Linq;
using CameraSystem;
using Grid;
using Math;
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

        private IGrid _grid;
        private ITileLoader _tileLoader;
        private ICameraController _cameraController;
        
        public class Factory : PlaceholderFactory<MapBehaviour> {
        }

        [Inject]
        public void Construct(IMapSectionData mapSectionData, IGrid grid, ICameraController cameraController,
                              ITileLoader tileLoader) {
            _grid = grid;
            _cameraController = cameraController;
        }

        private void Start() {
            Rect gridBounds = _grid.WorldSpaceBounds();
            _regionHandler.Regions.Clear();
            _regionHandler.AddRegion(Vector2.zero);
            _regionHandler.Regions.First().p0 = gridBounds.min;
            _regionHandler.Regions.First().p1 = gridBounds.max;
            
            _cameraController.SetRegionHandler(_regionHandler);
            _regionHandler.Validate();
        }
    }
}