using System;
using UI;
using Units.Serialized;
using UnityEngine;
using Zenject;

namespace Units.Editing {
    public class UnitEditingViewController : MonoBehaviour, IDismissNotifyingViewController {
        public event Action ViewControllerDismissed;

        private IUnitPickerViewController _unitPickerViewController;

        [Inject]
        public void Construct(IUnitPickerViewController unitPickerViewController) {
            _unitPickerViewController = unitPickerViewController;
        }

        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
            ViewControllerDismissed?.Invoke();
        }

        public void HandleAddUnitsPressed() {
            _unitPickerViewController.SpawnUnitClicked += HandleSpawnUnitClicked;
            _unitPickerViewController.Show();
        }

        private void HandleSpawnUnitClicked(IUnitData unitData, int numUnits) {
            Hide();
        }

        public void HandleCancelPressed() {
            Hide();
        }
    }
}