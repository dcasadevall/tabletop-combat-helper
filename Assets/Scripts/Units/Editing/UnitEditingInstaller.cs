using UI;
using UnityEngine;
using Zenject;

namespace Units.Editing {
    /// <summary>
    /// Installer used to inject all dependencies related to unit editing / batch unit selection.
    /// </summary>
    public class UnitEditingInstaller : Installer {
        public const string EDIT_UNITS_OVERLAY_ID = "EditUnits";
        
        private readonly UnitPickerViewController _unitPickerViewController;
        private readonly UnitEditingViewController _unitEditingViewController;

        public UnitEditingInstaller(UnitPickerViewController unitPickerViewController,
                                  UnitEditingViewController unitEditingViewController) {
            _unitPickerViewController = unitPickerViewController;
            _unitEditingViewController = unitEditingViewController;
        }

        public override void InstallBindings() {
            Container.Bind<IUnitPickerViewController>()
                     .To<UnitPickerViewController>()
                     .FromInstance(_unitPickerViewController)
                     .AsSingle();
            
            Container.Bind<IDismissNotifyingViewController>()
                     .To<UnitEditingViewController>()
                     .FromInstance(_unitEditingViewController)
                     .AsSingle()
                     .WithConcreteId(EDIT_UNITS_OVERLAY_ID);
        }
    }
}