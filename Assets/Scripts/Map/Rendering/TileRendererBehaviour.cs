using UnityEngine;
using Zenject;

namespace Map.Rendering {
  public class TileRendererBehaviour : MonoBehaviour, ITileRenderer {
    public class Pool : MonoMemoryPool<SpriteRenderer, TileRendererBehaviour> {
      protected override void Reinitialize(SpriteRenderer sprite, TileRendererBehaviour tileRendererBehaviour) {
        tileRendererBehaviour.SetSprite(sprite);
      }
    }

    [SerializeField]
    private SpriteRenderer _sprite;
    
    private void SetSprite(SpriteRenderer sprite) {
      _sprite = sprite;
    }
  }
}