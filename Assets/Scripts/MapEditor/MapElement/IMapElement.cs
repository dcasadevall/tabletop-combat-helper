using Math;

namespace MapEditor.MapElement {
    /// <summary>
    /// A map element created with the map editor.
    /// </summary>
    public interface IMapElement {
        void HandleDrag(IntVector2 tileCoords);
        void Remove();
    }
}