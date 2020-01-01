using System;
using UI;
using UnityEngine;

namespace Units.Editing {
    public class UnitEditingViewController : MonoBehaviour, IDismissNotifyingViewController {
        public event Action ViewControllerDismissed;

        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
            ViewControllerDismissed?.Invoke();
        }

        public void HandleAddUnitsPressed() {
            
        }
        
        public void HandleSelectUnitsPressed() {
            
        }
        
        public void HandleStopSelectingUnitsPressed() {
            
        }
        
        public void HandleCancelPressed() {
            
        }
    }
}