using Grid.Tiles;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Grid.Brushes {
    /// <summary>
    /// This Brush Allows to paint PathTiles with a directionality. 
    /// </summary>
    [CustomGridBrush(true, false, false, "Path Brush")]
    public class PathBrush : GridBrush {
        
        /// <summary>
        /// Determines if the path is drawn in the default direction or the inverse.
        /// </summary>
        public bool isInverse = false;

        /// <summary>
        /// If there is a path being drawn.
        /// </summary>
        public bool isDrawing = true;

        /// <summary>
        /// Last tile position.
        /// </summary>
        private Vector3Int _lastPosition = Vector3Int.zero;

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position) {
            base.Paint(gridLayout, brushTarget, position);
            
            if (isDrawing) {
                Tilemap tilemap = brushTarget.GetComponent<Tilemap>();

                if (tilemap != null) {
                    TileBase lastTile = tilemap.GetTile(position);
                    if ( lastTile != null && lastTile.GetType() == typeof(PathTile)) {
                        PathTile lastPathTile = lastTile as PathTile;
                        lastPathTile.PreviousPathTilePosition = _lastPosition;
                    }
                }
            }
        }
    }
}
