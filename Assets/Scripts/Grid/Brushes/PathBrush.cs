using System.Collections.Generic;
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

        /// <summary>
        /// Positions allowed to keep drawing tiles for the path.
        /// </summary>
        private List<Vector3Int> _allowedNextPositions;
        public List<Vector3Int> AllowedNextPositions => _allowedNextPositions;


        /// <summary>
        /// Paint keeps track of the current Path and updates it with the new tile being painted. The paths are
        /// saved as a new object as a child of the tilemap it belongs. The PathTile also accesses the Path object
        /// under a tilemap (if it exists for that position) to determine its TileData.
        /// </summary>
        /// <param name="gridLayout"></param>
        /// <param name="brushTarget"></param>
        /// <param name="position"></param>
        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position) {
            if (cellCount != 1) {
                base.Paint(gridLayout, brushTarget, position);
                return;
            }

            TileBase tile = cells[0].tile;
            if (tile.GetType() != typeof(PathTile)) {
                base.Paint(gridLayout, brushTarget, position);
                return;
            }

            Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null) return;

            PathTile pathTyle = tile as PathTile;

            if (isDrawing && (tilemap != _currentTilemap || pathTyle != _currentPathTyle || _currentPath == null)) {
                ResetBrush();
            }

            if (isDrawing) {
                _currentPath.AddLastLink(position);
                // Refresh previous link in the path
                tilemap.RefreshTile(_currentPath.GetPrevLink(position).Position);
            } else {
                _currentTilemap = tilemap;
                _currentPathTyle = pathTyle;
                _currentPath = GetPathOnTile(tilemap, position, true);
                isDrawing = true;
            }
            base.Paint(gridLayout, brushTarget, position);
            _allowedNextPositions = GetAllowedNextPositions(position, _currentPath);
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position) {
            base.Erase(gridLayout, brushTarget, position);
            
            Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null) return;

            Path path = GetPathOnTile(tilemap, position);

            if (path != null) {
                path.RemoveLink(position);
            }
        }

        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position) {
            base.BoxErase(gridLayout, brushTarget, position);
            
            Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null) return;

            foreach (var pos in position.allPositionsWithin) {
                Path path = GetPathOnTile(tilemap, pos);
                if (path != null) {
                    path.RemoveLink(pos);
                }            
            }
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position) {
            // Paint uses BoxFill of size (1,1,1), so only allow boxFill when painting a single tile
            if (position.size.Equals(Vector3Int.one)) {
                base.BoxFill(gridLayout, brushTarget, position);
            }
        }

        private void ResetBrush() {
            isDrawing = false;
        }

        private Path GetPathOnTile(Tilemap tilemap, Vector3Int position, bool shouldCreate = false) {
            var tilemapPaths = tilemap.GetComponentsInChildren<Path>();
            Path currentPath = null;
            foreach (var path in tilemapPaths) {
                if (path.ContainsTile(position)) {
                    currentPath = path;
                    break;
                }
            }
            if (currentPath == null && shouldCreate) {
                Path path = new GameObject().AddComponent<Path>();
                //TODO: Alberto: Use zenject?
                currentPath = Instantiate(path, tilemap.transform);
                currentPath.gameObject.name = _currentPathTyle.name;
                currentPath.Init();
                currentPath.AddLastLink(position);
            }
            return currentPath;
        }

        private Vector3Int GetLinkDirection(PathLink link) {
            Vector3 direction = Quaternion.Euler(0, 0, link.RotationAngle) * Vector3.down;
            return Vector3Int.RoundToInt(direction);
        }

        private List<Vector3Int> GetAllowedNextPositions(Vector3Int position, Path path) {
            float angle = -90;
            Vector3 direction = Quaternion.Euler(0, 0, path.GetLastLink().RotationAngle) * Vector3.down;
                
            if (path.Length == 1) {
                // all around is allowed if direction is zero
                angle = -270;
                direction = Vector3Int.down;
            }
            
            List<Vector3Int> allowedPositions = new List<Vector3Int>();
            
            while (angle <= 90) {
                Quaternion rotation = Quaternion.Euler(0,0,angle);
                Vector3 adjacentDirection = rotation * direction;
                Vector3 allowedPosition = position + adjacentDirection;
                allowedPositions.Add(Vector3Int.RoundToInt(allowedPosition));
                angle += 45;
            }

            return allowedPositions;
        }
       
    }
}