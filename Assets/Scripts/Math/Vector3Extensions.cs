using UnityEngine;

namespace Math {
    public static class Vector3Extensions {
        private const float kVector3Epsilon = 0.01f;

        /// <summary>
        /// Returns true if this vector equals the given vector.
        /// Uses a much smaller Epsilon comparison (By default, 10.0E-5).
        /// </summary>
        /// <returns><c>true</c>, if both vectors are close enough to meet the precision, <c>false</c> otherwise.</returns>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="precision">Precision.</param>
        public static bool EqualsVector3(this Vector3 a, Vector3 b, float precision = kVector3Epsilon) {
            return Vector3.SqrMagnitude(a - b) < Mathf.Sqrt(precision);
        }
    }
}
