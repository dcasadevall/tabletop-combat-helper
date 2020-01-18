using MapEditor.SectionTiles;
using UnityEngine;
using Zenject;

namespace MapEditor {
    public class MapEditorInstaller : MonoInstaller {
        public const string SECTION_TILE_EDITOR_ID = "SectionTileEditor";
        public const string ROOM_EDITOR_ID = "RoomEditor";
        public const string SECTION_TILES_CURSOR = "SectionTilesCursor";

        [SerializeField]
        private MapEditorToolbarViewController _toolbarPrefab;

        [SerializeField]
        private EditSectionTileViewController _editSectionTileVcPrefab;
        
        [SerializeField]
        private Texture2D _sectionTilesModeCursor;

        public override void InstallBindings() {
            Container.Bind<MapEditorToolbarViewController>()
                     .FromComponentInNewPrefab(_toolbarPrefab)
                     .AsSingle()
                     .NonLazy();

            // Section Tile Editor
            Container.Bind<EditSectionTileViewController>()
                     .FromComponentInNewPrefab(_editSectionTileVcPrefab)
                     .AsSingle()
                     .Lazy();
            Container.Bind<Texture2D>().WithId(SECTION_TILES_CURSOR).FromInstance(_sectionTilesModeCursor).AsSingle();
            Container.Bind<IMapEditorTool>().WithId(SECTION_TILE_EDITOR_ID).To<SectionTileEditor>().AsSingle();
        }
    }
}