using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

#if UNITY_EDITOR

#endif

namespace Grid.Tiles
{
    /// <summary>
    /// Terrain Tiles with a random pool. Takes into consideration the surrounding tiles to
    /// decide the sprite that will be rendered. Also, every permutation has a pool of tiles to select for random
    /// variation. Is a combination of Unity's TerrainTile and RandomRotatingTile from the 2D TileMap Extras
    /// package. 
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "New Terrain Random Tile", menuName = "Tiles/Terrain Random Tile")]
    public class TerrainRandomTile : TileBase
    {
        /// <summary>
        /// The Sprites used for defining the Terrain.
        /// </summary>
        [SerializeField] public SpriteArray[] spritePools;

        /// <summary>
        /// This method is called when the tile is refreshed.
        /// </summary>
        /// <param name="location">Position of the Tile on the Tilemap.</param>
        /// <param name="tileMap">The Tilemap the tile is present on.</param>
        public override void RefreshTile(Vector3Int location, ITilemap tileMap)
        {
            for (int yd = -1; yd <= 1; yd++)
                for (int xd = -1; xd <= 1; xd++)
                {
                    Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                    if (TileValue(tileMap, position))
                        tileMap.RefreshTile(position);
                }
        }

        /// <summary>
        /// Retrieves any tile rendering data from the scripted tile.
        /// </summary>
        /// <param name="location">Position of the Tile on the Tilemap.</param>
        /// <param name="tileMap">The Tilemap the tile is present on.</param>
        /// <param name="tileData">Data to render the tile.</param>
        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            UpdateTile(location, tileMap, ref tileData);
        }

        private void UpdateTile(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;

            int mask = TileValue(tileMap, location + new Vector3Int(0, 1, 0)) ? 1 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(1, 1, 0)) ? 2 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(1, 0, 0)) ? 4 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(1, -1, 0)) ? 8 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(0, -1, 0)) ? 16 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(-1, -1, 0)) ? 32 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(-1, 0, 0)) ? 64 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(-1, 1, 0)) ? 128 : 0;

            byte original = (byte)mask;
            if ((original | 254) < 255) { mask = mask & 125; }
            if ((original | 251) < 255) { mask = mask & 245; }
            if ((original | 239) < 255) { mask = mask & 215; }
            if ((original | 191) < 255) { mask = mask & 95; }

            int index = GetIndex((byte)mask);
            if (index >= 0 && index < spritePools?.Length && TileValue(tileMap, location))
            {
                tileData.sprite = GetRandomSpriteFromPool(spritePools[index].spriteArray, location);
                tileData.transform = GetTransform((byte)mask);
                tileData.color = Color.white;
                tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
                tileData.colliderType = Tile.ColliderType.Sprite;
            }
        }

        /// <summary>
        /// Retrieves a random sprite from a given Sprite pool. The sprite picked is randomized based on
        /// a given location (the location of the tile);
        /// </summary>
        /// <param name="pool">Sprite pool to choose from</param>
        /// <param name="location">Position on the map acting as seed for the randomization</param>
        /// <returns></returns>
        private Sprite GetRandomSpriteFromPool(Sprite[] pool, Vector3Int location) {
            if (pool != null  && pool.Length > 0) {
                long hash = location.x;
                hash = (hash + 0xabcd1234) + (hash << 15);
                hash = (hash + 0x0987efab) ^ (hash >> 11);
                hash ^= location.y;
                hash = (hash + 0x46ac12fd) + (hash << 7);
                hash = (hash + 0xbe9730af) ^ (hash << 11);
                var oldState = Random.state;
                Random.InitState((int)hash);
                Sprite sprite = pool[(int) (pool.Length * Random.value)];
                Random.state = oldState;
                return sprite;
            }
            return null;
        }

        private bool TileValue(ITilemap tileMap, Vector3Int position)
        {
            TileBase tile = tileMap.GetTile(position);
            return (tile != null && tile == this);
        }

        private int GetIndex(byte mask)
        {
            switch (mask)
            {
                case 0: return 0;
                case 1:
                case 4:
                case 16:
                case 64: return 1;
                case 5:
                case 20:
                case 80:
                case 65: return 2;
                case 7:
                case 28:
                case 112:
                case 193: return 3;
                case 17:
                case 68: return 4;
                case 21:
                case 84:
                case 81:
                case 69: return 5;
                case 23:
                case 92:
                case 113:
                case 197: return 6;
                case 29:
                case 116:
                case 209:
                case 71: return 7;
                case 31:
                case 124:
                case 241:
                case 199: return 8;
                case 85: return 9;
                case 87:
                case 93:
                case 117:
                case 213: return 10;
                case 95:
                case 125:
                case 245:
                case 215: return 11;
                case 119:
                case 221: return 12;
                case 127:
                case 253:
                case 247:
                case 223: return 13;
                case 255: return 14;
            }
            return -1;
        }

        private Matrix4x4 GetTransform(byte mask)
        {
            switch (mask)
            {
                case 4:
                case 20:
                case 28:
                case 68:
                case 84:
                case 92:
                case 116:
                case 124:
                case 93:
                case 125:
                case 221:
                case 253:
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -90f), Vector3.one);
                case 16:
                case 80:
                case 112:
                case 81:
                case 113:
                case 209:
                case 241:
                case 117:
                case 245:
                case 247:
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -180f), Vector3.one);
                case 64:
                case 65:
                case 193:
                case 69:
                case 197:
                case 71:
                case 199:
                case 213:
                case 215:
                case 223:
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -270f), Vector3.one);
            }
            return Matrix4x4.identity;
        }
    }
    
    [Serializable]
    public class SpriteArray {
        public Sprite[] spriteArray;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TerrainRandomTile))]
    public class TerrainTileEditor : Editor
    {
        private TerrainRandomTile Tile { get { return (target as TerrainRandomTile); } }
        private readonly bool[] _showFoldouts = new bool[15];
        private readonly String[] _labels = {
            "Filled",
            "Three Sides",
            "Two Sides and One Corner",
            "Two Adjacent Sides",
            "Two Opposite Sides",
            "One Side and Two Corners",
            "One Side and One Lower Corner",
            "One Side and One Upper Corner",
            "One Side",
            "Four Corners",
            "Three Corners",
            "Two Adjacent Corners",
            "Two Opposite Corners",
            "One Corner",
            "Empty"
        };

        public void OnEnable()
        {
            if (Tile.spritePools == null || Tile.spritePools.Length != 15)
            {
                Tile.spritePools = new SpriteArray[15];
                EditorUtility.SetDirty(Tile);
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Set which Sprite is shown based on the surroundings of the tile. " +
                                      "The labels make reference to which tiles on the sides " +
                                      "and corners around the tile don't have the same tile type. " +
                                      "\n\np.e. Filled means all tiles on sides and corners are different type. Empty means " +
                                      "no tiles on sides and corners are different type, so its surrounded by tiles of the " +
                                      "same type in all sides and corners (the naming of the labels makes no sense, and " +
                                      "uses an opposite convention of what would be intuitive or clear to reference which " +
                                      "tiles around are the same type for terrain blending purposes)." +
                                      "\n\nFor every surroundings permutation there's a pool of sprites " +
                                      "from which the sprite will be chosen randomly using the tile position as seed.", 
                                      MessageType.None);

            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 210;

            EditorGUI.BeginChangeCheck();
            
            for (int i = 0; i < 15; i++) {
                EditorGUILayout.Space();
                _showFoldouts[i] = EditorGUILayout.Foldout(_showFoldouts[i], _labels[i]);

                if (_showFoldouts[i]) {

                    int count = EditorGUILayout.DelayedIntField("Number of Sprite Options",
                        Tile.spritePools[i].spriteArray?.Length ?? 1);
                    if (count < 1) {
                        count = 1;
                    }

                    if (Tile.spritePools[i].spriteArray == null || Tile.spritePools[i].spriteArray.Length != count) {
                        Array.Resize(ref Tile.spritePools[i].spriteArray, count);
                    }

                    for (int j = 0; j < count; j++) {
                        Tile.spritePools[i].spriteArray[j] = (Sprite) EditorGUILayout.ObjectField("Option " + (j+1),
                            Tile.spritePools[i].spriteArray[j],
                            typeof(Sprite),
                            false,
                            null);
                    }
                }
            }

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(Tile);

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
    }
#endif
}