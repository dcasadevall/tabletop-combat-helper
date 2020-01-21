using System.Linq;
using UnityEngine;

namespace Map.MapData {
    public class SerializedMapData : ScriptableObject, IMutableMapData {
        public string mapName;
        public string MapName {
            get {
                return mapName;
            }
        }
        
        public MapSectionData[] sections;
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
    }
}