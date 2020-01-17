using UnityEngine;
using Zenject;

namespace MapEditor {
    public class MapEditorInstaller : MonoInstaller {
        public const string SECTION_TILE_EDITOR_ID = "SectionTileEditor";
        public const string ROOM_EDITOR_ID = "RoomEditor";
        
        [SerializeField]
        private MapEditorToolbarViewController _toolbarPrefab;
        
        public override void InstallBindings() {
            Container.Bind<MapEditorToolbarViewController>()
                     .FromComponentInNewPrefab(_toolbarPrefab)
                     .AsSingle()
                     .NonLazy();

            Container.Bind<IMapEditorTool>().WithId(SECTION_TILE_EDITOR_ID).To<SectionTileEditor>().AsSingle();
        }
    }
}