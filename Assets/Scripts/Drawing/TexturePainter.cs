using Math;
using UnityEngine;
using Zenject;

namespace Drawing {
    public class TexturePainter : ITexturePainter {
        private readonly IFactory<Sprite, ISpriteState> _spriteStateFactory;

        public TexturePainter(IFactory<Sprite, ISpriteState> spriteStateFactory) {
            _spriteStateFactory = spriteStateFactory;
        }
        
        public void PaintPixel(Sprite sprite, IntVector2 pixel, TexturePaintParams paintParams) {
            Color32[] colors = sprite.texture.GetPixels32();

            for (int x = pixel.x - paintParams.brushThickness; x <= pixel.x + paintParams.brushThickness; x++) {
                // Check if the X wraps around the image, so we don't draw pixels on the other side of the image
                if (x >= (int) sprite.rect.width || x < 0) {
                    continue;
                }

                for (int y = pixel.y - paintParams.brushThickness; y <= pixel.y + paintParams.brushThickness; y++) {
                    PaintPixel(sprite, colors, x, y, paintParams.color);
                }
            }
            
            sprite.texture.SetPixels32(colors);
            sprite.texture.Apply();
        }

        public void EraseAllPixels(Sprite sprite) {
            sprite.texture.SetPixels(new Color[(int)sprite.rect.width * (int)sprite.rect.height]);
            sprite.texture.Apply();
        }

        public ISpriteState SaveState(Sprite sprite) {
            return _spriteStateFactory.Create(sprite);
        }

        #region Helper methods
        private void PaintPixel(Sprite sprite, Color32[] colors, int x, int y, Color color) {
            // Need to transform x and y coordinates to flat coordinates of array
            int arrayPos = y * (int)sprite.rect.width + x;

            // Check if this is a valid position
            if (arrayPos > sprite.texture.GetPixels32().Length || arrayPos < 0) {
                return;
            }

            colors[arrayPos] = color;
        }
        #endregion
    }
}