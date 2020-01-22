using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Map.MapData {
    public class SerializedMapData : ScriptableObject, IMutableMapData, ISerializationCallbackReceiver {
        public string mapName;
        public string MapName {
            get {
                return mapName;
            }
        }
        
        public SerializedMapSectionData[] sections;
        public IMapSectionData[] Sections {
            get {
                return sections.Cast<IMapSectionData>().ToArray();
            }
        }

        IMutableMapSectionData[] IMutableMapData.Sections {
            get {
                return sections.Cast<IMutableMapSectionData>().ToArray();
            }
        }

        public void OnBeforeSerialize() {
            for (uint i = 0; i < sections.Length; i++) {
                sections[i].sectionIndex = i;
            }
        }

        public void OnAfterDeserialize() {
            for (uint i = 0; i < sections.Length; i++) {
                sections[i].sectionIndex = i;
            }
        }
    }
}