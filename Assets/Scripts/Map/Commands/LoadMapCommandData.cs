using System;
using System.Runtime.Serialization;

namespace Map.Commands {
    [Serializable]
    public class LoadMapCommandData : ISerializable {
        public uint mapIndex;
        // Scene and section to load. We do not serialize these, as we want the scene to be bound to our binary.
        public readonly string sceneName = "EncounterScene";
        public readonly string sectionSceneName = "MapSectionScene";

        public LoadMapCommandData(uint mapIndex) {
            this.mapIndex = mapIndex;
        }
        
        public LoadMapCommandData(uint mapIndex, string sceneName, string sectionSceneName) {
            this.mapIndex = mapIndex;
            this.sceneName = sceneName;
            this.sectionSceneName = sectionSceneName;
        }
        
        #region ISerializable
        public LoadMapCommandData(SerializationInfo info, StreamingContext context) {
            mapIndex = info.GetUInt32("mapIndex");
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("mapIndex", mapIndex);
        }
        #endregion  
    }
}