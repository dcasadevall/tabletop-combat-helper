using Math;

namespace MapEditor.MapElement {
    public interface IMapElementMenuViewController {
        void Show(IntVector2 tileCoords, IMapElement mapElement);
    }
}