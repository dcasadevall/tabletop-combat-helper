using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Grid.Tiles {
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
        public Vector3Int? PreviousPathTilePosition { get; set; }
        
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);
            if (PreviousPathTilePosition == null) {
                tileData.sprite = spritePools[0].spriteArray[0];
            } else {
                Debug.Log(PreviousPathTilePosition.ToString());
            }
        }
    }
}
