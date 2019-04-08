using UnityEngine;

namespace Drawing {
    public struct TexturePaintParams {
        public readonly Color color;
        public readonly int brushThickness;

        public static TexturePaintParams MakeWithColor(Color color) {
            return new TexturePaintParams(color, brushThickness: 1);
        }
        
        public static TexturePaintParams MakeWithColor(Color color, int brushThickness) {
            return new TexturePaintParams(color, brushThickness);
        }

        public static TexturePaintParams MakeEraser() {
            return new TexturePaintParams(new Color(), brushThickness: 1);
        }
        
        public static TexturePaintParams MakeEraser(int brushThickness) {
            return new TexturePaintParams(new Color(), brushThickness);
        }
        
        private TexturePaintParams(Color color, int brushThickness) {
            this.color = color;
            this.brushThickness = brushThickness;
        }
    }
}