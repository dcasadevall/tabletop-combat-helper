using Drawing.UI;
using UnityEngine;
using Zenject;

namespace Drawing {
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

        private IDrawingViewController _drawingViewController;
        private ITexturePainter _texturePainter;
        
        /// <summary>
        /// The pixel coordinates of the last pixel that was painted in this "drag" gesture.
        /// </summary>
        private Vector2 _previousPixelCoords;

        [Inject]
        public void Construct(IDrawingViewController drawingViewController, ITexturePainter texturePainter) {
            _drawingViewController = drawingViewController;
            _texturePainter = texturePainter;
        }

        private void Reinitialize(Vector2 position, Sprite sprite) {
            transform.position = position;
            _spriteRenderer.sprite = sprite;
        }

        public void HandleMouseDown(Vector2 point) {
            Vector2 pixelCoords = GetLocalToPixelCoordinates(point);
            _texturePainter.PaintPixel(_spriteRenderer.sprite, pixelCoords, _drawingViewController.PaintParams);

            _previousPixelCoords = pixelCoords;
        }

        public void HandleMouseDrag(Vector2 point) {
            Vector2 pixelCoords = GetLocalToPixelCoordinates(point);
            _texturePainter.PaintPixel(_spriteRenderer.sprite, pixelCoords, _drawingViewController.PaintParams);

            _previousPixelCoords = pixelCoords;
        }

        public void HandleMouseUp(Vector2 point) {
        }

        public void Clear() {
            _texturePainter.EraseAllPixels(_spriteRenderer.sprite);
        }

        private Vector2 GetLocalToPixelCoordinates(Vector2 localPosition) {
            // Scale based on PixelsPerUnit in the sprite.
            float scaledX = localPosition.x * _spriteRenderer.sprite.pixelsPerUnit;
            float scaledY = localPosition.y * _spriteRenderer.sprite.pixelsPerUnit;

            // Round to nearest pixel
            return new Vector2(Mathf.RoundToInt(scaledX), Mathf.RoundToInt(scaledY));
        }
    }
}