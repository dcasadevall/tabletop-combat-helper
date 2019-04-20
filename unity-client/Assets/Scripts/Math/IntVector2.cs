using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace Math {
    /// <summary>
    /// A struct used to represent 2d vectors of integer numbers.
    /// </summary>
    [Serializable]
    public struct IntVector2 : IEquatable<IntVector2>, ISerializable {
        public readonly int x;
        public readonly int y;

        public static IntVector2 Zero {
            get {
                return Of(0, 0);
            }
        }
        
        public static IntVector2 One {
            get {
                return Of(1, 1);
            }
        }

        public static IntVector2 Of(Vector2 vector2) {
            return Of((int) vector2.x, (int) vector2.y);
        }
        
        public static IntVector2 Of(int x, int y) {
            return new IntVector2(x, y);
        }
        
        public static IntVector2 Of(uint x, uint y) {
            return new IntVector2((int)x, (int)y);
        }

        private IntVector2(int x, int y) {
            this.x = x;
            this.y = y;
        }
        
        #region ISerializable
        public IntVector2(SerializationInfo info, StreamingContext context) {
            x = info.GetInt32("x");
            y = info.GetInt32("y");
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("x", x);
            info.AddValue("y", y);
        }
        #endregion
        
        /// <summary>
        ///   <para>Returns a nicely formatted string for this vector.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString() {
            return string.Format("({0}, {1})", x, y);
        }

        #region Equality
        public bool Equals(IntVector2 other) {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            
            return obj is IntVector2 && Equals((IntVector2) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (x * 397) ^ y;
            }
        }
        #endregion

        #region Operators
        public static IntVector2 operator +(IntVector2 a, IntVector2 b) {
            return new IntVector2(a.x + b.x, a.y + b.y);
        }

        public static IntVector2 operator -(IntVector2 a, IntVector2 b) {
            return new IntVector2(a.x - b.x, a.y - b.y);
        }

        public static IntVector2 operator *(IntVector2 a, IntVector2 b) {
            return new IntVector2(a.x * b.x, a.y * b.y);
        }

        public static IntVector2 operator /(IntVector2 a, IntVector2 b) {
            return new IntVector2(a.x / b.x, a.y / b.y);
        }

        public static IntVector2 operator -(IntVector2 a) {
            return new IntVector2(-a.x, -a.y);
        }

        public static IntVector2 operator *(IntVector2 a, int d) {
            return new IntVector2(a.x * d, a.y * d);
        }

        public static IntVector2 operator *(int d, IntVector2 a) {
            return new IntVector2(a.x * d, a.y * d);
        }
        
        public static IntVector2 operator *(IntVector2 a, uint d) {
            return new IntVector2(a.x * (int)d, a.y * (int)d);
        }

        public static IntVector2 operator *(uint d, IntVector2 a) {
            return new IntVector2(a.x * (int)d, a.y * (int)d);
        }

        public static IntVector2 operator /(IntVector2 a, int d) {
            return new IntVector2(a.x / d, a.y / d);
        }

        public static bool operator ==(IntVector2 lhs, IntVector2 rhs) {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(IntVector2 lhs, IntVector2 rhs) {
            return !(lhs == rhs);
        }
        #endregion
    }
}