using System;
using Units.Spawning;
using UnityEngine;
using Zenject;

namespace Units.Selection {
    public class UnitSelectionInstaller : MonoInstaller {
        [SerializeField]
        private UnitToolbarViewController _unitToolbarPrefab;

        [SerializeField]
        private UnitMenuViewController _unitMenuPrefab;
        
        [SerializeField]
        private BatchUnitMenuViewController _batchUnitMenuPrefab;
        
        public override void InstallBindings() {
            // TODO: Don't expose UnitSelectionHighlighter globally. only 2 classes need it.
            Container.Bind<UnitSelectionHighlighter>().AsSingle();
            Container.Bind(typeof(IInitializable), typeof(IDisposable)).To<UnitSelectionDetector>().AsSingle();
            Container.Bind<IUnitTransformRegistry>()
                     .To<UnitRegistry>()
                     .FromResolve()
                     .WhenInjectedInto<UnitSelectionHighlighter>();

            // UI: Normal and batch selection
            Container.BindInterfacesTo<UnitToolbarViewController>()
                     .FromComponentInNewPrefab(_unitToolbarPrefab)
                     .AsSingle()
                     .NonLazy();
            
            // UI: Selection prefab. Inject into the installer so we avoid having too many MonoInstallers,
            // while being able to isolate dependencies.
            Container.Bind<UnitMenuViewController>()
                     .FromComponentInNewPrefab(_unitMenuPrefab)
                     .AsSingle()
                     .WhenInjectedInto<UnitSelectionDetector>()
                     .Lazy();
            Container.Bind<BatchUnitMenuViewController>()
                     .FromComponentInNewPrefab(_batchUnitMenuPrefab)
                     .AsSingle()
                     .WhenInjectedInto<UnitToolbarViewController>()
                     .Lazy();
            Container.Bind<BatchUnitSelectionDetector>()
                     .AsSingle()
                     .WhenInjectedInto<UnitToolbarViewController>();
        }
    }
}