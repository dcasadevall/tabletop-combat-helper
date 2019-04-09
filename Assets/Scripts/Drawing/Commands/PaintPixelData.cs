using System;
using Math;
using UnityEngine;

namespace Drawing.Commands {
    [Serializable]
    public class PaintPixelData {
        public readonly IntVector2 _drawableTileCoords;
        public readonly Vector2 _pixelPosition;
        public readonly TexturePaintParams _paintParams;

        public PaintPixelData(IntVector2 drawableTileCoords, Vector2 pixelPosition,
                              TexturePaintParams texturePaintParams) {
            _drawableTileCoords = drawableTileCoords;
            _pixelPosition = pixelPosition;
            _paintParams = texturePaintParams;
        }
    }
}