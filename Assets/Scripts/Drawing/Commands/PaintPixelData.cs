using System;
using System.Runtime.Serialization;
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
            _paintParams = (TexturePaintParams) info.GetValue("paintParams", typeof(TexturePaintParams));
            _drawableTileCoords = (IntVector2) info.GetValue("tileCoords", typeof(IntVector2));
            _pixelPosition = (IntVector2) info.GetValue("pixelPosition", typeof(IntVector2));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("paintParams", _paintParams);
            info.AddValue("tileCoords", _drawableTileCoords);
            info.AddValue("pixelPosition", _pixelPosition);
        }
        #endregion 
    }
}