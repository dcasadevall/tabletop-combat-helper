using UnityEngine;

namespace Math {
    [System.Serializable]
    public struct TransformData {
        public readonly Vector3 position;
        public readonly Quaternion rotation;
        public readonly Vector3 scale;

        public TransformData(TransformData other) {
            position = other.position;
            rotation = other.rotation;
            scale = other.scale;
        }

        public static TransformData Of(Transform transform) {
            return new TransformData(transform.position, transform.rotation, transform.lossyScale);
        }

        public static TransformData Of(Vector3 position) {
            return Of(position, default(Quaternion));
        }
        
        public static TransformData Of(Quaternion rotation) {
            return Of(default(Vector3), rotation);
        }

        public static TransformData Of(Vector3 position, Quaternion rotation) {
            return new TransformData(position, rotation, Vector3.one);
        }

        private TransformData(Vector3 position, Quaternion rotation, Vector3 scale) {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }

        /// <summary>
        /// Returns a new copy of the given Transformdata, with the position passed in.
        /// </summary>
        /// <param name="transformData"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static TransformData Copy(TransformData transformData, Vector3 position) {
            return new TransformData(position, transformData.rotation, transformData.scale);
        }

        /// <summary>
        /// Returns a new copy of the given TransformData, with the rotation passed in
        /// </summary>
        /// <param name="transformData"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static TransformData Copy(TransformData transformData, Quaternion rotation) {
            return new TransformData(transformData.position, rotation, transformData.scale);
        }

        public override bool Equals(object obj) {
            if (!(obj is TransformData)) {
                return false;
            }

            TransformData otherData = (TransformData) obj;
            if (!position.EqualsVector3(otherData.position)) {
                return false;
            }

            if (!rotation.EqualsQuaternion(otherData.rotation)) {
                return false;
            }

            if (!scale.EqualsVector3(otherData.scale)) {
                return false;
            }

            return true;
        }

        public override int GetHashCode() {
            return (position.GetHashCode() + "," + rotation.GetHashCode() + "," + scale.GetHashCode()).GetHashCode();
        }

        public override string ToString() {
            return string.Format("Position: {0}. Rotation: {1}. Scale: {2}", position, rotation, scale);
        }
    }
}