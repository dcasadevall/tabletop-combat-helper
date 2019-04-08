using UnityEngine;

namespace Drawing {
    /// <summary>
    /// Implementors of this interface will handle painting over a readWrite enabled Sprite.
    /// </summary>
    public interface ITexturePainter {
        void PaintPixel(Sprite sprite, Vector2 pixel, TexturePaintParams paintParams);
        void EraseAllPixels(Sprite sprite);
    }
}