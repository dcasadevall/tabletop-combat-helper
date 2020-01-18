namespace Map.MapSections.Commands {
    public class MapSectionWillLoadSignal {
        public readonly IMapSectionData mapSectionData;
        
        public MapSectionWillLoadSignal(IMapSectionData mapSectionData) {
            this.mapSectionData = mapSectionData;
        }
    }
}