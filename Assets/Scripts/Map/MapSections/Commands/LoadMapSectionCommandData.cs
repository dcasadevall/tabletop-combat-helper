using System.Runtime.Serialization;

namespace Map.MapSections.Commands {
    public class LoadMapSectionCommandData : ISerializable {
        public uint sectionIndex;

        public LoadMapSectionCommandData(uint sectionIndex) {
            this.sectionIndex = sectionIndex;
        }
        
        #region ISerializable
        public LoadMapSectionCommandData(SerializationInfo info, StreamingContext context) {
            sectionIndex = info.GetUInt32("sectionIndex");
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("sectionIndex", sectionIndex);
        }
        #endregion  
    }
}