using Math;
using UniRx.Async;

namespace MapEditor.MapElement {
    public interface IMapElementMenuViewController {
        UniTask Show(IntVector2 tileCoords, IMapElement mapElement);
    }
}