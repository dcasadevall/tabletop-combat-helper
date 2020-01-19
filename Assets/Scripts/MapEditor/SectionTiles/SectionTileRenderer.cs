using Grid.Positioning;
using Math;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MapEditor.SectionTiles {
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
        }
        
        public class Pool : MonoMemoryPool<IntVector2, string, SectionTileRenderer> {
            protected override void Reinitialize(IntVector2 tileCoords, string sectionName, SectionTileRenderer item) {
                item.SetSectionConnection(tileCoords, sectionName);
            }
        }
    }
}