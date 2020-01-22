
namespace Map.MapSelection {
    public interface IMapSelectViewController {
        event System.Action<int> LoadMapClicked;
        event System.Action<int> EditMapClicked;
        
        void Show();
        void Hide(); 
    }
}
