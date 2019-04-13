
namespace Map.UI {
    public interface IMapSelectViewController {
        event System.Action<int> LoadMapClicked;
        
        void Show();
        void Hide(); 
    }
}
