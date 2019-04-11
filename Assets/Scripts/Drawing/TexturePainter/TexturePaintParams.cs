using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace Drawing.TexturePainter {
    [Serializable]
    public struct TexturePaintParams : ISerializable {
        public readonly Color color;
        public readonly int brushThickness;

        public static TexturePaintParams MakeWithColor(Color color, int brushThickness) {
            return new TexturePaintParams(color, brushThickness);
        }

        public static TexturePaintParams MakeEraser(int brushThickness) {
            return new TexturePaintParams(new Color(), brushThickness);
        }
        
        private TexturePaintParams(Color color, int brushThickness) {
            this.color = color;
            this.brushThickness = brushThickness;
        }
        
        #region ISerializable
        public TexturePaintParams(SerializationInfo info, StreamingContext context) {
            brushThickness = info.GetInt32("thickness");
            ColorUtility.TryParseHtmlString(info.GetString("color"), out color);
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("thickness", brushThickness);
            info.AddValue("color", ColorUtility.ToHtmlStringRGBA(color));
        }
        #endregion 
    }
}