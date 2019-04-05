namespace Drawing {
    /// <summary>
    /// Interface to be used and exposed only to the <see cref="Drawing.UI.IDrawingViewController"/>.
    /// </summary>
    public interface IDrawingInputManagerInternal {
        bool IsEnabled { set; }
    }
}