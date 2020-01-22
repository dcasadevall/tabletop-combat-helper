using System.Collections.Generic;
using CameraSystem;
using Grid.Positioning;
using Logging;
using Math;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace MapEditor.SectionTiles {
    /// <summary>
    /// MonoBehaviour used to render the individual section tiles into the grid. 
    /// </summary>
    public class SectionTileRenderer : MonoBehaviour {
        [SerializeField]
        private TextMesh _sectionLabel;

        private IGridPositionCalculator _gridPositionCalculator;

        [Inject]
        public void Construct(IGridPositionCalculator gridPositionCalculator) {
            _gridPositionCalculator = gridPositionCalculator;
        }

        private void SetSectionConnection(IntVector2 tileCoords, string sectionName) {
            _sectionLabel.text = sectionName;
            transform.position = _gridPositionCalculator.GetTileCenterWorldPosition(tileCoords);
            transform.position = (Vector3)(transform.position * Vector2.one) +
                                 Vector3.forward * DepthConstants.SECTION_TILES_EDITOR_DEPTH;
        }
        
        public class Pool : MonoMemoryPool<IntVector2, string, SectionTileRenderer> {
            private readonly ILogger _logger;
            private Dictionary<IntVector2, SectionTileRenderer> _spawnedRenderers = new Dictionary<IntVector2, SectionTileRenderer>();

            public Pool(ILogger logger) {
                _logger = logger;
            }

            protected override void Reinitialize(IntVector2 tileCoords, string sectionName, SectionTileRenderer item) {
                if (_spawnedRenderers.ContainsKey(tileCoords)) {
                    _logger.Log(LoggedFeature.MapEditor, "Spawning section tile renderer already in tile");
                    Despawn(_spawnedRenderers[tileCoords]);
                }
                
                item.SetSectionConnection(tileCoords, sectionName);
                _spawnedRenderers[tileCoords] = item;
            }

            public void Despawn(IntVector2 tileCoords) {
                if (!_spawnedRenderers.ContainsKey(tileCoords)) {
                    _logger.LogError(LoggedFeature.MapEditor, "Despawning section tile not in coords: {0}", tileCoords);
                    return;
                }

                Despawn(_spawnedRenderers[tileCoords]);
                _spawnedRenderers.Remove(tileCoords);
            }
        }
    }
}