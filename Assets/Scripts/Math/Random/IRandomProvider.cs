
namespace Math.Random {
    public interface IRandomProvider {
        /// <summary>
        /// Modifies the seed used for this random provider
        /// </summary>
        /// <param name="seed"></param>
        void SetSeed(int seed);

        /// <summary>
        /// Gets an integer from start to end - 1,
        /// chosen randomly with a distribution that may vary per implementation
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        int GetRandomIntegerInRange(int min, int max);

        /// <summary>
        /// Gets a float from start to end - 1,
        /// chosen randomly with a distribution that may vary per implementation
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        float GetRandomFloatInRange(float min, float max);
    }
}
