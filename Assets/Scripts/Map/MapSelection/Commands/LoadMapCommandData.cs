using System;
using System.Runtime.Serialization;

namespace Map.MapSelection.Commands {
    [Serializable]
    public class LoadMapCommandData : ISerializable {
        public uint mapIndex;

        //  We do not serialize this, as we want it to be a runtime choice.
        public readonly bool isMapEditor;

        public string SceneName {
            get {
                if (isMapEditor) {
                    return "MapEditorScene";
                }

                return "EncounterScene";
            }
        }

        public string SectionSceneName {
            get {
                if (isMapEditor) {
                    return "MapEditorSectionScene";
                }

                return "MapSectionScene";
            }
        }

        public LoadMapCommandData(uint mapIndex) {
            this.mapIndex = mapIndex;
            this.isMapEditor = false;
        }
        
        public LoadMapCommandData(uint mapIndex, bool isMapEditor) {
            this.mapIndex = mapIndex;
            this.isMapEditor = isMapEditor;
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