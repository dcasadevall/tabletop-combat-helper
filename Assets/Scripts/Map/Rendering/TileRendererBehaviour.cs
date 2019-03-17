using UnityEngine;
using Zenject;

namespace Map.Rendering {
  /// <summary>
  /// Responsible for rendering a single tile in the tile map.
  /// </summary>
  public class TileRendererBehaviour : MonoBehaviour, ITileRenderer {
    public class Pool : MonoMemoryPool<Sprite, TileRendererBehaviour> {
      protected override void Reinitialize(Sprite sprite, TileRendererBehaviour tileRendererBehaviour) {
        tileRendererBehaviour.SetSprite(sprite);
      }
    }

    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer {
      get {
        return _spriteRenderer;
      }
    }
    
    private void SetSprite(Sprite sprite) {
      _spriteRenderer.sprite = sprite;
    }
  }
}