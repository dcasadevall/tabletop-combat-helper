namespace Drawing {
    public interface IDrawingInputManager {
        event System.Action DrawingEnabled;
        event System.Action DrawingDisabled;
        
        bool IsDrawing { get; }
    }
}