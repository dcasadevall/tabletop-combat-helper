using CameraSystem;
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
        private SpriteRenderer _backgroundSprite;
        [SerializeField]
        private RegionHandler _regionHandler;
#pragma warning restore 649

        private ICameraController _cameraController;
        
        public class Factory : PlaceholderFactory<IMapData, MapBehaviour> {
        }

        [Inject]
        public void Construct(IMapData mapData, ICameraController cameraController) {
            _cameraController = cameraController;
            SetMapData(mapData);
        }
        
        private void SetMapData(IMapData mapData) {
            _backgroundSprite.sprite = mapData.BackgroundSprite;
            _cameraController.SetRegionHandler(_regionHandler);
        }
    }
}