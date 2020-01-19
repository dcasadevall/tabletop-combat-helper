using Map.MapSelection.Commands;
using Map.Serialized;
using Zenject;

namespace Map.MapSelection {
    /// <summary>
    /// Installer used only for map editor context.
    /// MapData is injected in a mutable form.
    /// </summary>
    public class MapEditorSelectionInstaller : Installer {
        // Map Selection Data is loaded in a preload scene
        // and injected here.
        private MapSelectionData _mapSelectionData;
        public MapEditorSelectionInstaller(MapSelectionData mapSelectionData) {
            _mapSelectionData = mapSelectionData;
        }
        
        public override void InstallBindings() {
            foreach (var mapReference in _mapSelectionData.mapReferences) {
                Container.Bind<IMutableMapReference>()
                         .To<MapReference>()
                         .FromInstance(mapReference)
                         .WhenInjectedInto<LoadEditableMapCommand>();
            }
        }
    }
}