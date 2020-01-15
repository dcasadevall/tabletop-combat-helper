using Zenject;

namespace Utils.Clock {
  public class ClockInstaller : MonoInstaller {
    public override void InstallBindings() {
      Container.Bind<IClock>().To<UnityClock>().AsSingle();
    }
  }
}