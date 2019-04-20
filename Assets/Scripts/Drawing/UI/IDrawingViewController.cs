using Drawing.TexturePainter;
using UnityEngine;

namespace Drawing.UI {
    public interface IDrawingViewController {
        event System.Action DrawingEnabled;
        event System.Action DrawingDisabled;
        event System.Action CancelButtonPressed;

        TexturePaintParams PaintParams { get; }

        void Show();
        void Hide();
    }
}