
using CommandSystem;
using Map.Commands;
using Map.UI;
using Replays.Persistence;
using Replays.Persistence.UI;
using Zenject;

namespace EncounterSelection {
    public class EncounterSelectionLoader : IInitializable {
        private readonly EncounterSelectionContext _encounterSelectionContext;
        private readonly ICommandQueue _commandQueue;
        private readonly ICommandHistoryLoader _commandHistoryLoader;
        private readonly IMapSelectViewController _mapSelectViewController;
        private readonly IReplayLoaderViewController _replayLoaderViewController;

        public EncounterSelectionLoader(EncounterSelectionContext encounterSelectionContext,
                                        ICommandQueue commandQueue,
                                        ICommandHistoryLoader commandHistoryLoader,
                                        IMapSelectViewController mapSelectViewController,
                                        IReplayLoaderViewController replayLoaderViewController) {
            _encounterSelectionContext = encounterSelectionContext;
            _commandQueue = commandQueue;
            _commandHistoryLoader = commandHistoryLoader;
            _mapSelectViewController = mapSelectViewController;
            _replayLoaderViewController = replayLoaderViewController;
        }

        public void Initialize() {
            _mapSelectViewController.LoadMapClicked += HandleLoadMapClicked;
            _replayLoaderViewController.LoadReplayClicked += HandleLoadReplayClicked;
            
            _mapSelectViewController.Show();
            _replayLoaderViewController.Show();
        }

        private void HandleLoadMapClicked(int mapIndex) {
            _mapSelectViewController.LoadMapClicked -= HandleLoadMapClicked;
            _replayLoaderViewController.LoadReplayClicked -= HandleLoadReplayClicked;
            _mapSelectViewController.Hide();
            _replayLoaderViewController.Hide();

            _encounterSelectionContext.EncounterType = EncounterType.Combat;
            LoadMapCommandData commandData = new LoadMapCommandData((uint)mapIndex);
            _commandQueue.Enqueue<LoadMapCommand, LoadMapCommandData>(commandData);
        }
        
        private void HandleLoadReplayClicked(string saveName) {
            _replayLoaderViewController.LoadReplayClicked -= HandleLoadReplayClicked;
            _mapSelectViewController.LoadMapClicked -= HandleLoadMapClicked;
            _replayLoaderViewController.Hide();
            _mapSelectViewController.Hide();
            
            _encounterSelectionContext.EncounterType = EncounterType.Replay;
            _commandHistoryLoader.LoadCommandHistory(saveName);
        }
    }
}