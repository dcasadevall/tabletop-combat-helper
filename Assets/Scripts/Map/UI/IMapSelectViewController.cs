
namespace Map.UI {
    public interface IMapSelectViewController {
        event System.Action<IMapData> LoadMapClicked;
        void Show();
        void Hide(); 
    }
}
