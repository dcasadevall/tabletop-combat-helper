using System.Collections.Generic;
using Grid.Tiles.PathTile;
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
            if (tile == null) return;

            if (tile.GetType() != typeof(PathTile)) {
                base.Paint(gridLayout, brushTarget, position);
                return;
            }

            Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null) return;

            PathTile pathTile = tile as PathTile;

            if (isDrawing && (tilemap != _currentTilemap || pathTile != _currentPathTyle || _currentPath == null)) {
                ResetBrush();
            }

            if (isDrawing) {
                ContinuePath(tilemap, position, _currentPath);
            } else {
                _currentTilemap = tilemap;
                _currentPathTyle = pathTile;
                Path newPath = StartPath(tilemap, position, pathTile);
                if (newPath == null) return;
                _currentPath = newPath;
            }

            base.Paint(gridLayout, brushTarget, position);
            _allowedNextPositions = GetAllowedNextPositions(tilemap, position, _currentPath);
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

        private Path StartPath(Tilemap tilemap, Vector3Int position, PathTile pathTile) {
            //TODO: Alberto: Use interceptors at some point?
            Path path = CreatePath(tilemap, position, pathTile.name);
            if (path == null) return null;

            isDrawing = true;

            return path;
        }

        private void ContinuePath(Tilemap tilemap, Vector3Int position, Path currentPath) {
            bool allowedPosition = false;
            foreach (Vector3Int pos in _allowedNextPositions) {
                if (pos.Equals(position)) {
                    allowedPosition = true;
                    break;
                }
            }

            if (allowedPosition == false) {
                ResetBrush();
                return;
            }

            // Add current position to the existing path
            currentPath.AddLastLink(position);
            // Refresh previous link in the path
            tilemap.RefreshTile(currentPath.GetPrevLink(position).position);
        }

        private void ResetBrush() {
            isDrawing = false;
        }

        private Path GetPathOnTile(Tilemap tilemap, Vector3Int position) {
            Path currentPath = null;
            var tilemapPaths = tilemap.GetComponentsInChildren<Path>();
            foreach (var path in tilemapPaths) {
                if (path.ContainsTile(position)) {
                    currentPath = path;
                    break;
                }
            }

            return currentPath;
        }

        private Path CreatePath(Tilemap tilemap, Vector3Int position, string pathName) {
            Path currentPath = GetPathOnTile(tilemap, position);
            if (currentPath != null) return null;

            //TODO: Alberto: Use zenject
            Path path = new GameObject().AddComponent<Path>();
            path.transform.SetParent(tilemap.transform);
            path.gameObject.name = pathName;
            path.AddLastLink(position);
            return path;
        }

        private List<Vector3Int> GetAllowedNextPositions(Tilemap tilemap, Vector3Int position, Path path) {
            float angle = -90;
            PathLink link = path.GetLastLink();
            Vector3 direction = Quaternion.Euler(0, 0, link.rotationAngle) * Vector3.down;

            if (path.Length == 1) {
                // all around is allowed if 
                angle = -270;
                direction = Vector3Int.down;
            }

            if (link.isDiagonal) {
                // start from center if it's a diagonal tile
                angle = 0;
            }

            List<Vector3Int> allowedPositions = new List<Vector3Int>();
            var tilemapPaths = tilemap.GetComponentsInChildren<Path>();
            while (angle <= 90) {
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                Vector3 adjacentDirection = rotation * direction;
                Vector3Int allowedPosition = Vector3Int.RoundToInt(position + adjacentDirection);
                bool isOccupied = false;
                foreach (Path p in tilemapPaths) {
                    if (p.ContainsTile(allowedPosition)) {
                        isOccupied = true;
                        break;
                    }
                }

                if (isOccupied == false) {
                    allowedPositions.Add(Vector3Int.RoundToInt(allowedPosition));
                }

                angle += 45;
            }

            return allowedPositions;
        }
    }
}