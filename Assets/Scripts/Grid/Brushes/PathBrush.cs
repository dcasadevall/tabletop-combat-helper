using Grid.Tiles;
using Grid.Tiles.PathTile;
using UnityEditor;
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
        public bool isDrawing;

        /// <summary>
        /// Last tile position.
        /// </summary>
        private Vector3Int _lastPosition;

        /// <summary>
        /// Last PathTile
        /// </summary>
        private PathTile _lastPathTile;

        /// <summary>
        /// The instance of pathTyle that is being drawn
        /// </summary>
        private PathTile _currentPathTyle;

        /// <summary>
        /// The TileMap where the path is being drawn, to monitor if it changes.
        /// </summary>
        private Tilemap _currentTilemap;

        /// <summary>
        /// The current path being drawn
        /// </summary>
        private Path _currentPath;


        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position) {
            if (cellCount != 1) {
                base.Paint(gridLayout, brushTarget, position);
                return;
            }

            //Debug.Log(cells[0].tile.GetType());
            TileBase tile = cells[0].tile;
            if (tile.GetType() != typeof(PathTile)) {
                base.Paint(gridLayout, brushTarget, position);
                return;
            }

            Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null) return;

            PathTile pathTyle = tile as PathTile;

            if (isDrawing && (tilemap != _currentTilemap || pathTyle != _currentPathTyle)) {
                ResetBrush();
            }

            if (isDrawing) {
                //TODO: Alberto: probably can be done better (setting and erasing every paint call looks weird)
                _currentPathTyle.CurrentPath = _currentPath;

            } else {
                _currentTilemap = tilemap;
                _currentPathTyle = pathTyle;
                _currentPath = GetPathOnTile(tilemap, position);
                _currentPathTyle.CurrentPath = _currentPath;
                isDrawing = true;
            }
            
            base.Paint(gridLayout, brushTarget, position);

            _currentPathTyle.CurrentPath = null;
            
            return;


            PathTile currentPathTile = GetPathTileInPos(tilemap, position);
            if (currentPathTile == null) return;


            if (isDrawing) {
                /*_lastPathTile.NextPos = position;
                currentPathTile.PrevPos = _lastPosition;
                tilemap.RefreshTile(position);
                tilemap.RefreshTile(_lastPosition);

                _lastPosition = position;
                _lastPathTile = currentPathTile;*/
            } else {
                /*_currentTilemap = tilemap;
                tilemap.RefreshTile(position);

                _lastPosition = position;
                _lastPathTile = currentPathTile;*/

                isDrawing = true;
            }
        }

        private void ResetBrush() {
            isDrawing = false;
        }

        private Path GetPathOnTile(Tilemap tilemap, Vector3Int position) {
            var tilemapPaths = tilemap.GetComponentsInChildren<Path>();
            Path currentPath = null;
            foreach (var path in tilemapPaths) {
                if (path.ContainsTile(position)) {
                    currentPath = path;
                    break;
                }
            }
            if (currentPath == null) {
                Path path = new GameObject().AddComponent<Path>();
                //TODO: Alberto: Use zenject?
                currentPath = Instantiate(path, tilemap.transform);
                currentPath.AddLastLink(position);
            }
            return currentPath;
        }

        private PathTile GetPathTileInPos(Tilemap tilemap, Vector3Int position) {
            if (!tilemap.Equals(_currentTilemap)) {
                TilemapChanged(_currentTilemap, tilemap);
            }

            if (tilemap != null) {
                TileBase tile = tilemap.GetTile(position);
                if (tile != null && tile.GetType() == typeof(PathTile)) {
                    PathTile pathTile = tile as PathTile;
                    return pathTile;
                }
            }

            return null;
        }

        private void TilemapChanged(Tilemap oldTilemap, Tilemap newTilemap) {
            Debug.LogWarning("Tilemap changed during path drawing.");
            isDrawing = false;
        }
    }
}