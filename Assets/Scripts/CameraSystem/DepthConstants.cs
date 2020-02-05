namespace CameraSystem {
    /// <summary>
    /// Some of our visual elements use the Z position to determine overlay priority.
    /// This is not great, but simple and works for now. Consider refactoring if it gets too messy.
    /// </summary>
    public class DepthConstants {
        public const float UNIT_DEPTH = -3.0f;
        public const float CELL_HIGHLIGHT_DEPTH = -2.5f;
        public const float FOG_OF_WAR_DEPTH = -2.0f;
        public const float MAP_EDITOR_ELEMENT_DEPTH = -1.0f;
    }
}