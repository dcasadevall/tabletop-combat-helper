using MapEditor.MapElement;
using Math;
using UniRx.Async;
using UnityEngine;

namespace MapEditor.SingleTileEditor {
    public interface ISingleTileMapEditorToolDelegate {
        Texture2D CursorTexture { get; }
        
        UniTask Show(IntVector2 tileCoords);
        IMapElement MapElementAtTileCoords(IntVector2 tileCoords);
    }
}