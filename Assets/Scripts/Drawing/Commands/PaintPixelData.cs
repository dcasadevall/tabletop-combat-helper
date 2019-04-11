using System;
using System.Runtime.Serialization;
using Drawing.TexturePainter;
using Math;
using UnityEngine;

namespace Drawing.Commands {
    [Serializable]
    public class PaintPixelData : ISerializable {
        public readonly IntVector2 _drawableTileCoords;
        public readonly IntVector2 _pixelPosition;
        public readonly TexturePaintParams _paintParams;

        public PaintPixelData(IntVector2 drawableTileCoords, IntVector2 pixelPosition,
                              TexturePaintParams texturePaintParams) {
            _drawableTileCoords = drawableTileCoords;
            _pixelPosition = pixelPosition;
            _paintParams = texturePaintParams;
        }
        
        #region ISerializable
        public PaintPixelData(SerializationInfo info, StreamingContext context) {
            _paintParams = (TexturePaintParams) info.GetValue(nameof(_paintParams), typeof(TexturePaintParams));
            _drawableTileCoords = (IntVector2) info.GetValue(nameof(_drawableTileCoords), typeof(IntVector2));
            _pixelPosition = (IntVector2) info.GetValue(nameof(_pixelPosition), typeof(IntVector2));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue(nameof(_paintParams), _paintParams);
            info.AddValue(nameof(_drawableTileCoords), _drawableTileCoords);
            info.AddValue(nameof(_pixelPosition), _pixelPosition);
        }
        #endregion 
    }
}