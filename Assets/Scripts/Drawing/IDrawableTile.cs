namespace Drawing {
    /// <summary>
    /// An individual drawable tile that will receive mouse input events from <see cref="DrawingInputManager"/>.
    /// It is instantiated by the <see cref="IDrawableTileRegistry"/> if needed.
    /// </summary>
    public interface IDrawableTile {
        void HandleMouseDown();
        void HandleMouseDrag();
        void HandleMouseUp();
    }
}