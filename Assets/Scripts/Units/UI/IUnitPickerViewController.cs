using Math;
using Units.Serialized;

namespace Units.UI {
    public interface IUnitPickerViewController {
        event System.Action<IUnitData, int> SpawnUnitClicked;
        
        void Show();
        void Hide();
    }
}