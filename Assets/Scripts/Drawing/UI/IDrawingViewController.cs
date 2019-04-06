namespace Drawing.UI {
    public interface IDrawingViewController {
        event System.Action DrawingEnabled;
        event System.Action DrawingDisabled;
        
        bool IsDrawing { get; }
    }
}