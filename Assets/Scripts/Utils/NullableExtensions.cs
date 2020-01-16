using System;
using Math;

namespace Utils {
    public static class NullableExtensions {
        /// <summary>
        /// Gets the IntVector2 value of this nullable.
        /// Throws an exception if the nullable has no value.
        /// </summary>
        /// <param name="nullable"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IntVector2 GetValueChecked(this IntVector2? nullable) {
            if (nullable == null) {
                throw new Exception("Value is null");
            }

            return nullable.Value;
        }
    }
}