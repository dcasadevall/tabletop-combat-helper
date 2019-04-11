using UnityEngine;
using Zenject;

namespace Drawing {
    /// <summary>
    /// Simple class used to save / restore state of a sprite before and after drawing on it.
    /// </summary>
    public class SpriteState : ISpriteState {
        public class Factory : IFactory<Sprite, ISpriteState> {
            public ISpriteState Create(Sprite sprite) {
                return new SpriteState(sprite);
            }
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