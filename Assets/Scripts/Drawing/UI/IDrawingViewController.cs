using Drawing.TexturePainter;
using UnityEngine;

namespace Drawing.UI {
    public interface IDrawingViewController {
        event System.Action DrawingEnabled;
        event System.Action DrawingDisabled;
        
        TexturePaintParams PaintParams { get; }
    }
}