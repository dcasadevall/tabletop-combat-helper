using UnityEngine;

namespace Units.Serialized {
    /// <summary>
    /// <see cref="IUnitData"/> implementation using <see cref="ScriptableObject"/>
    /// </summary>
    public class UnitData : ScriptableObject, IUnitData {
        public new string name;
        public string Name {
            get {
                return name;
            }
        }
        
        public Sprite sprite;
        public Sprite Sprite {
            get {
                return sprite;
            }
        }

        public Sprite avatarSprite;
        public Sprite AvatarSprite {
            get {
                return avatarSprite;
            }
        }
    }
}