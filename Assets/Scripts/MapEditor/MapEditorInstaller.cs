using Map.MapData.Store;
using MapEditor.SectionTiles;
using Math;
using UnityEngine;
using Zenject;

namespace MapEditor {
    public class MapEditorInstaller : MonoInstaller {
        public const string SECTION_TILE_EDITOR_ID = "SectionTileEditor";
        public const string ROOM_EDITOR_ID = "RoomEditor";
        public const string SECTION_TILES_CURSOR = "SectionTilesCursor";
        public const string SECTION_TILE_PREFAB = "SectionTilesCursor";

        [SerializeField]
        private MapEditorToolbarViewController _toolbarPrefab;

        [SerializeField]
        private EditSectionTileViewController _editSectionTileVcPrefab;
        
        [SerializeField]
        private SectionTileRenderer _sectionTileRendererPrefab;
        
        [SerializeField]
        private Texture2D _sectionTilesModeCursor;

        // This is injected via the proper installer.
        // We just want to make sure it's only exposed to this installer and the proper command.
        private IMapDataStore _mapDataStore;

        [Inject]
        public void Construct(IMapDataStore mapDataStore) {
            _mapDataStore = mapDataStore;
        }

        public override void InstallBindings() {
            // Toolbar View Controller
            Container.Bind<MapEditorToolbarViewController>()
                     .FromComponentInNewPrefab(_toolbarPrefab)
                     .AsSingle()
                     .NonLazy();
            Container.Bind<IMapDataStore>()
                     .FromInstance(_mapDataStore)
                     .WhenInjectedInto<MapEditorToolbarViewController>();

            // Section Tile Editor
            Container.Bind<EditSectionTileViewController>()
                     .FromComponentInNewPrefab(_editSectionTileVcPrefab)
                     .AsSingle()
                     .Lazy();
            Container.Bind<Texture2D>().WithId(SECTION_TILES_CURSOR).FromInstance(_sectionTilesModeCursor).AsSingle();
            Container.Bind<IMapEditorTool>().WithId(SECTION_TILE_EDITOR_ID).To<SectionTileEditor>().AsSingle();
            Container.BindMemoryPool<SectionTileRenderer, SectionTileRenderer.Pool>()
                     .FromComponentInNewPrefab(_sectionTileRendererPrefab)
                     .WhenInjectedInto<SectionTileVisualizer>();
            Container.BindInterfacesTo<SectionTileVisualizer>().AsSingle();
        }
    }
}