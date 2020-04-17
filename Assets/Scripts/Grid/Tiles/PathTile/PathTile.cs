using System;
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
        
        public Path CurrentPath { get; set;}

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);

//            UpdatePathTypeAndRotation(position);

//            PathType pathType = GetPathType();
//            Debug.Log($"PathType value {(int) _pathType}");
            tileData.sprite = spritePools[0].spriteArray[0];
//            tileData.flags = TileFlags.LockTransform;
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

        private Vector3 ConvertToNonNullableVector3(Vector3? nullableVector3) {
            if (nullableVector3 == null) {
                throw new Exception();
            }

            return new Vector3(nullableVector3.GetValueOrDefault().x,
                nullableVector3.GetValueOrDefault().y,
                nullableVector3.GetValueOrDefault().z);
        }
    }
}