using Zenject;

namespace Util {
  public class ClockInstaller : MonoInstaller {
    public override void InstallBindings() {
      Container.Bind<IClock>().To<UnityClock>().AsSingle();
    }
  }
}