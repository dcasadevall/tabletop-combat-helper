using Math;
using Units.Serialized;

namespace Units.UI {
    public delegate void SpawnUnitClickedDelegate(IUnitData unitData, int numUnits);
    
    public interface IUnitPickerViewController {
        event SpawnUnitClickedDelegate SpawnUnitClicked;
        
        void Show();
        void Hide();
    }
}