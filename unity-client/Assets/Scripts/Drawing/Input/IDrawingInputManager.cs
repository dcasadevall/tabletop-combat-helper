using Drawing.TexturePainter;

namespace Drawing.Input {
    // TODO: This should not expose the ITickable interface,
    // but sadly we can't seem to bind lifecycle interfaces via subcontainer,
    // (even withKernel())
    public interface IDrawingInputManager {
        void Tick(TexturePaintParams paintParams);
    }
}