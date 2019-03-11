using UnityEngine;

namespace Math {
    public static class IntVector2Extensions {
        /// <summary>
        /// Returns true if this <see cref="Rect"/> contains the given <see cref="IntVector2"/> point.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool Contains(this Rect rect, IntVector2 point) {
            if (rect.min.x > point.x) {
                return false;
            }

            if (rect.max.x <= point.x) {
                return false;
            }

            if (rect.min.y > point.y) {
                return false;
            }

            if (rect.max.y <= point.y) {
                return false;
            }

            return true;
        }
    }
}