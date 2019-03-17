using UnityEngine;
using Zenject;

namespace Map.Rendering {
  public class TileRendererBehaviour : MonoBehaviour, ITileRenderer {
    public class Pool : MonoMemoryPool<Sprite, TileRendererBehaviour> {
      protected override void Reinitialize(Sprite sprite, TileRendererBehaviour tileRendererBehaviour) {
        tileRendererBehaviour.SetSprite(sprite);
      }
    }

    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    
    private void SetSprite(Sprite sprite) {
      _spriteRenderer.sprite = sprite;
    }
  }
}