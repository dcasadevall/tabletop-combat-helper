using Math;
using UnityEngine;

namespace Drawing.TexturePainter {
    /// <summary>
    /// Implementors of this interface will handle painting over a readWrite enabled Sprite.
    /// </summary>
    public interface ITexturePainter {
        void PaintPixel(Sprite sprite, IntVector2 pixel, TexturePaintParams paintParams);
        void EraseAllPixels(Sprite sprite);
        ISpriteState SaveState(Sprite sprite);
    }
}