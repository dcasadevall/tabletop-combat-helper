using UnityEngine;

namespace Drawing {
    public interface ITexturePainter {
        void PaintPixel(Sprite sprite, Vector2 pixel, TexturePaintParams paintParams);
        // void PaintBetweenPixels(Texture2D texture2D, Vector2 source, Vector2 target, TexturePaintParams paintParams);
        void EraseAllPixels(Sprite sprite);
    }
}