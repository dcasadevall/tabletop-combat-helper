using UnityEngine;

namespace Utils.Random {
    public static class Random
    {
        /// <summary>
        /// Retrieves a random sprite from a given Sprite pool. The sprite picked is randomized based on
        /// a given location (the location of the tile);
        /// </summary>
        /// <param name="pool">Sprite pool to choose from</param>
        /// <param name="location">Position on the map acting as seed for the randomization</param>
        /// <returns></returns>
        public static Sprite GetRandomSpriteFromPool(Sprite[] pool, Vector3Int location) {
            if (pool != null  && pool.Length > 0) {
                long hash = location.x;
                hash = (hash + 0xabcd1234) + (hash << 15);
                hash = (hash + 0x0987efab) ^ (hash >> 11);
                hash ^= location.y;
                hash = (hash + 0x46ac12fd) + (hash << 7);
                hash = (hash + 0xbe9730af) ^ (hash << 11);
                
                var oldState = UnityEngine.Random.state;
                UnityEngine.Random.InitState((int)hash);
                Sprite sprite = pool[(int) (pool.Length * UnityEngine.Random.value)];
                UnityEngine.Random.state = oldState;
                return sprite;
            }
            return null;
        }
    }
}
