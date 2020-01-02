using Units.Serialized;

namespace Units.Editing {
    public delegate void SpawnUnitClickedDelegate(IUnitData unitData, int numUnits);
    
    public interface IUnitPickerViewController {
        event SpawnUnitClickedDelegate SpawnUnitClicked;
        
        void Show();
        void Hide();
    }
}