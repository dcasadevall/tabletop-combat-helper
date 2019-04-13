using CommandSystem;
using Map.Commands;
using Map.UI;
using Zenject;

namespace Map {
    public class MapSelectionLoader : IInitializable {
        private IMapSelectViewController _mapSelectViewController;
        private readonly ICommandQueue _commandQueue;

        public MapSelectionLoader(IMapSelectViewController mapSelectViewController, ICommandQueue commandQueue) {
            _mapSelectViewController = mapSelectViewController;
            _commandQueue = commandQueue;

            _mapSelectViewController.LoadMapClicked += HandleLoadMapClicked;
        }

        public void Initialize() {
            _mapSelectViewController.Show();
        }
        
        private void HandleLoadMapClicked(int mapIndex) {
            _mapSelectViewController.LoadMapClicked -= HandleLoadMapClicked;
            _mapSelectViewController.Hide();
            
            LoadMapCommandData commandData = new LoadMapCommandData((uint)mapIndex);
            _commandQueue.Enqueue(commandData);
        }
    }
}