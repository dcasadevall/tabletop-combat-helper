using UI.RadialMenu;
using UnityEngine;
using Zenject;

namespace UI {
    /// <summary>
    /// Installs UI elements that can be reused throughout the application. Normally injected in the project context.
    /// </summary>
    public class UIInstaller : MonoInstaller {
        public static string UNIT_MENU_ID = "UnitMenu";
        
        [SerializeField]
        private ModalViewController _modalViewControllerPrefab;

        [SerializeField]
        private RadialMenuViewController _unitRadialMenuPrefab;

        public override void InstallBindings() {
            Container.Bind<IModalViewController>().To<ModalViewController>()
                     .FromComponentsInNewPrefab(_modalViewControllerPrefab).AsSingle()
                     .Lazy();
//
//            Container.Bind<IRadialMenu>().To<RadialMenuViewController>()
//                     .FromComponentInNewPrefab(_unitRadialMenuPrefab).AsSingle()
//                     .WithConcreteId(UNIT_MENU_ID)
//                     .Lazy();
        }
    }
}