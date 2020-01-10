using System;
using Units.Spawning;
using Zenject;

namespace Units.Selection {
    public class UnitSelectionInstaller : Installer {
        private readonly UnitMenuViewController _unitMenuViewController;

        public UnitSelectionInstaller(UnitMenuViewController unitMenuViewController) {
            _unitMenuViewController = unitMenuViewController;
        }

        public override void InstallBindings() {
            // TODO: Don't expose UnitSelectionHighlighter globally. only 2 classes need it.
            Container.Bind<UnitSelectionHighlighter>().AsSingle();
            Container.Bind(typeof(IInitializable), typeof(IDisposable)).To<UnitSelectionDetector>().AsSingle();
            Container.Bind<IUnitTransformRegistry>()
                     .To<UnitRegistry>()
                     .FromResolve()
                     .WhenInjectedInto<UnitSelectionHighlighter>();
            
            // UI: Normal and batch selection
            Container.Bind<UnitMenuViewController>()
                     .FromInstance(_unitMenuViewController)
                     .WhenInjectedInto<UnitSelectionDetector>()
                     .Lazy();
            Container.Bind<BatchUnitSelectionDetector>()
                     .AsSingle()
                     .WhenInjectedInto<UnitToolbarViewController>();
            Container.Bind<BatchUnitMenuViewController>()
                     .AsSingle()
                     .WhenInjectedInto<UnitToolbarViewController>();
        }
    }
}