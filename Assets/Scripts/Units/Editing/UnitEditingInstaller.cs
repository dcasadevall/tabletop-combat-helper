using UI;
using UnityEngine;
using Zenject;

namespace Units.Editing {
    /// <summary>
    /// Installer used to inject all dependencies related to unit editing / batch unit selection.
    /// </summary>
    public class UnitEditingInstaller : Installer {
        public const string EDIT_UNITS_OVERLAY_ID = "EditUnits";
        
        private readonly UnitEditingViewController _unitEditingViewController;

        public UnitEditingInstaller(UnitEditingViewController unitEditingViewController) {
            _unitEditingViewController = unitEditingViewController;
        }

        public override void InstallBindings() {
            Container.Bind<IDismissNotifyingViewController>()
                     .WithId(EDIT_UNITS_OVERLAY_ID)
                     .To<UnitEditingViewController>()
                     .FromInstance(_unitEditingViewController)
                     .AsSingle();
        }
    }
}