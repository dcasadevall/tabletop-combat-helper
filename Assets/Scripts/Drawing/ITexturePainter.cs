using UnityEngine;

namespace Drawing {
    public interface ITexturePainter {
        void PaintPixel(Sprite sprite, Vector2 pixel, TexturePaintParams paintParams);
        void EraseAllPixels(Sprite sprite);
    }
}