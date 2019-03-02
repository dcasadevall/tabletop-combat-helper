using Units.Serialized;

namespace Units.UI {
    public interface IUnitPickerViewController {
        event System.Action<IUnitData> SpawnUnitClicked;
        void Show();
        void Hide();
    }
}