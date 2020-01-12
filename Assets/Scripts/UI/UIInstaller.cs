using UI.RadialMenu;
using UI.SelectionBox;
using UnityEngine;
using Zenject;

namespace UI {
    /// <summary>
    /// Installs UI elements that can be reused throughout the application. Normally injected in the project context.
    /// </summary>
    public class UIInstaller : MonoInstaller {
        [SerializeField]
        private ModalViewController _modalViewControllerPrefab;
        
        [SerializeField]
        private SelectionBoxBehaviour _selectionBoxBehaviour;

        public override void InstallBindings() {
            Container.Bind<IModalViewController>().To<ModalViewController>()
                     .FromComponentsInNewPrefab(_modalViewControllerPrefab).AsSingle()
                     .Lazy();
            
            Container.Bind<ISelectionBox>().To<SelectionBoxBehaviour>()
                     .FromComponentsInNewPrefab(_selectionBoxBehaviour).AsSingle()
                     .Lazy();
        }
    }
}