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
            _paintParams = new TexturePaintParams(info, context);
            _drawableTileCoords = new IntVector2(info, context);
            _pixelPosition = new IntVector2(info, context);
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            _paintParams.GetObjectData(info, context);
            _drawableTileCoords.GetObjectData(info, context);
            _pixelPosition.GetObjectData(info, context);
        }
        #endregion 
    }
}