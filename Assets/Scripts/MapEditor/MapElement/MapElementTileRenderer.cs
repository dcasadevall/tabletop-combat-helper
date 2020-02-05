using System.Collections.Generic;
using CameraSystem;
using Grid.Positioning;
using Logging;
using Math;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace MapEditor.MapElement {
    /// <summary>
    /// MonoBehaviour used to render the individual map elements into the grid.
    /// For now, it only supports 1 map element per tile.
    /// </summary>
    public class MapElementTileRenderer : MonoBehaviour {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private TextMesh _label;

        private IGridPositionCalculator _gridPositionCalculator;

        [Inject]
        public void Construct(IGridPositionCalculator gridPositionCalculator) {
            _gridPositionCalculator = gridPositionCalculator;
        }

        private void Initialize(IntVector2 tileCoords, Sprite sprite, string label) {
            _label.text = label;
            _spriteRenderer.sprite = sprite;
            transform.position = _gridPositionCalculator.GetTileCenterWorldPosition(tileCoords);
            transform.position = (Vector3) (transform.position * Vector2.one) +
                                 Vector3.forward * DepthConstants.MAP_EDITOR_ELEMENT_DEPTH;
        }

        public class Pool : MonoMemoryPool<IntVector2, Sprite, string, MapElementTileRenderer> {
            private readonly ILogger _logger;

            private Dictionary<IntVector2, MapElementTileRenderer> _spawnedRenderers =
                new Dictionary<IntVector2, MapElementTileRenderer>();

            public Pool(ILogger logger) {
                _logger = logger;
            }

            protected override void Reinitialize(IntVector2 tileCoords, 
                                                 Sprite sprite,
                                                 string label,
                                                 MapElementTileRenderer item) {
                if (_spawnedRenderers.ContainsKey(tileCoords)) {
                    _logger.Log(LoggedFeature.MapEditor, "Spawning map element renderer already in tile");
                    Despawn(_spawnedRenderers[tileCoords]);
                }

                item.Initialize(tileCoords, sprite, label);
                _spawnedRenderers[tileCoords] = item;
            }

            public void Despawn(IntVector2 tileCoords) {
                if (!_spawnedRenderers.ContainsKey(tileCoords)) {
                    _logger.LogError(LoggedFeature.MapEditor, "Despawning map element not in coords: {0}", tileCoords);
                    return;
                }

                Despawn(_spawnedRenderers[tileCoords]);
                _spawnedRenderers.Remove(tileCoords);
            }
        }
    }
}