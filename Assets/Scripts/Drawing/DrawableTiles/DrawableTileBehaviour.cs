using Drawing.Input;
using Drawing.TexturePainter;
using UnityEngine;
using Zenject;

namespace Drawing.DrawableTiles {
    /// <summary>
    /// Holds the sprite reference to be used when <see cref="IDrawableTile"/> receives callbacks
    /// from the <see cref="DrawingInputManager"/>.
    /// </summary>
    public class DrawableTileBehaviour : MonoBehaviour, IDrawableTile {
        public class Pool : MonoMemoryPool<Vector2, DrawableTileBehaviour> {
            private readonly IFactory<int, Sprite> _drawableSpriteFactory;
            private int _numTiles = 0;
            
            public Pool(IFactory<int, Sprite> drawableSpriteFactory) {
                _drawableSpriteFactory = drawableSpriteFactory;
            }
            
            protected override void Reinitialize(Vector2 position, DrawableTileBehaviour tile) {
                Sprite sprite = _drawableSpriteFactory.Create(_numTiles);
                _numTiles++;
                if (sprite == null) {
                    return;
                }
                
                tile.Reinitialize(position, sprite);
                tile.Clear();
            }

            protected override void OnDespawned(DrawableTileBehaviour tile) {
                _numTiles--;
                base.OnDespawned(tile);
            }
        }

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        private ITexturePainter _texturePainter;
        
        [Inject]
        public void Construct(ITexturePainter texturePainter) {
            _texturePainter = texturePainter;
        }

        private void Reinitialize(Vector2 position, Sprite sprite) {
            transform.position = position;
            _spriteRenderer.sprite = sprite;
        }

        public Sprite Sprite {
            get {
                return _spriteRenderer.sprite;
            }
        }

        private void Clear() {
            _texturePainter.EraseAllPixels(_spriteRenderer.sprite);
        }
    }
}