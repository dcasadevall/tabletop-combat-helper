using UI;
using Units.Serialized;

namespace Units.Spawning.UI {
    public delegate void SpawnUnitClickedDelegate(IUnitData unitData, int numUnits);
    
    public interface IUnitPickerViewController : IDismissNotifyingViewController {
        event SpawnUnitClickedDelegate SpawnUnitClicked;
    }
}