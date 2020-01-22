using MapEditor.MapElement;
using Math;
using UnityEngine;

namespace MapEditor {
    public interface IMapEditorTool {
        /// <summary>
        /// Returns the map element at the given world position (if any).
        /// </summary>
        /// <param name="tileCoords"></param>
        /// <returns></returns>
        IMapElement MapElementAtTileCoords(IntVector2 tileCoords);
        
        void StartEditing();
        void StopEditing();
    }
}