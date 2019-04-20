
namespace Math.Random {
    /// <summary>
    /// Use this mock class to test classes that may have a random provider in it.
    /// We can then test based on the expected result per weight
    /// </summary>
    public class AlwaysReturnSameWeightRandomProvider : IRandomProvider {
        private int _weight = 0;

        public AlwaysReturnSameWeightRandomProvider(int weight) {
            _weight = weight;
        }

        public void SetReturnedWeight(int weight) {
            _weight = weight;
        }

        public void SetSeed(int seed) {
            //Do nothing
        }

        public int GetRandomIntegerInRange(int min, int max) {
            return _weight;
        }

        public float GetRandomFloatInRange(float min, float max) {
            return _weight;
        }
    }
}
