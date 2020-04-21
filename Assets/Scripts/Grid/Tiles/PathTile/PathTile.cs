using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils.Enums;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Grid.Tiles.PathTile {
    /// <summary>
    /// Requires the PathBrush to be painted correctly.
    /// Similar to terrain tiles but where the rotation of the tile matters,
    /// like a cliff, or where there's a direction, like a river. The difference with a terrain tile
    /// is that this path has directionality, and has to be drawn from a beginning to an end.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "New Path Tile", menuName = "Tiles/Path Tile")]
    public class PathTile : TileBase {
        [SerializeField] public SpriteArray[] spritePools;
        
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);

            Path currentPath = GetCurrentPath(tilemap, position);
            if (currentPath == null) {
                if (spritePools.Length == 0) return;
                tileData.sprite = spritePools[0].spriteArray?[0];
                return;
            }

            PathLink link = currentPath.GetLink(position);
            PathLink prevLink = currentPath.GetPrevLink(position);
            PathLink nextLink = currentPath.GetNextLink(position);
            
            bool isDiagonal = IsDiagonal(link, prevLink, nextLink);
            link.isDiagonal = isDiagonal;

            PathType pathType = GetPathType(link, prevLink, nextLink, isDiagonal);
            link.pathType = pathType;
            tileData.sprite = spritePools[(int) pathType].spriteArray[0];

            float angle = GetRotationAngle(link, prevLink, nextLink, isDiagonal);
            link.rotationAngle = angle;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            tileData.transform = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);

            tileData.flags = TileFlags.LockTransform;
        }

        private Path GetCurrentPath(ITilemap tilemap, Vector3Int position) {
            //TODO: Alberto: use injection to get the paths
            Path[] paths = tilemap.GetComponent<Transform>().GetComponentsInChildren<Path>();
            foreach (Path path in paths) {
                if (path.ContainsTile(position)) {
                    return path;
                }
            }
            return null;
        }

        private PathType GetPathType(PathLink link, PathLink prevLink, PathLink nextLink, bool isDiagonal) {
            
            if (prevLink == null && nextLink == null) {
                return PathType.Single;
            }
            if (prevLink == null) {
                return isDiagonal? PathType.DiagonalStart : PathType.Start;
            }
            if (nextLink == null) {
                return isDiagonal? PathType.DiagonalEnd : PathType.End;
            }
            
            Vector3Int directionFromPrev = prevLink.position - link.position;

            Vector3Int directionToNext = link.position - nextLink.position;
            float angle = Vector3.SignedAngle(directionFromPrev, directionToNext, Vector3.forward);

            switch (angle) {
                case 0:
                    return isDiagonal? PathType.Diagonal : PathType.Straight;
                case 45:
                    return isDiagonal? PathType.DiagonalRight : PathType.HalfTurnRight;
                case 90:
                    return PathType.TurnRight;
                case -45:
                    return isDiagonal? PathType.DiagonalLeft : PathType.HalfTurnLeft;
                case -90:
                    return PathType.TurnLeft;
            }
            return PathType.Single;
        }
        
        private float GetRotationAngle(PathLink link, PathLink prevLink, PathLink nextLink, bool isDiagonal) {
            if (prevLink == null && nextLink == null) {
                return 0;
            }
            float angle;
            if (prevLink == null) {
                Vector3Int directionToNext = nextLink.position - link.position;
                angle =  Vector3.SignedAngle(Vector3.down, directionToNext, Vector3.forward);
            } else {
                Vector3Int directionFromPrev = link.position - prevLink.position;
                angle = Vector3.SignedAngle(Vector3.down, directionFromPrev, Vector3.forward);
            }
            return isDiagonal ? angle - 45 : angle;
        }

        private bool IsDiagonal(PathLink link, PathLink prevLink, PathLink nextLink) {
            if (prevLink == null && nextLink == null) {
                return false;
            }
            int absoluteAngle;
            if (prevLink == null) {
                Vector3Int directionToNext = link.position - nextLink.position;
                absoluteAngle = (int) Vector3.Angle(Vector3.down, directionToNext);
            } else {
                Vector3Int directionFromPrev = prevLink.position - link.position;
                absoluteAngle = (int) Vector3.Angle(Vector3.down, directionFromPrev);
            }
            return (absoluteAngle / 45) % 2 == 1;
        }
    }
}