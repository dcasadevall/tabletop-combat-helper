using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils.DataStructures;
using Random = UnityEngine.Random;
#if UNITY_EDITOR

#endif

namespace Grid.Tiles {
    /// <summary>
    /// Random Tiles are tiles which pseudo-randomly pick a sprite from a given list of sprites and a target location, and displays that sprite.
    /// The Sprite displayed for the Tile is randomized based on its location and will be fixed for that particular location.
    /// The random pick can be weighted.
    /// Also, if enabled, a random rotation and flipping is added to the sprite.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "New Random Rotating Tile", menuName = "Tiles/Random Rotating Tile")]
    public class RandomRotatingTile : Tile {

        /// <summary>
        /// Determines if the weighted random should be applied;
        /// </summary>
        [SerializeField] public bool weightedRandomEnabled = false;
        
        /// <summary>
        /// If random rotation is applied to the sprite
        /// </summary>
        [SerializeField] public bool randomRotationEnabled = true;

        /// <summary>
        /// If random flipping in x and y axis is applied to the sprite
        /// </summary>
        [SerializeField] public bool randomFlippingEnabled = true;
        
        /// <summary>
        /// IF random rotation is true, if there are fixed angle intervals, like every 90 degrees. 
        /// </summary>
        [SerializeField] public float rotationIntervals = 0f;

        

        /// <summary>
        /// The Sprites used for randomizing output.
        /// </summary>
        [SerializeField] public WeightedSprite[] weightedSprites;

        /// <summary>
        /// Retrieves any tile rendering data from the scripted tile.
        /// </summary>
        /// <param name="location">Position of the Tile on the Tilemap.</param>
        /// <param name="tileMap">The Tilemap the tile is present on.</param>
        /// <param name="tileData">Data to render the tile.</param>
        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData) {
            base.GetTileData(location, tileMap, ref tileData);

            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;
            
            if ((weightedSprites != null) && (weightedSprites.Length > 0)) {
                long hash = location.x;
                hash = (hash + 0xabcd1234) + (hash << 15);
                hash = (hash + 0x0987efab) ^ (hash >> 11);
                hash ^= location.y;
                hash = (hash + 0x46ac12fd) + (hash << 7);
                hash = (hash + 0xbe9730af) ^ (hash << 11);
                var oldState = Random.state;
                Random.InitState((int) hash);

                if (weightedRandomEnabled) {
                    // Get the cumulative weight of the sprites
                    var cumulativeWeight = 0;
                    foreach (var spriteInfo in weightedSprites) cumulativeWeight += spriteInfo.Weight;

                    // Pick a random weight and choose a sprite depending on it
                    var randomWeight = Random.Range(0, cumulativeWeight);
                    foreach (var spriteInfo in weightedSprites) {
                        randomWeight -= spriteInfo.Weight;
                        if (randomWeight < 0) {
                            tileData.sprite = spriteInfo.Sprite;    
                            break;
                        }
                    }
                } else {
                    tileData.sprite = weightedSprites[(int) (weightedSprites.Length * Random.value)].Sprite;
                }
                
                Quaternion rotation = Quaternion.identity;
                if (randomRotationEnabled) {
                    rotation = GetRandomRotation(rotationIntervals, Random.value);
                }
                
                Vector3 scale = Vector3.one;
                if (randomFlippingEnabled) {
                    scale = GetScaleRandomFlipping(Random.value);
                }

                Random.state = oldState;
                
                tileData.transform = Matrix4x4.TRS(Vector3.zero, rotation, scale);
                tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
                tileData.colliderType = Tile.ColliderType.Sprite;
            }
        }

        /// <summary>
        /// Creates an angle from a random 0 to 1 value with possible fixed angle intervals.
        /// </summary>
        /// <param name="fixedIntervals">The degrees that the angle should snap to, 0 means no snapping</param>
        /// <param name="randomValue">random value from which to extract the angle</param>
        /// <returns></returns>
        private Quaternion GetRandomRotation(float fixedIntervals, float randomValue) {
            float randomAngle = 360 * randomValue;
            if (fixedIntervals > 0) {
                int clampedVal = (int)(randomAngle / fixedIntervals);
                randomAngle = clampedVal * fixedIntervals;
            }
            return Quaternion.Euler(0f, 0f, randomAngle);
        }

        private Vector3 GetScaleRandomFlipping(float randomValue) {
            // Provides a bit for every axis and gives 1 or 0 to each one randomly.
            int axisToFlip = (int) (3 * randomValue);
            Vector3 scaleFlipping = Vector3.one;
            switch (axisToFlip) {
                case 0:
                    break;
                case 1:
                    scaleFlipping.x = -1;
                    break;
                case 2:
                    scaleFlipping.y = -1;
                    break;
            }
            return scaleFlipping;
        }
    }

#if UNITY_EDITOR
    
#endif
}