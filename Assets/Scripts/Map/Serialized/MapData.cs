using System;
using System.Linq;
using UnityEngine;

namespace Map.Serialized {
    public class MapData : ScriptableObject, IMapData {
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
                return sections;
            }
            set {
                
            }
        }
    }
}