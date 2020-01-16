using System;
using System.Runtime.Serialization;
using Map.Commands;

namespace Map.MapSections.Commands {
    [Serializable]
    public class LoadMapSectionCommandData : ISerializable {
        public readonly uint sectionIndex;
        public readonly LoadMapCommandData mapCommandData;

        public LoadMapSectionCommandData(uint sectionIndex, LoadMapCommandData mapCommandData) {
            this.sectionIndex = sectionIndex;
            this.mapCommandData = mapCommandData;
        }
        
        #region ISerializable
        public LoadMapSectionCommandData(SerializationInfo info, StreamingContext context) {
            sectionIndex = info.GetUInt32("sectionIndex");
            mapCommandData = (LoadMapCommandData)info.GetValue("mapCommandData", typeof(LoadMapCommandData));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("sectionIndex", sectionIndex);
            info.AddValue("mapCommandData", mapCommandData);
        }
        #endregion  
    }
}