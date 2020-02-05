using Map.MapData.Store;
using MapEditor.MapElement;
using MapEditor.PlayerUnits;
using MapEditor.SectionTiles;
using MapEditor.SingleTileEditor;
using MapEditor.Units;
using Math;
using UnityEngine;
using Zenject;

namespace MapEditor {
    public class MapEditorInstaller : MonoInstaller {
        public const string SECTION_TILE_EDITOR_ID = "SectionTileEditor";
        public const string UNIT_TILE_EDITOR_ID = "UnitTileEditor";
        public const string PLAYER_UNITS_TILE_EDITOR_ID = "PlayerUnitsTileEditor";
        public const string SECTION_TILES_CURSOR = "SectionTilesCursor";
        public const string UNIT_TILES_CURSOR = "UnitTilesCursor";
        public const string PLAYER_UNITS_TILES_CURSOR = "PlayerUnitsTilesCursor";
        public const string ROOM_EDITOR_ID = "RoomEditor";

        [SerializeField]
        private MapEditorToolbarViewController _toolbarPrefab;

        [SerializeField]
        private EditSectionTileViewController _editSectionTileVcPrefab;

        [SerializeField]
        private MapElementTileRenderer _mapElementTileRendererPrefab;
        
        [SerializeField]
        private Sprite _sectionTileSprite;
        
        [SerializeField]
        private Sprite _playerUnitsSprite;

        [SerializeField]
        private MapElementMenuViewController _mapElementMenuVcPrefab;

        [SerializeField]
        private Texture2D _sectionTilesModeCursor;

        [SerializeField]
        private Texture2D _unitTilesModeCursor;
        
        [SerializeField]
        private Texture2D _playerUnitsTilesModeCursor;

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

            // Map Elements
            Container.Bind<IMapElementMenuViewController>()
                     .To<MapElementMenuViewController>()
                     .FromComponentInNewPrefab(_mapElementMenuVcPrefab)
                     .AsSingle()
                     .Lazy();
            Container.BindInterfacesTo<MapElementSelectionDetector>().AsSingle();

            // Section Tile Editor
            Container.Bind<EditSectionTileViewController>()
                     .FromComponentInNewPrefab(_editSectionTileVcPrefab)
                     .AsSingle()
                     .Lazy();

            // Note that we bind Texture2D asCached because it is also bound for other cursors.
            // Even WithId, this is necessary.
            // https://gitter.im/Extenject/community?at=5df66794dbf24d0becf4a38e
            Container.Bind<SectionTileMapEditorTool>().AsSingle();
            Container.Bind<Texture2D>().WithId(SECTION_TILES_CURSOR).FromInstance(_sectionTilesModeCursor).AsCached();
            Container.Bind<IMapEditorTool>().WithId(SECTION_TILE_EDITOR_ID)
                     .FromResolveGetter<SectionTileMapEditorTool>(editor => {
                         return Container.Instantiate<SingleTileMapEditorTool>(new[] {editor});
                     }).AsCached();

            Container.Bind<Sprite>().FromInstance(_sectionTileSprite).AsCached()
                     .WhenInjectedInto<SectionTileVisualizer>();
            Container.BindMemoryPool<MapElementTileRenderer, MapElementTileRenderer.Pool>()
                     .FromComponentInNewPrefab(_mapElementTileRendererPrefab)
                     .AsCached()
                     .WhenInjectedInto<SectionTileVisualizer>();
            Container.BindInterfacesTo<SectionTileVisualizer>().AsSingle();
            
            // Unit Tile Editor
            Container.Bind<UnitMapEditorTool>().AsSingle();
            Container.Bind<Texture2D>().WithId(UNIT_TILES_CURSOR).FromInstance(_unitTilesModeCursor).AsCached();
            Container.Bind<IMapEditorTool>().WithId(UNIT_TILE_EDITOR_ID)
                     .FromResolveGetter<UnitMapEditorTool>(editor => {
                         return Container.Instantiate<SingleTileMapEditorTool>(new[] {editor});
                     }).AsCached();
            
            // Initial Player Units Editor
            Container.Bind<PlayerUnitsMapEditorTool>().AsSingle();
            Container.Bind<Texture2D>().WithId(PLAYER_UNITS_TILES_CURSOR).FromInstance(_playerUnitsTilesModeCursor).AsCached();
            Container.Bind<IMapEditorTool>().WithId(PLAYER_UNITS_TILE_EDITOR_ID)
                     .FromResolveGetter<PlayerUnitsMapEditorTool>(editor => {
                         return Container.Instantiate<SingleTileMapEditorTool>(new[] {editor});
                     }).AsCached();

            Container.Bind<Sprite>().FromInstance(_playerUnitsSprite).AsCached()
                     .WhenInjectedInto<PlayerUnitsTileVisualizer>();
            Container.BindMemoryPool<MapElementTileRenderer, MapElementTileRenderer.Pool>()
                     .FromComponentInNewPrefab(_mapElementTileRendererPrefab)
                     .AsCached()
                     .WhenInjectedInto<PlayerUnitsTileVisualizer>();
            Container.BindInterfacesTo<PlayerUnitsTileVisualizer>().AsSingle();
        }
    }
}