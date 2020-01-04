using System;
using Zenject;

namespace Units.Selection {
    public class UnitSelectionInstaller : Installer {
        private readonly UnitMenuViewController _unitMenuViewController;

        public UnitSelectionInstaller(UnitMenuViewController unitMenuViewController) {
            _unitMenuViewController = unitMenuViewController;
        }

        public override void InstallBindings() {
            Container.Bind(typeof(IInitializable), typeof(IDisposable)).To<UnitSelectionDetector>().AsSingle();
            
            // UI
            Container.Bind<UnitMenuViewController>()
                     .FromInstance(_unitMenuViewController)
                     .WhenInjectedInto<UnitSelectionDetector>()
                     .Lazy();
        }
    }
}