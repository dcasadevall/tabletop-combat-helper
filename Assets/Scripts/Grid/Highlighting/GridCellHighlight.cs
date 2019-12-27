using UnityEngine;
using Zenject;

namespace Grid.Highlighting {
    public class GridCellHighlight : MonoBehaviour, IGridCellHighlight {
        [SerializeField]
        private SpriteRenderer _backgroundRenderer;
        
        public void SetColor(Color color) {
            _backgroundRenderer.color = color;
        }
        
        private void Initialize(Vector3 position, Color color) {
            transform.position = position;
            SetColor(color);
        }

        public class Pool : MonoMemoryPool<Vector3, Color, GridCellHighlight> {
            protected override void Reinitialize(Vector3 position, Color color, GridCellHighlight item) {
                item.Initialize(position, color);
            }
        }
    }
}