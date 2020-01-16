using Zenject;

namespace Utils.Mesh {
    public class MeshUtilsInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<IGridMeshGenerator>().To<GridMeshGenerator>().AsSingle();
        }
    }
}