using Prototype;
using UnityEngine;
using Zenject;

namespace Map {
    [RequireComponent(typeof(RegionHandler))]
    public class MapBehaviour : MonoBehaviour {
        public class Factory : PlaceholderFactory<MapBehaviour> {
        }
        
        private void Start() {
            // TODO: Inject this
            FindObjectOfType<PrototypeCameraController>().SetRegionHandler(GetComponent<RegionHandler>());
        }
    }
}