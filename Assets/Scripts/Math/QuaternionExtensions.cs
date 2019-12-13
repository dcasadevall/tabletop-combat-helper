using UnityEngine;

namespace Math {
    public static class QuaternionExtensions {
        private const float kQuaternionEpsilon = 0.01f;

        /// <summary>
        /// Returns true if this quaternion equals the given quaternion. Uses a much smaller Epsilon comparison (10.0E-2).
        /// Use this over == operator or .Equals for more consistent results than Unity's...
        /// </summary>
        /// <returns><c>true</c>, if quaternion equals the other one , <c>false</c> otherwise.</returns>
        /// <param name="self">this quaternion.</param>
        /// <param name="other">other quaternion.</param>
        /// <param name="precision">Precision.</param>
        public static bool EqualsQuaternion(this Quaternion self, Quaternion other, float precision = kQuaternionEpsilon) {
            float angle = Quaternion.Angle(self, other);
            return angle <= precision;
        }
    }
}
