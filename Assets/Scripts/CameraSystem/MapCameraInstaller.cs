using UnityEngine;
using Zenject;

namespace CameraSystem {
    /// <summary>
    /// This installer assumes that the camera in current context contains the <see cref="PrototypeCameraController"/>
    /// component, thus having been initialized via <see cref="LobbyCameraInstaller"/>.
    /// </summary>
    public class MapCameraInstaller : MonoInstaller {
        public override void InstallBindings() {
            Camera camera = Container.Resolve<Camera>();
            Container.Bind<ICameraController>().To<PrototypeCameraController>().FromNewComponentOn(camera.gameObject)
                     .AsSingle();
            
            Container.Bind<CameraInput>().AsSingle();
        }
    }
}