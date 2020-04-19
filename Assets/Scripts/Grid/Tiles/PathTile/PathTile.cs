using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
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
                tileData.sprite = spritePools[0].spriteArray[0];
                return;
            }

            PathLink link = currentPath.GetLink(position);
            PathLink prevLink = currentPath.GetPrevLink(position);
            PathLink nextLink = currentPath.GetNextLink(position);

            PathType pathType = GetPathType(link, prevLink, nextLink);
            link.PathType = pathType;
            tileData.sprite = spritePools[(int) pathType].spriteArray[0];

            float angle = GetRotation(link, prevLink, nextLink);
            link.RotationAngle = angle;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            tileData.transform = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);

            tileData.flags = TileFlags.LockTransform;
        }

        private float GetRotation(PathLink link, PathLink prevLink, PathLink nextLink) {
            if (prevLink == null && nextLink == null) {
                return 0;
            }

            if (prevLink == null) {
                Vector3Int directionToNext = nextLink.Position - link.Position;
                return Vector3.SignedAngle(Vector3.down, directionToNext, Vector3.forward);
            }

            Vector3Int directionFromPrev = link.Position - prevLink.Position;
            return Vector3.SignedAngle(Vector3.down, directionFromPrev, Vector3.forward);
        }

        private Path GetCurrentPath(ITilemap tilemap, Vector3Int position) {
            Path[] paths = tilemap.GetComponent<Transform>().GetComponentsInChildren<Path>();
            foreach (Path path in paths) {
                if (path.ContainsTile(position)) {
                    return path;
                }
            }
            return null;
        }

        private PathType GetPathType(PathLink link, PathLink prevLink, PathLink nextLink) {
            
            if (prevLink == null && nextLink == null) {
                return PathType.Single;
            }
            if (prevLink == null) {
                return PathType.Start;
            }
            if (nextLink == null) {
                return PathType.End;
            }
            
            Vector3Int directionFromPrev = prevLink.Position - link.Position;
            Vector3Int directionToNext = link.Position - nextLink.Position;
            float angle = Vector3.SignedAngle(directionFromPrev, directionToNext, Vector3.forward);

            switch (angle) {
                case 0:
                    return PathType.Straight;
                case 45:
                    return PathType.HalfTurnRight;
                case 90:
                    return PathType.TurnRight;
                case -45:
                    return PathType.HalfTurnLeft;
                case -90:
                    return PathType.TurnLeft;
            }
            return PathType.Single;
        }

//        private void UpdatePathTypeAndRotation(Vector3 position) {
//            if (PrevPos == null && NextPos == null) {
//                _pathType = PathType.Single;
//                return;
//            }
//
//            Vector3 directionFromPrev;
//            Vector3 directionToNext;
//            Vector3 nextPosition;
//            Vector3 prevPosition;
//
//            if (PrevPos == null) {
//                _pathType = PathType.Start;
//                nextPosition = ConvertToNonNullableVector3(NextPos);
//                directionToNext = nextPosition - position;
//                _rotation = Vector3.SignedAngle(Vector3.down, directionToNext, Vector3.forward);
//                return;
//            }
//
//            if (NextPos == null) {
//                _pathType = PathType.End;
//                prevPosition = ConvertToNonNullableVector3(PrevPos);
//                directionFromPrev = position - prevPosition;
//                _rotation = Vector3.SignedAngle(Vector3.down, directionFromPrev, Vector3.forward);
//                return;
//            }
//
//            prevPosition = ConvertToNonNullableVector3(PrevPos);
//            directionFromPrev = position - prevPosition;
//            _rotation = Vector3.SignedAngle(Vector3.down, directionFromPrev, Vector3.forward);
//
//            nextPosition = ConvertToNonNullableVector3(NextPos);
//            directionToNext = nextPosition - position;
//            Vector3 relativeDirection = Quaternion.Euler(0, 0, -(_rotation)) * directionToNext;
//
//            // The path is drawn from top down, so turn left is vector right and vice versa.
//            if (relativeDirection.Equals(Vector3.right)) {
//                _pathType = PathType.TurnLeft;
//                return;
//            }
//
//            if (relativeDirection.Equals(Vector3.down)) {
//                _pathType = PathType.Straight;
//                return;
//            }
//
//            if (relativeDirection.Equals(Vector3.left)) {
//                _pathType = PathType.TurnRight;
//                return;
//            }
//        }
    }
}