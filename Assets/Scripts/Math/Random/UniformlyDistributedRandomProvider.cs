
namespace Math.Random {
    public class UniformlyDistributedRandomProvider : IRandomProvider {
        public void SetSeed(int seed) {
            UnityEngine.Random.InitState(seed);
        }

        public int GetRandomIntegerInRange(int min, int max) {
            if (max <= min) {
                throw new System.Exception("Invalid range provided. Max must be greater than min");
            }

            return UnityEngine.Random.Range(min, max);
        }

        public float GetRandomFloatInRange(float min, float max) {
            if (max <= min) {
                throw new System.Exception("Invalid range provided. Max must be greater than min");
            }

            return UnityEngine.Random.Range(min, max);
        }
    }
}
