using UnityEngine;
using Zenject;

namespace Drawing.TexturePainter {
    /// <summary>
    /// Simple class used to save / restore state of a sprite before and after drawing on it.
    /// </summary>
    public class SpriteState : ISpriteState {
        public class Factory : PlaceholderFactory<Sprite, ISpriteState> {
        }
        
        private readonly Color32[] _pixels;

        public SpriteState(Sprite sprite) {
            _pixels = sprite.texture.GetPixels32();
        }

        public void RestoreState(Sprite sprite) {
            sprite.texture.SetPixels32(_pixels);
            sprite.texture.Apply();
        }
    }
}